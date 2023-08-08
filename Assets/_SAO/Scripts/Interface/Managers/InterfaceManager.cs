using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInterface
{
    public class InterfaceManager : MonoBehaviour
    {
        public static event Action CatogoryMenuOpened;
        public static event Action CatogoryMenuClosed;

        /// <summary>
        /// List of all the opened sub-menu's
        /// </summary>
        private List<MenuItem> openMenus = new List<MenuItem>();

        [SerializeField] private Canvas interfaceCanvas;
        [SerializeField] private CategorieMenu categorieMenu;
        [SerializeField] private MenuItem initialMenuItem;
        private bool _isOpen;

        private Coroutine _lerpInterfaceRoutine;
        private float _moveTime = 0.7f;

        private void Awake()
        {
            MenuItem.IsPressed += OnMenuItemPressed;
        }

        private void OnMenuItemPressed(MenuItem newItem, bool isSelected)
        {
            Debug.Log("OnMenuItemPressed " + newItem.name + " " + isSelected);
            bool isPartOfMenu = false;

            // Check if the item is part of a submenu of one of the already selected menuItems
            for (int i = 0; i < openMenus.Count; i++)
            {
                // If it is part of a menu, then the menus after that should be closed and the new menu should be opened
                if (openMenus[i].subMenu == null) { continue; }
                if (openMenus[i].subMenu.items.Contains(newItem))
                {
                    // The item is part of a submenu which is open, disable all the submenus after that
                    for (int j = openMenus.Count - 1; j > i; j--)
                    {
                        openMenus[j].Deselect();
                        openMenus.RemoveAt(j); // Change Remove to RemoveAt here
                    }
                    isPartOfMenu = true;
                    break;
                }
            }

            if (!isPartOfMenu)
            {
                // If it is not part of a menu then it is part of the categorie menu and the categorie menu should be closed and the new menu should be opened
                for (int i = openMenus.Count - 1; i >= 0; i--)
                {
                    openMenus[i].Deselect();
                    openMenus.RemoveAt(i);
                }
            }

            if (isSelected)
            {
                openMenus.Add(newItem);
                newItem.Select();
            }
            else
            {
                openMenus.Remove(newItem);
                newItem.Deselect();
            }

            if (_lerpInterfaceRoutine != null) StopCoroutine(_lerpInterfaceRoutine);
            _lerpInterfaceRoutine = StartCoroutine(LerpToNewPos());
        }

        private void Update()
        {
//#if UNITY_EDITOR
            if (InputHandler.Instance.wasKeyPressedThisFrame(Key.X))
                ToggleCatogoryMenu(true);

            if (InputHandler.Instance.wasKeyPressedThisFrame(Key.C))
                ToggleCatogoryMenu(false);

            if (InputHandler.Instance.WasControllerButtonPressedThisFrame(ControllerButton.B))
            {
                ToggleCatogoryMenu(true);
            }
            if (InputHandler.Instance.WasControllerButtonPressedThisFrame(ControllerButton.Trigger))
            {
                ToggleCatogoryMenu(true);
            }
            //#endif
        }

        internal void ToggleCatogoryMenu(bool enable)
		{
            if (enable == _isOpen) return;
            _isOpen = enable;
            if (enable)
            {
                CatogoryMenuOpened?.Invoke();
                categorieMenu.OpenMenu();
            }
            else
            {
                CatogoryMenuClosed?.Invoke();
                categorieMenu.CloseMenu();
            }
        }

        private IEnumerator LerpToNewPos()
        {
            // Find the correct position to go to
            Vector2 pos = Vector2.zero;
            for (int i = 0; i < openMenus.Count; i++)
            {
                if (openMenus[i].subMenu != null)
                    pos.x -= openMenus[i].subMenu.transform.localPosition.x;
            }

            float elapsedTime = 0f;
            while (elapsedTime < _moveTime)
            {
                Vector2 nextPos = Vector2.Lerp(transform.localPosition, pos, elapsedTime / _moveTime);
                transform.localPosition = nextPos;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
