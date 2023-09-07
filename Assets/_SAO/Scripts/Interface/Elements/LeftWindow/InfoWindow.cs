using PlayerInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InfoWindowMenus
{
    PlayerEquipmentMenu,
    PartyMenu
}

public class InfoWindow : MonoBehaviour
{
    [SerializeField] private PlayerEquipmentMenu _playerEquipmentMenu;

    private Menu _currentMenu;

    public void OpenMenu(InfoWindowMenus menuType)
    {
        Menu menu = null;
        switch (menuType)
        {
            case InfoWindowMenus.PlayerEquipmentMenu:
                menu = _playerEquipmentMenu;
                break;
            case InfoWindowMenus.PartyMenu:
                break;
            default:
                break;
        }

        if (menu != null && menu != _currentMenu)
        {
            if (_currentMenu != null)
                StartCoroutine(_currentMenu.CloseMenu());

            _currentMenu = menu;
            _currentMenu.OpenMenu();
        }
    }

    private void OnCategoryOpened()
    {
        // TODO: Open a specific menu based on the category
    }

    public void CloseMenu()
    {
        if (_currentMenu != null)
            StartCoroutine(_currentMenu.CloseMenu());

        _currentMenu = null;
    }
    
    public void OpenPlayerEquipmentMenu()
    {
        // I sadly have to do this because I can't call a UnityEvent through the inspector
        OpenMenu(InfoWindowMenus.PlayerEquipmentMenu);
    }
}
