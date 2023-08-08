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
        private int _currentCategorieMenu;

        private float _moveTime = 1f;
        private float _interfaceInitHeight;

        private void Awake()
        {
            _interfaceInitHeight = categorieMenu.transform.localPosition.y;
            MenuItem.IsPressed += OnMenuItemPressed;
        }

        private void OnMenuItemPressed(MenuItem newItem, bool isSelected)
        {
            Debug.Log("OnMenuItemPressed " + newItem.name + " " + isSelected);
            bool isPartOfMenu = false;

            // Check if the item is part of a submenu of one of the already selected menuItems
            for (int i = 0; i < openMenus.Count; i++)
            {
                // Check all the submenu's which are part of the item
                for (int j = 0; j < openMenus[i].subMenus.Count; j++) // Change i to j here
                {
                    // If it is part of a menu, then the menus after that should be closed and the new menu should be opened
                    if (openMenus[i].subMenus[j].items.Contains(newItem))
                    {
                        // The item is part of a submenu which is open, disable all the submenus after that
                        for (int k = openMenus.Count - 1; k > i; k--)
                        {
                            openMenus[k].Deselect();
                            openMenus.RemoveAt(k); // Change Remove to RemoveAt here
                        }
                        isPartOfMenu = true;
                        break;
                    }
                }
                if (isPartOfMenu) break;
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

        /// <summary>
        /// Method to add/remove (sub)menu's from the open (sub)menu list.
        /// Used to know the road that the user has taken in the interface.
        /// </summary>
        /// <param name="isOpen">The state of the (sub)menu</param>
        /// <param name="menu">Which (sub)menu has been interacted with</param>
        //private void OnMenuToggled(bool isOpen, Menu menu)
        //{
        //    Debug.Log(menu.transform.name + " OPEN: " + isOpen);

        //    if (isOpen)
        //    {
        //        openMenus.Add(menu);
        //        menu.OpenMenu();
        //        // Lerp categoryMenuRect to new pos
        //        //StartCoroutine(LerpToNewPos(menu.transform.position));
        //    }
        //    else openMenus.Remove(menu);
        //}

        private IEnumerator LerpToNewPos()
        {
            Vector2 pos = Vector2.zero;
            for (int i = 0; i < openMenus.Count; i++)
            {
                pos.x -= openMenus[i].transform.localPosition.x;
                Debug.Log(openMenus[i].name + " | " + pos);
            }
            Debug.Log("Lerp");
            float elapsedTime = 0f;
            while (elapsedTime < _moveTime)
            {
                Vector2 nextPos = Vector2.Lerp(transform.localPosition, pos, elapsedTime / _moveTime);
                transform.localPosition = nextPos;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// Method that handles ... whenever the (sub)menu is fully opened/closed
        /// </summary>
        /// <param name="isOpen">The state of the (sub)menu</param>
        /// <param name="menu">Which (sub)menu has been interacted with</param>
        private void OnMenuToggleComplete(bool isOpen, Menu menu)
        {
            if (isOpen)
            {
                //if (menu == categorieMenu)
                //    initialMenuItem.Interact();
            } else
            {

            }
        }
    }
}
