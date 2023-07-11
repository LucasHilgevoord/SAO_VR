using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nervegear : Wearable
{
    [SerializeField] private GameObject _batteryIcon;
    [SerializeField] private GameObject _timeIcon;

    [SerializeField] private bool useVoiceTrigger = true;
    [SerializeField] private VoiceRecognition voiceRecognition;

    public override void StartWearing(Transform parent)
    {
        base.StartWearing(parent);

        _batteryIcon.SetActive(true);
        _timeIcon.SetActive(true);
        StartNextSceneCheck();
    }

    private void StartNextSceneCheck()
    {
        if (useVoiceTrigger) {
            if (voiceRecognition.Init(() => { StartCoroutine(StartEnteringSequence()); }) == true)
            {
                // Wait until the system has recognized the keywords
                Debug.Log("Waiting for voice trigger!");
            } else
            {
                // Speech recognition is not supported
                StartCoroutine(StartEnteringSequence());
            }
        }
    }

    private IEnumerator StartEnteringSequence()
    {
        int randomNumber = Random.Range(0, 2);
        string linkStartVoice = randomNumber == 0 ? "link_start_kirito" : "link_start_asuna";
        float audioDur = AudioManager.Instance.PlayAudio(AudioGroupType.Startup, linkStartVoice);
        
        yield return new WaitForSeconds(audioDur);
        SceneLoader.Instance.LoadScene((int)SceneType.Startup, true, Color.black, 2);
    }
}
