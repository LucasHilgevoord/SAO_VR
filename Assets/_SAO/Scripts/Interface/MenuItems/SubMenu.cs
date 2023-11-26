using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace PlayerInterface
{
    public class SubMenu : Menu
    {
        [SerializeField] private GameObject lineArrow;
        [SerializeField] private CanvasGroup lineArrowCanvas;
        [SerializeField] private RectTransform line;

        [SerializeField] private bool centerItemOnSelect = true;
        [SerializeField] private int centerItemIndex = 1;

        private float spawnDelay = 0.05f;
        private float hideDelay = 0.05f;
        private Coroutine closeMenuCoroutine;

        private MenuItem currentSelectedItem, prevSelectedItem;

        private void OnEnable()
        {
            MenuItem.IsPressed += OnMenuItemPressed;
        }

        private void OnDisable()
        {
            MenuItem.IsPressed -= OnMenuItemPressed;
        }

        private void Start()
        {
            // Disable all items
            foreach (MenuItem item in items)
            {
                item.gameObject.SetActive(false);
            }

            // Disable lineArrow
            lineArrow.SetActive(false);
        }


        internal virtual void OnMenuItemPressed(MenuItem item, bool isSelected)
        {
            if (!items.Contains(item)) { return; }

            if (isSelected == false)
            {
                // Show the line left to the submenu
                ShowLineArrow();
            } else
            {
                // Hide the line left to the submenu
                HideLineArrow();
            }

            // TODO: This system does not really work whenever there is no submenu existing..? and somehow only with the profile one.
            // TODO: This stuff ugly, But it should handle that when one of the submenu's has been chosen, that it gets centered
            if (currentSelectedItem != null)
            {
                // Wait until all their submenu's are closed.
                prevSelectedItem = currentSelectedItem;
                prevSelectedItem.OnDeselectEvents.AddListener(WaitUntilCenterSelectItem);
            } else
            {
                CenterSelectedItem(item);
                SetSideArrow(item, isSelected);
            }

            currentSelectedItem = isSelected ? item : null;
        }

        private void WaitUntilCenterSelectItem()
        {
            prevSelectedItem.OnDeselectEvents.RemoveListener(WaitUntilCenterSelectItem);
            if (currentSelectedItem != null)
            {
                CenterSelectedItem(currentSelectedItem);
                SetSideArrow(currentSelectedItem, currentSelectedItem.isSelected);
            } else if (prevSelectedItem != null)
            {
                SetSideArrow(prevSelectedItem, prevSelectedItem.isSelected);
            }
        }

        private void CenterSelectedItem(MenuItem item)
        {
            if (centerItemOnSelect)
            {
                // Find the index of the item that was pressed in the list of items
                int selectedIndex = items.IndexOf(item);
                int centerItemIndex = items.Count / 2;
                List<MenuItem> itemsCopy = new List<MenuItem>();

                // Move the selected item to the center
                for (int i = 0; i < items.Count; i++)
                {
                    int newIndex = (i + selectedIndex) % items.Count;
                    itemsCopy.Add(items[newIndex]);
                }

                items = itemsCopy;

                // Find the new index of the selected item in the modified list
                int newSelectedIndex = items.IndexOf(item);

                // Calculate the offset needed to move the selected item to the center
                int offset = centerItemIndex - newSelectedIndex;

                // Adjust the list so that the selected item becomes the center item
                for (int i = 0; i < items.Count; i++)
                {
                    int newIndex = (i + offset + items.Count) % items.Count;

                    // SetSiblingIndex to rearrange the order of items
                    items[i].transform.SetSiblingIndex(newIndex);
                }
            }
        }

        private void SetSideArrow(MenuItem item, bool isSelected)
        {
            // Set the side arrow for the items
            foreach (MenuItem i in items)
            {
                i.EnableArrowImage(item == i ? isSelected : false);
            }
        }

        public override void OpenMenu()
        {
            base.OpenMenu();

            currentSelected = null;
            ShowLineArrow();
            EnableFullAlpha();

            if (closeMenuCoroutine != null)
                StopCoroutine(closeMenuCoroutine);

            for (int i = 0; i < items.Count; i++)
            {
                //items[i].IsPressed += OnMenuItemPressed;
                DOTween.Kill(items[i].canvasGroup, true);

                // Assigning starting values
                items[i].gameObject.SetActive(true);
                items[i].canvasGroup.alpha = 0;
                //items[i].myCollider.enabled = false;

                // Visualize the items
                items[i].canvasGroup.DOFade(1, fadeDuration).SetDelay(spawnDelay * i);
            }
        }

        public override IEnumerator CloseMenu()
        {
            yield return StartCoroutine(base.CloseMenu());
            closeMenuCoroutine = StartCoroutine(HideItems());
            yield return closeMenuCoroutine;

        }

        private IEnumerator HideItems()
        {
            HideLineArrow();

            // Disable everything from the current item that is open
            if (currentSelected != null)
                currentSelected.ToggleItem();

            // Hide the items
            for (int i = items.Count - 1; i >= 0; i--)
            {
                //items[i].IsPressed -= OnMenuItemPressed;
                DOTween.Kill(items[i].canvasGroup, true);
                yield return new WaitForSeconds(hideDelay);
                items[i].canvasGroup.DOFade(0, hideDuration);
            }
            yield return new WaitForSeconds(hideDuration);
            for (int i = 0; i < items.Count; i++)
                items[i].gameObject.SetActive(false);
        }

        /// <summary>
        /// Method to show the line with arrow
        /// </summary>
        internal void ShowLineArrow()
        {
            DOTween.Kill(lineArrowCanvas);

            lineArrow.SetActive(true);
            lineArrowCanvas.alpha = 0;

            lineArrowCanvas.DOFade(1, fadeDuration * items.Count);
        }

        /// <summary>
        /// Method to hide the line with arrow
        /// </summary>
        internal void HideLineArrow()
        {
            if (lineArrow.activeInHierarchy == false)
                return;

            DOTween.Kill(lineArrowCanvas);
            lineArrowCanvas.DOFade(0, (fadeDuration / 2) * items.Count).OnComplete(() =>
            {
                lineArrow.SetActive(false);
            });
        }

        internal void ScaleLine(float height)
        {
            Vector2 sizeDelta = line.sizeDelta;
            sizeDelta.y = height;
            line.sizeDelta = sizeDelta;
        }
    }
}