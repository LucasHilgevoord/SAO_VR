using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using System;
using UnityEngine.InputSystem;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class NameHandler : MonoBehaviour
{
    public static event Action CredentialsAccepted;

    [SerializeField] private CanvasGroup myCanvasGroup;

    private Vector3 presetSize = new Vector3(1, 0, 1);
    private float windowToggleDuration = 0.2f;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI _usernameLabel;
    [SerializeField] private NonNativeKeyboard _keyboard;

    [SerializeField] private Button _confirmButton;

    private void Awake()
    {
        StartCoroutine(WaitForCurvedUI());
        _keyboard.OnKeyboardValueKeyPressed += OnKeyboardKeyPressed;
    }

    private void OnDisable()
    {
        _keyboard.OnKeyboardValueKeyPressed -= OnKeyboardKeyPressed;
    }

    private void OnKeyboardKeyPressed(KeyboardValueKey obj)
    {
        Debug.Log(obj.Value);
    }

    private void Update()
    {
        // Testing
        //if (InputHandler.Instance.wasKeyPressedThisFrame(Key.Enter))
        //    ConfirmCredentials();
        //if (InputHandler.Instance.wasKeyPressedThisFrame(Key.Escape))
        //    RejectCredentials();

        if (_usernameLabel.text == "") {
            _confirmButton.interactable = false;
        } else
        {
            _confirmButton.interactable = true;
        }
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
        _keyboard.gameObject.SetActive(true);
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

    public void ConfirmCredentials()
    {
        Debug.Log("Name: " + _usernameLabel.text);
        PlayerPrefs.SetString("playerName", _usernameLabel.text);
        StartCoroutine(Confirm());
    }

    private IEnumerator Confirm()
    {
        _keyboard.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        CloseWindow();
    }

    public void RejectCredentials()
    {
        inputField.text = "";
    }

}
