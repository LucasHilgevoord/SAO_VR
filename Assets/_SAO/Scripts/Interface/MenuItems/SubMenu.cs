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
            // Idk why I made this a static event
            foreach (MenuItem item in items)
            {
                item.IsPressed += OnMenuItemPressed;
            }
        }

        private void OnDisable()
        {
            foreach (MenuItem item in items)
            {
                item.IsPressed -= OnMenuItemPressed;
            }

            if (closeMenuCoroutine != null)
            {
                StopCoroutine(closeMenuCoroutine);
                closeMenuCoroutine = null;
            }

            if (lineArrowCanvas != null)
                DOTween.Kill(lineArrowCanvas);

            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item != null && item.canvasGroup != null)
                        DOTween.Kill(item.canvasGroup);
                }
            }
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
            //Debug.Log($"SubMenu: Item {(isSelected ? "selected" : "deselected")}: {item.gameObject.name}");
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

            if (currentSelectedItem != null)
            {
                prevSelectedItem = currentSelectedItem;

                // Wait until all their submenu's are closed.
                if (prevSelectedItem.subMenu != null)
                {
                    prevSelectedItem.OnDeselectEvents.AddListener(WaitUntilCenterSelectItem);
                } else
                {
                    CenterSelectedItem(item);
                }
            } else
            {
                CenterSelectedItem(item);
            }

            currentSelectedItem = isSelected ? item : null;
        }

        private void WaitUntilCenterSelectItem()
        {
            prevSelectedItem.OnDeselectEvents.RemoveListener(WaitUntilCenterSelectItem);
            if (currentSelectedItem != null)
                CenterSelectedItem(currentSelectedItem);
        }

        private void CenterSelectedItem(MenuItem item)
        {
            //Debug.Log("SubMenu: Centering item: " + item.gameObject.name);
            if (!centerItemOnSelect || !items.Contains(item)) return;

            int selectedIndex = items.IndexOf(item);
            int centerIndex = items.Count / 2;

            // Calculate the offset to move the selected item to the center
            int offset = centerIndex - selectedIndex;

            // Rotate the list
            List<MenuItem> reordered = new List<MenuItem>();
            for (int i = 0; i < items.Count; i++)
            {
                int rotatedIndex = (i - offset + items.Count) % items.Count;
                reordered.Add(items[rotatedIndex]);
            }

            items = reordered;

            // Update UI order using SetSiblingIndex
            for (int i = 0; i < items.Count; i++)
            {
                items[i].transform.SetSiblingIndex(i);
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

            if (!isActiveAndEnabled)
                yield break;

            closeMenuCoroutine = StartCoroutine(HideItems());
            yield return closeMenuCoroutine;
            closeMenuCoroutine = null;
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
                if (!isActiveAndEnabled)
                    yield break;

                DOTween.Kill(items[i].canvasGroup, true);
                yield return new WaitForSeconds(hideDelay);
                items[i].canvasGroup.DOFade(0, hideDuration);
            }
            yield return new WaitForSeconds(hideDuration);
            if (!isActiveAndEnabled)
                yield break;

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
                if (lineArrow != null)
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