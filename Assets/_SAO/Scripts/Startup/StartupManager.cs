using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StartupManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private int currentSequence = -1;
    [SerializeField] private bool skipStartup;
    [SerializeField] private bool useVoiceTrigger = true;

    [Header("World Elements")]
    [SerializeField] private ParticleSystem bgParticles;

    [Header("Sequence Elements")]
    [SerializeField] private VoiceRecognition voiceRecognition;
    [SerializeField] private ParticleSystem linkStartParticles;
    [SerializeField] private StatusCircleManager statusCircles;
    [SerializeField] private LanguageManager languages;
    [SerializeField] private LoginManager login;
    [SerializeField] private CharacterSelectionManager characterSelection;
    [SerializeField] private EnterSAO enterSAO;

    private void Awake()
    {
        LanguageManager.ClosedLanguages += OnLanguageSelected;
        voiceRecognition.PhraseTriggered += OnVoiceTriggerReceived;
    } 

    private void Start()
    {
        // Ignore the startup sequence and go directly to the game
        if (skipStartup)
        {
            SceneLoader.Instance.LoadScene((int)SceneType.Interface, true, Color.white);
            return;
        }

        //// Check a voice trigger is availabe and enabled to start the sequence,
        //if (useVoiceTrigger && voiceRecognition.Init() == true)
        //{
        //    // Wait until the system has recognized the keywords
        //    Debug.Log("Waiting for voice trigger!");
        //}
        //else if ((useVoiceTrigger == false || voiceRecognition.Init() == false) && currentSequence == 0)
        //{
        //    // Speech recognition is not supported
        //    int randomNumber = UnityEngine.Random.Range(0, 2);
        //    string linkStartVoice = randomNumber == 0 ? "link_start_kirito" : "link_start_asuna";
        //    float audioDur = AudioManager.Instance.PlayAudio(AudioGroupType.Startup, linkStartVoice);
        //    StartCoroutine(DelayStartSequence(audioDur - 0.5f));
        //} else
        //{
        //    StartStartupSequence();
        //}
    }

    private IEnumerator DelayStartSequence(float duration)
    {
        yield return new WaitForSeconds(duration);
        StartStartupSequence();
    }

    private void OnVoiceTriggerReceived() 
    {
        voiceRecognition.PhraseTriggered -= OnVoiceTriggerReceived;
        StartStartupSequence(); 
    }

    private void StartStartupSequence() { NextSequence(false); }

    private void NextSequence(bool addIndex = true)
    {
        if (addIndex)
            currentSequence += 1;

        switch (currentSequence)
        {
            case 0:
                StartLinkStartParticles();
                break;
            case 1:
                StartStatusCircles();
                break;
            case 2:
                StartLanguages();
                break;
            case 3:
                StartLogin();
                break;
            case 4:
                StartCharacterSelection();
                break;
            case 5:
                StartEnteringSAO();
                break;
            default:
                break;
        }
    }

    private void TimeSequence(float sequenceDuration, GameObject parent = null, float nextSequenceDelay = 0)
    {
        StartCoroutine(WaitForSequenceFinished(sequenceDuration, parent));

        float delay = sequenceDuration + nextSequenceDelay;
        StartCoroutine(WaitForNextSequence(delay));
    }

    private IEnumerator WaitForSequenceFinished(float sequenceDuration, GameObject parent = null)
    {
        yield return new WaitForSeconds(sequenceDuration);
        if (parent != null) parent.SetActive(false);
    }

    private IEnumerator WaitForNextSequence(float nextSequenceDelay = 0)
    {
        yield return new WaitForSeconds(nextSequenceDelay);
        NextSequence();
    }

    private void StartLinkStartParticles()
    {
        Debug.Log("Started Sequence: LinkStart Particles");
        AudioManager.Instance.PlayAudio(AudioGroupType.Startup, "startup_particles");
        linkStartParticles.gameObject.SetActive(true);
        TimeSequence(linkStartParticles.main.duration + 2, linkStartParticles.gameObject, -0.5f);
    }

    private void StartStatusCircles()
    {
        Debug.Log("Started Sequence: Status Circles");
        statusCircles.gameObject.SetActive(true);
        TimeSequence(6f, statusCircles.gameObject);
    }

    private void StartLanguages()
    {
        Debug.Log("Started Sequence: Languages");
        languages.gameObject.SetActive(true);
    }

    private void OnLanguageSelected()
    {
        Debug.Log("Finished Sequence: Language");
        TimeSequence(0.5f, languages.gameObject);
        LanguageManager.ClosedLanguages -= OnLanguageSelected;
    }

    private void StartLogin()
    {
        Debug.Log("Started Sequence: Login");
        LoginManager.ClosedWindow += OnLoginCompleted;

        login.gameObject.SetActive(true);
    }

    private void OnLoginCompleted()
    {
        LoginManager.ClosedWindow -= OnLoginCompleted;
        TimeSequence(0.5f, login.gameObject);
    }

    private void StartCharacterSelection()
    {
        Debug.Log("Started Sequence: Character Selection");
        CharacterSelectionManager.CharacterSelected += OnCharacterSelectionCompleted;
        characterSelection.gameObject.SetActive(true);
    }

    private void OnCharacterSelectionCompleted()
    {
        Debug.Log("Completed Sequence: Character Selection");
        CharacterSelectionManager.CharacterSelected -= OnCharacterSelectionCompleted;
        TimeSequence(0.5f, characterSelection.gameObject);
    }

    private void StartEnteringSAO()
    {
        Debug.Log("Started Sequence: Enter SAO");
        enterSAO.gameObject.SetActive(true);
        enterSAO.StartSequence();
    }
}
