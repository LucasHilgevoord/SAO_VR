using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class LoginManager : MonoBehaviour
{
    public static event Action ClosedWindow;

    [SerializeField] private Transform window;
    [SerializeField] private CanvasGroup myCanvasGroup;

    private Vector3 presetSize = new Vector3(1, 0, 1);
    private float windowToggleDuration = 0.2f;

    private void Awake()
    {
        StartCoroutine(WaitForCurvedUI());
    }

    private void Update()
    {
        // Testing
        if (Input.GetKeyDown(KeyCode.Return))
            ConfirmCredentials();
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

    private void SelectPassword()
    {

    }

    private void SelectUserName()
    {

    }

    private void ConfirmCredentials()
    {
        CloseWindow();
    }
}
