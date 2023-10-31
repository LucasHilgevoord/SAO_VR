using DG.Tweening;
using PlayerInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum InfoWindowMenus
{
    Default,
    PlayerEquipmentMenu,
    PartyMenu
}

public class InfoWindow : MonoBehaviour
{
    private bool isOpen;
    [SerializeField] private DescriptionWindow _descriptionWindow;
    private InfoItem _currentMenu;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject window;
    private float openingDuration = 0.5f;

    [SerializeField] private CanvasGroup headerCanvasGroup;
    [SerializeField] private Text username;

    private void Awake()
    {
        InfoItem.OpenRequest += OpenInfoItem;
    }

    private void Start()
    {
        username.text = PlayerPrefs.GetString("username", "Kirito");
        window.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (InputHandler.Instance.wasKeyPressedThisFrame(UnityEngine.InputSystem.Key.M))
        {
            OpenWindow();
        }

        if (InputHandler.Instance.wasKeyPressedThisFrame(UnityEngine.InputSystem.Key.N))
        {
            CloseWindow();
        }
    }

    public void OpenInfoItem(InfoItem item)
    {
        if (item == _currentMenu) { return; }
        Action openMenu = () => { item.OpenWindow(); };

        if (!isOpen) { OpenWindow(openMenu); }
        else
        {
            // Close the previous one
            if (_currentMenu != null)
                _currentMenu.CloseWindow();

            // Open the new one
            _currentMenu = item;
            if (item != null)
                _currentMenu.OpenWindow();
        }
    }

    public void CloseItem()
    {
        if (_currentMenu != null)
            _currentMenu.CloseWindow();
        _currentMenu = null;
    }

    private void OpenWindow(Action OnOpenComplete = null)
    {
        Debug.Log("open window");
        DOTween.Kill(window.transform, true);
        DOTween.Kill(canvasGroup, true);

        // Setting up the start values
        window.SetActive(true);

        // Opening the window
        canvasGroup.alpha = 1;
        window.transform.localScale = Vector3.zero;
        window.transform.DOScale(Vector3.one, openingDuration);
        
        headerCanvasGroup.alpha = 0;
        headerCanvasGroup.DOFade(1, openingDuration)
            .SetDelay(openingDuration)
            .OnComplete(() => { OnOpenComplete?.Invoke(); });
        isOpen = true;
    }

    public void CloseWindow()
    {
        DOTween.Kill(window.transform);
        DOTween.Kill(canvasGroup);

        // Closing the window
        canvasGroup.DOFade(0, openingDuration).OnComplete(() => { window.SetActive(false); });
    }
}
