using System;
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
        private List<Menu> openMenus = new List<Menu>();

        [SerializeField] private CategorieMenu categorieMenu;
        [SerializeField] private MenuItem initialMenuItem;

        private void Awake()
        {
            Menu.MenuToggled += OnMenuToggled;
            Menu.MenuToggleComplete += OnMenuToggleComplete;
        }

        private void Update()
        {
            if (InputHandler.Instance.wasKeyPressedThisFrame(Key.X))
                ToggleCatogoryMenu(true);

            if (InputHandler.Instance.wasKeyPressedThisFrame(Key.C))
                ToggleCatogoryMenu(false);
        }

        internal void ToggleCatogoryMenu(bool enable)
		{
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
        private void OnMenuToggled(bool isOpen, Menu menu)
        {
            if (isOpen) openMenus.Add(menu);
            else openMenus.Remove(menu);
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
                if (menu == categorieMenu)
                    initialMenuItem.Interact();
            } else
            {

            }
        }
    }
}
