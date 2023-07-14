using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using System;
using UnityEngine.InputSystem;

public class NameHandler : MonoBehaviour
{
    public static event Action CredentialsAccepted;

    [SerializeField] private CanvasGroup myCanvasGroup;

    private Vector3 presetSize = new Vector3(1, 0, 1);
    private float windowToggleDuration = 0.2f;

    public InputField inputField; 

    private void Awake()
    {
        StartCoroutine(WaitForCurvedUI());
    }

    private void Update()
    {
        // Testing
        if (InputHandler.Instance.wasKeyPressedThisFrame(Key.Enter))
            ConfirmCredentials();
        if (InputHandler.Instance.wasKeyPressedThisFrame(Key.Escape))
            RejectCredentials();
    }

    private IEnumerator WaitForCurvedUI()
    {
        myCanvasGroup.alpha = 0;
        yield return new WaitForSeconds(0.1f);
        myCanvasGroup.alpha = 1;
        OpenWindow();
    }

    private void OpenWindow()
    {
        transform.localScale = presetSize;
        transform.DOScale(Vector3.one, windowToggleDuration);
    }

    private void CloseWindow()
    {
        transform.DOScale(presetSize, windowToggleDuration);
        myCanvasGroup.DOFade(0, windowToggleDuration).OnComplete(() => 
        {
            CredentialsAccepted?.Invoke();
            transform.gameObject.SetActive(false);
        });
    }

    private void ConfirmCredentials()
    {
        PlayerPrefs.SetString("playerName", inputField.text);
        CloseWindow();
    }

    private void RejectCredentials()
    {
        inputField.text = "";
    }

}
