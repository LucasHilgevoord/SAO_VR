using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using System;

public class NameHandler : MonoBehaviour
{
    public static event Action CredentialsAccepted;

    [SerializeField] private Transform window;
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
        if (Input.GetKeyDown(KeyCode.Return))
            ConfirmCredentials();
        if (Input.GetKeyDown(KeyCode.Escape))
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
        window.localScale = presetSize;
        window.DOScale(Vector3.one, windowToggleDuration);
    }

    private void CloseWindow()
    {
        window.DOScale(presetSize, windowToggleDuration);
        myCanvasGroup.DOFade(0, windowToggleDuration).OnComplete(() => { CredentialsAccepted?.Invoke(); });
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
