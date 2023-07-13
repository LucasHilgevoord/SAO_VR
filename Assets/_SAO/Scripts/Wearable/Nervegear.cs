using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class NerveGear : Wearable
{
    [SerializeField] private GameObject _batteryIcon;
    [SerializeField] private TextMeshPro _timeText;
    [SerializeField] private TextMeshPro _systemText;

    [SerializeField] private bool useVoiceTrigger = true;
    [SerializeField] private VoiceRecognition voiceRecognition;

    private bool _linkStarted;
    private bool _blink;
    private float _blinkInterval = 0.5f;

    private List<InputDevice> devices = new List<InputDevice>();

    public override void StartWearing(Transform parent)
    {
        base.StartWearing(parent);
        _batteryIcon.SetActive(true);
        _timeText.gameObject.SetActive(true);
        StartCoroutine(UpdateTime());
        
        StartInputCheck();
    }

    private IEnumerator UpdateTime()
    {
        while (true)
        {
            _blink = !_blink;
            string timeFormat = _blink ? "HH mm" : "HH:mm";
            _timeText.text = DateTime.Now.ToString(timeFormat);
            yield return new WaitForSeconds(_blinkInterval);
        }
    }

    private void StartInputCheck()
    {
        if (useVoiceTrigger) {
            if (voiceRecognition.Init(() => { StartCoroutine(StartEnteringSequence()); }) == true)
            {
                // Wait until the system has recognized the keywords
                Debug.Log("Waiting for voice trigger!");
                _systemText.text = "<b>Voice Command Required:</b>\r\nPlease audibly state 'Link Start' to enter \r\n'Sword Art Online'.\r\n";
            }
            else
            {
                // Speech recognition is not supported
                _systemText.text = "<b>System Alert:</b>\r\nMicrophone is currently unavailable. \r\nPlease press 'A' to enter \r\n'Sword Art Online'";

                // Get the controllers
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller, devices);
                StartCoroutine(CheckForControllerInput());
            }
        }
        _systemText.gameObject.SetActive(true);
    }

    private IEnumerator CheckForControllerInput()
    {
        while (!_linkStarted)
        {
            foreach (InputDevice controller in devices)
            {
                if (controller.isValid && controller.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
                {
                    Debug.Log("A button was pressed");
                    StartCoroutine(StartEnteringSequence());
                    yield break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator StartEnteringSequence()
    {
        _linkStarted = true;
        int randomNumber = UnityEngine.Random.Range(0, 2);
        string linkStartVoice = randomNumber == 0 ? "link_start_kirito" : "link_start_asuna";
        float audioDur = AudioManager.Instance.PlayAudio(AudioGroupType.Startup, linkStartVoice);
        
        yield return new WaitForSeconds(audioDur);
        SceneLoader.Instance.LoadScene((int)SceneType.Startup, true, Color.black, 1);
    }
}
