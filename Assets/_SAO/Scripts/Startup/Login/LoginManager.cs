using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class LoginManager : MonoBehaviour
{
    public static event Action ClosedWindow;

    [SerializeField] private Transform window;
    [SerializeField] private CanvasGroup myCanvasGroup;

    private Vector3 presetSize = new Vector3(1, 0, 1);
    private float windowToggleDuration = 0.2f;

    [SerializeField] private InputField _usernameInput;
    [SerializeField] private InputField _passwordInput;

    private void Awake()
    {
        StartCoroutine(WaitForCurvedUI());
    }

    private void Start()
    {
        StartCoroutine(LoginSequence());
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
        myCanvasGroup.DOFade(0, windowToggleDuration).OnComplete(()=> { ClosedWindow?.Invoke(); });
    }

    private IEnumerator LoginSequence()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(SelectUserName());
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(SelectPassword());
        yield return StartCoroutine(ConfirmCredentials());
    }

    private IEnumerator SelectUserName()
    {
        string username = "*********";
        yield return StartCoroutine(AnimateInput(_usernameInput, username, 0.05f));
    }

    private IEnumerator SelectPassword()
    {
        string password = "******";
        yield return StartCoroutine(AnimateInput(_passwordInput, password, 0.05f));
    }

    private IEnumerator AnimateInput(InputField field, string input, float characterDelay)
    {
        char[] charArray = input.ToCharArray();
        for (int i = 0; i < charArray.Length; i++)
        {
            yield return new WaitForSeconds(characterDelay);
            field.text += charArray[i];
        }
    }

    private IEnumerator ConfirmCredentials()
    {
        yield return new WaitForSeconds(0.5f);
        CloseWindow();
    }
}
