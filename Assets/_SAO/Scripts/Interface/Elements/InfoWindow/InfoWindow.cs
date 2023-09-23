using DG.Tweening;
using PlayerInterface;
using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private PlayerEquipmentWindow _playerEquipmentMenu;
    [SerializeField] private DescriptionWindow _descriptionWindow;
    private InfoItem _currentMenu;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject window;
    private float openingDuration = 0.5f;

    [SerializeField] private CanvasGroup headerCanvasGroup;
    [SerializeField] private Text username;

    private void Start()
    {
        username.text = PlayerPrefs.GetString("username", "Kirito");
        window.transform.localScale = Vector3.zero;

        //OpenWindow();
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

    public void OpenItem(InfoWindowMenus menuType)
    {
        InfoItem menu = null;
        switch (menuType)
        {
            case InfoWindowMenus.PlayerEquipmentMenu:
                menu = _playerEquipmentMenu;
                break;
            case InfoWindowMenus.PartyMenu:
                break;
            case InfoWindowMenus.Default:
                menu = null;
                break;
            default:
                break;
        }

        if (menu != _currentMenu)
        {
            if (_currentMenu != null)
                _currentMenu.CloseWindow();

            _currentMenu = menu;
            if (menu != null)
            {
                _currentMenu.gameObject.SetActive(true);
                _currentMenu.OpenWindow();
            }
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
    
    public void OpenPlayerEquipmentMenu()
    {
        Action openMenu = () => { OpenItem(InfoWindowMenus.PlayerEquipmentMenu); };
        if (!isOpen) { OpenWindow(openMenu); } else
        {
            openMenu?.Invoke();
        }
    }

    public void OpenDefaultMenu()
    {
        OpenItem(InfoWindowMenus.Default);
    }
}
