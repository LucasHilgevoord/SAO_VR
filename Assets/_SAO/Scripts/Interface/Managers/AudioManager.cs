using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioGroupType
{
    Global,
    Background,
    Startup,
    Interface
}

public class AudioManager : Singleton<AudioManager>
{
    public static event Action Initialized;

    /// <summary> The AudioListener which will receive the audio </summary>
    public AudioListener audioListener;

    /// <summary> List of all possible AudioSources on which can be played </summary>
    private List<AudioSource> audioSources;

    /// <summary> Total AudioSources that can remain on the object if there aren't more required</summary>
    private int presetAudioSourcesAmount;

    /// <summary> List of all useable AudioClips </summary>
    [SerializeField] private List<AudioGroup> audioGroups;

    internal override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        // Assign all the preset AudioSources from the gameObject
        audioSources = new List<AudioSource>(this.GetComponents<AudioSource>());
        presetAudioSourcesAmount = audioSources.Count;
        Initialized?.Invoke();
    }

    private void Start()
    {
        
    }

    /// <summary>
    /// Method to play a specific audioclip
    /// </summary>
    /// <param name="clipName">Name of the clip</param>
    /// <param name="volume">Volume of the source</param>
    /// <param name="pitch">Pitch of the source</param>
    /// <param name="delay">Delay before playing</param>
    /// <param name="loop">Should the clip be looped</param>
    public float PlayAudio(AudioGroupType audioGroup, string clipName, Action completeCallback = null, float volume = 1, float pitch = 1, float delay = 0, bool loop = false)
    {
        // Search for the audiolistener if there is none assigned
        if (audioListener == null) { FindAudioListener(); }

        AudioSource audioSource = GetAvailableSource();
        if (audioSource == null) { throw new ObjectUnavailableException("No audioSource available!"); }

        // Iterate over assigned audioGroups to find the list with the desired clip
        AudioGroup group = audioGroups.Find(g => g.id == audioGroup.ToString().ToLower());
        if (group == null) { throw new KeyNotFoundException("There is no audioGroup with the id: " + audioGroup.ToString().ToLower()); }

        // Iterate over the clips to find the right clip
        AudioClip clip = null;
        for (int i = 0; i < group.audioClips.Count; i++)
        {
            if (group.audioClips[i].name == clipName)
            {
                clip = group.audioClips[i];
                break;
            }
        }

        if (clip != null)
        {
            // Play the clip and assign all attributes
            audioSource.PlayOneShot(clip, volume);
            audioSource.pitch = pitch;
            audioSource.loop = loop;

            StartCoroutine(WaitForAudioFinish(audioSource, clip.length, completeCallback));
        } else
            throw new KeyNotFoundException("No clip found with the name: " + clipName);

        // Return the lenght of the clip
        return clip.length;
    }

    /// <summary>
    /// Method to stop a specific audioclip
    /// </summary>
    /// <param name="clipName">Name of the clip</param>
    public void StopAudio(string clipName, Action callback)
    {
        AudioSource source = null;

        // Find the AudioSource which is playing the assigned clip
        for (int i = 0; i < audioSources.Count; i++)
        {
            if (audioSources[i].clip.name == clipName)
                source = audioSources[i];
        }

        if (source != null)
        {
            // Stop the AudioSource
            source.Stop();
            AudioFinished(source, callback);
        } else
        {
            throw new KeyNotFoundException("No audioSource found which is currently playing: " + clipName);
        }
    }

    /// <summary>
    /// Method to find/create an AudioSource which is not yet in use
    /// </summary>
    /// <returns>Available AudioSource</returns>
    private AudioSource GetAvailableSource()
    {
        // Find an AudioSource which is not yet playing anything
        for (int i = 0; i < audioSources.Count; i++)
        {
            if (audioSources[i].isPlaying == false)
                return audioSources[i];
        }

        // Create a new AudioSource Component so that it can be used
        AudioSource newSource = this.gameObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        audioSources.Add(newSource);
        return newSource;
    }

    /// <summary>
    /// Method which waits until the AudioSource is done playing
    /// </summary>
    /// <param name="source">AudioSource which should be waited on</param>
    /// <param name="clipLenght">Duration of the clip</param>
    /// <returns>Finished AudioSource</returns>
    private IEnumerator WaitForAudioFinish(AudioSource source, float clipLenght, Action completeCallback)
    {
        yield return new WaitForSeconds(clipLenght);
        AudioFinished(source, completeCallback);
    }

    /// <summary>
    /// Method which removes unnecessary AudioSource Components from the gameObject
    /// </summary>
    /// <param name="source">AudioSource which is done playing</param>
    private void AudioFinished(AudioSource source, Action completeCallback)
    {
        // Remove the AudioSource if there are too many sources present
        if (audioSources.Count > presetAudioSourcesAmount)
        {
            audioSources.Remove(source);
            Destroy(source);
        }

        // Call the complete callback if assigned
        if (completeCallback != null) { completeCallback?.Invoke(); }
    }

    /// <summary>
    /// Method to set the audioListener
    /// </summary>
    private void FindAudioListener(AudioListener listener = null) { audioListener = listener != null ? listener : Camera.main.GetComponent<AudioListener>(); }
}