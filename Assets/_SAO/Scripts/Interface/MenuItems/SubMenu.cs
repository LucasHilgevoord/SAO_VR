using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInterface
{
    public class SubMenu : Menu
    {
        [SerializeField] private GameObject lineArrow;
        [SerializeField] private CanvasGroup lineArrowCanvas;
        private float spawnDelay = 0.1f;
        private Coroutine disableItemsRoutine;

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

        public override void OpenMenu()
        {
            base.OpenMenu();

            currentSelected = null;
            ShowLineArrow();
            EnableFullAlpha();

            if (disableItemsRoutine != null)
                StopCoroutine(disableItemsRoutine);

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

        public override void CloseMenu()
        {
            base.CloseMenu();

            HideLineArrow();

            // Disable everything from the current item that is open
            if (currentSelected != null)
                currentSelected.ToggleItem();

            // Hide the items
            for (int i = items.Count - 1; i >= 0; i--)
            {
                //items[i].IsPressed -= OnMenuItemPressed;
                DOTween.Kill(items[i].canvasGroup, true);

                items[i].canvasGroup.DOFade(0, fadeDuration / 2).SetDelay(spawnDelay * (items.Count - i) / 2);
            }

            // Disable the items after all the items are hidden
            disableItemsRoutine = StartCoroutine(DisableItems((fadeDuration / 2) + (spawnDelay * (items.Count - 1))));
        }

        /// <summary>
        /// Method to disable all the items at the same time, this way the layoutgroup won't move them
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator DisableItems(float delay)
        {
            yield return new WaitForSeconds(delay);

            for (int i = 0; i < items.Count; i++)
                items[i].gameObject.SetActive(false);

            disableItemsRoutine = null;
        }

        internal override void OnMenuItemPressed(MenuItem item, bool isSelected)
        {
            if (item == currentSelected && isSelected == false)
            {
                // Case: Item is closed so nothing is selected anymore
                ShowLineArrow();

                base.OnMenuItemPressed(item, isSelected);
                return;
            }
            
            if (currentSelected == null)
            {
                // Case: Nothing is selected but an item will open now
                HideLineArrow();
            }

            base.OnMenuItemPressed(item, isSelected);
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
    }
}