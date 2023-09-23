using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

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
        private bool _isOpen;

        private Coroutine _lerpInterfaceRoutine;
        private float _moveTime = 2f;

        private float _closeSideDelay = 0f;

        private bool _allowInteraction = true;

        private void Awake()
        {
            MenuItem.IsPressed += OnMenuItemPressed;
        }

        private void OnMenuItemPressed(MenuItem newItem, bool isSelected)
        {
            StartCoroutine(CloseSides(newItem, isSelected));
        }
        
        private IEnumerator CloseSides(MenuItem newItem, bool isSelected)
        {
            _allowInteraction = false;
            bool isPartOfMenu = false;

            // Check if the item is part of a submenu of one of the already selected menuItems
            for (int i = 0; i < openMenus.Count; i++)
            {
                // If it is part of a menu, then the menus after that should be closed and the new menu should be opened
                if (openMenus[i].subMenu == null) { continue; }
                if (openMenus[i].subMenu.items.Contains(newItem))
                {
                    // The item is part of the same submenu of the previous item
                    for (int j = openMenus.Count - 1; j > i; j--)
                    {
                        yield return StartCoroutine(openMenus[j].Deselect());
                        openMenus.RemoveAt(j);

                        if (_lerpInterfaceRoutine != null) StopCoroutine(_lerpInterfaceRoutine);
                        _lerpInterfaceRoutine = StartCoroutine(LerpToNewPos());
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
                    if (openMenus[i] == null) { continue; }
                    Debug.Log("1) Deselect: " + openMenus[i].name);
                    yield return StartCoroutine(openMenus[i].Deselect());
                    openMenus.RemoveAt(i);

                    if (_lerpInterfaceRoutine != null) StopCoroutine(_lerpInterfaceRoutine);
                    _lerpInterfaceRoutine = StartCoroutine(LerpToNewPos());
                }
            }

            if (isSelected)
            {
                openMenus.Add(newItem);
                newItem.Select();
                
                if (categorieMenu.items.Contains(newItem))
                    categorieMenu.OnCategoryItemSelected(categorieMenu.items.FindIndex(x => x.Equals(newItem)));
            }
            else
            {
                Debug.Log("2) Deselect: " + newItem.name);
                yield return StartCoroutine(newItem.Deselect());
                openMenus.Remove(newItem);
            }

            if (_lerpInterfaceRoutine != null) StopCoroutine(_lerpInterfaceRoutine);
            _lerpInterfaceRoutine = StartCoroutine(LerpToNewPos());
            yield return _lerpInterfaceRoutine;

            _allowInteraction = true;
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
