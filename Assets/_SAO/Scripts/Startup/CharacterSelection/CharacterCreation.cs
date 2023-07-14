using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterCreation : MonoBehaviour
{
    public event Action CharacterCreated;
    private bool _isStarted;

    [SerializeField] private GameObject characterCreation;
    [SerializeField] private NameHandler nameHandler;
    [SerializeField] private Color _fadeColor;

    private void Update()
    {
        if (!_isStarted) return;

        if (InputHandler.Instance.wasKeyPressedThisFrame(Key.Enter))
            OnCredentialsAccepted();

    }

    internal void StartCharacterCreation()
    {
        OverlayEffects.Instance.FadeIn(1f, _fadeColor, 0, EnableCharacterCreator);
    }

    private void EnableCharacterCreator()
    {
        characterCreation.SetActive(true);

        // Set the visuals of the scene
        OverlayEffects.Instance.FadeOut(1f, _fadeColor, 0.5f, () => { StartCoroutine(ShowNameInput()); });
    }

    private IEnumerator ShowNameInput()
    {
        yield return new WaitForSeconds(0.5f);

        nameHandler.gameObject.SetActive(true);
        NameHandler.CredentialsAccepted += OnCredentialsAccepted;
    }

    private void OnCredentialsAccepted()
    {
        NameHandler.CredentialsAccepted -= OnCredentialsAccepted;
        OverlayEffects.Instance.FadeIn(1f, Color.white, 0, DisableCharacterCreator);
    }

    private void DisableCharacterCreator()
    {
        // Set the visuals of the scene
        nameHandler.gameObject.SetActive(false);
        characterCreation.SetActive(false);
        OverlayEffects.Instance.FadeInOut(1f, _fadeColor, 0.5f, () => { CharacterCreated?.Invoke(); });
    }
}
