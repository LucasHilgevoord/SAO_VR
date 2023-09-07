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
        private float spawnDelay = 0.05f;
        private float hideDelay = 0.05f;
        private Coroutine closeMenuCoroutine;

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

            // Disable the arrow of all the items
            foreach (MenuItem i in items)
            {
                i.EnableArrowImage(isSelected);
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

            Debug.Log("start");

            // Hide the items
            for (int i = items.Count - 1; i >= 0; i--)
            {
                //items[i].IsPressed -= OnMenuItemPressed;
                DOTween.Kill(items[i].canvasGroup, true);
                yield return new WaitForSeconds(hideDelay);
                items[i].canvasGroup.DOFade(0, hideDuration);
            }
            yield return new WaitForSeconds(hideDuration);

            Debug.Log("end");
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
    }
}