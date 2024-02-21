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
        private List<MenuItem> openItemList = new List<MenuItem>();

        [SerializeField] private Canvas interfaceCanvas;
        [SerializeField] private CategorieMenu categorieMenu;
        private bool _isOpen;

        private Coroutine _lerpLastMenuRoutine;
        private float _moveTime = 2f;
        private float _closeSideDelay = 0f;
        private bool _allowInteraction = true;

        private void Awake()
        {
            MenuItem.IsPressed += OnMenuItemPressed;
        }

        private void OnMenuItemPressed(MenuItem newItem, bool isSelected)
        {
            StartCoroutine(UpdateOpenMenu(newItem, isSelected));
        }
        
        private IEnumerator UpdateOpenMenu(MenuItem newItem, bool isSelected)
        {
            _allowInteraction = false;
            Debug.Log("Pressed item: " + newItem.name + " - isSelected: " + isSelected);

            // Check if the item is part of a submenu of one of the already selected menuItems, if not close all so we can open the new one
            for (int i = 0; i < openItemList.Count; i++)
            {
                Debug.Log("New tree check: Checking Item: " + i + " of " + (openItemList.Count - 1) + " | " + openItemList[i].gameObject.name);

                // If it is part of a menu, then the menus after that should be closed and the new menu should be opened
                if (openItemList[i].subMenu == null) { continue; }
                if (openItemList[i].subMenu.items.Contains(newItem))
                {
                    // The item is part of the same submenu of the previous item
                    for (int j = openItemList.Count - 1; j > i; j--)
                    {
                        Debug.Log("New Tree: Closing Item: " + j + " of " + (openItemList.Count - 1) + " | " + openItemList[j].gameObject.name);
                        yield return StartCoroutine(openItemList[j].Deselect());
                        openItemList.RemoveAt(j);

                        if (_lerpLastMenuRoutine != null) StopCoroutine(_lerpLastMenuRoutine);
                        _lerpLastMenuRoutine = StartCoroutine(LerpLastMenuToCenter());
                    }
                    break;
                }
            }

            // Check if we pressed a category button, if there is no category open yet skip it
            if (openItemList.Count > 0 && categorieMenu.items.Contains(newItem))
            {
                Debug.Log("Close current category (" + openItemList[0].gameObject.name + ") and open new category (" + newItem.gameObject.name + ")");
                // If it is not part of a menu then it is part of the categorie menu and the categorie menu should be closed and the new menu should be opened
                for (int i = openItemList.Count - 1; i >= 0; i--)
                {
                    Debug.Log("Closing Item to open new category: " + openItemList[i].gameObject.name);
                    if (openItemList[i] == null) { continue; }
                    yield return StartCoroutine(openItemList[i].Deselect());
                    openItemList.RemoveAt(i);

                    // Lerp the interface each time to the center of the last opened menu after we closed the previous one
                    if (_lerpLastMenuRoutine != null) StopCoroutine(_lerpLastMenuRoutine);
                    _lerpLastMenuRoutine = StartCoroutine(LerpLastMenuToCenter());
                }
            }

            // Check if we want to open or close the item
            if (isSelected)
            {
                // If this item is part of the category menu then move the categories to the new item
                if (categorieMenu.items.Contains(newItem))
                    yield return StartCoroutine(categorieMenu.OnCategoryItemSelected(categorieMenu.items.FindIndex(x => x.Equals(newItem))));

                // Add the item to the openItems en select it
                openItemList.Add(newItem);
                newItem.Select();
            }
            else
            {
                // The item should be closed
                yield return StartCoroutine(newItem.Deselect());
                openItemList.Remove(newItem);
            }

            // Lerp the whole interface to so that the last opened menu is in the center of the screen
            if (_lerpLastMenuRoutine != null) StopCoroutine(_lerpLastMenuRoutine);
            _lerpLastMenuRoutine = StartCoroutine(LerpLastMenuToCenter());
            yield return _lerpLastMenuRoutine;

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

        private IEnumerator LerpLastMenuToCenter()
        {
            // Find the correct position to go to
            Vector2 pos = Vector2.zero;
            for (int i = 0; i < openItemList.Count; i++)
            {
                if (openItemList[i].subMenu != null)
                    pos.x -= openItemList[i].subMenu.transform.localPosition.x;
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
