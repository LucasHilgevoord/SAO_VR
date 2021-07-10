using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    /// <summary> The AudioListener which will receive the audio </summary>
    public AudioListener audioListener;

    /// <summary> List of all possible AudioSources on which can be played </summary>
    private List<AudioSource> audioSources;

    /// <summary> Total AudioSources that can remain on the object if there aren't more required</summary>
    private int presetAudioSourcesAmount;

    /// <summary> List of all useable AudioClips </summary>
    [SerializeField] private List<AudioClip> clips;

    internal override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        SceneLoader.SwitchSceneComplete += OnSceneSwitched;
    }

    private void OnDestroy()
    {
        SceneLoader.SwitchSceneComplete -= OnSceneSwitched;
    }

    private void Start()
    {
        // Assign all the preset AudioSources from the gameObject
        FindAudioListener();

        audioSources = new List<AudioSource>(this.GetComponents<AudioSource>());
        presetAudioSourcesAmount = audioSources.Count;
    }

    /// <summary>
    /// Methode to play a specific audioclip
    /// </summary>
    /// <param name="clipName">Name of the clip</param>
    /// <param name="volume">Volume of the source</param>
    /// <param name="pitch">Pitch of the source</param>
    /// <param name="delay">Delay before playing</param>
    /// <param name="loop">Should the clip be looped</param>
    public void PlayAudio(string clipName, float volume = 1, float pitch = 1, float delay = 0, bool loop = false)
    {
        AudioSource mySource = GetAvailableSource();
        AudioClip clip = null;

        // Iterate over the clips to find the right clip
        for (int i = 0; i < clips.Count; i++)
        {
            if (clips[i].name == clipName)
            {
                clip = clips[i];
                break;
            }
        }

        if (clip != null)
        {
            // Play the clip and assign all attributes
            mySource.PlayOneShot(clip, volume);
            mySource.pitch = pitch;
            mySource.loop = loop;

            StartCoroutine(WaitForAudioFinish(mySource, clip.length));
        } else
        {
            throw new KeyNotFoundException("No clip found with the name: " + clipName);
        }
    }

    /// <summary>
    /// Methode to stop a specific audioclip
    /// </summary>
    /// <param name="clipName">Name of the clip</param>
    public void StopAudio(string clipName)
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
            AudioFinished(source);
        } else
        {
            throw new KeyNotFoundException("No audioSource found which is currently playing: " + clipName);
        }
    }

    /// <summary>
    /// Methode to find/create an AudioSource which is not yet in use
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
    /// Methode which waits until the AudioSource is done playing
    /// </summary>
    /// <param name="source">AudioSource which should be waited on</param>
    /// <param name="clipLenght">Duration of the clip</param>
    /// <returns>Finished AudioSource</returns>
    private IEnumerator WaitForAudioFinish(AudioSource source, float clipLenght)
    {
        yield return new WaitForSeconds(clipLenght);
        AudioFinished(source);
    }

    /// <summary>
    /// Methode which removes unnecessary AudioSource Components from the gameObject
    /// </summary>
    /// <param name="source">AudioSource which is done playing</param>
    private void AudioFinished(AudioSource source)
    {
        // Remove the AudioSource if there are too many sources present
        if (audioSources.Count > presetAudioSourcesAmount)
        {
            audioSources.Remove(source);
            Destroy(source);
        }
    }

    /// <summary>
    /// Method to set the audioListener
    /// </summary>
    private void FindAudioListener(AudioListener listener = null) { audioListener = listener != null ? listener : Camera.main.GetComponent<AudioListener>(); }

    private void OnSceneSwitched(SceneType type) { FindAudioListener(); }
}