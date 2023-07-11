using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preboot : MonoBehaviour
{
    private bool audioManagerInitialized;

    private void Awake()
    {
        AudioManager.Initialized += OnAudioManagerInitialized;
    }
    private void OnDestroy()
    {
        AudioManager.Initialized -= OnAudioManagerInitialized;
    }

    private void Update()
    {
        if (audioManagerInitialized)
        {
            SceneLoader.Instance.LoadScene(1, false);
        }
    }

    private void OnAudioManagerInitialized() { audioManagerInitialized = true; }
}
