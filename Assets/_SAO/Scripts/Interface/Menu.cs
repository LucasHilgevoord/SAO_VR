using UnityEngine;
using System;
using DG.Tweening;
using System.Collections.Generic;

namespace PlayerInterface
{
    public class Menu : MonoBehaviour
    {
        /// <summary>
        /// Event which contents if a menu will open or close
        /// </summary>
        public static Action<bool, Menu> MenuToggled;

        /// <summary>
        /// Event which contents if a menu has fully opened or closed;
        /// </summary>
        public static Action<bool, Menu> MenuToggleComplete;

        public List<MenuItem> items = new List<MenuItem>();
        internal MenuItem currentSelected;
        internal MenuItem previousSelected;

        private float fadeOutAlpha = 0.2f;
        internal float fadeDuration = 0.2f;

        /// <summary>
        /// Open the menu
        /// </summary>
        public virtual void OpenMenu() { MenuToggled?.Invoke(true, this); }

        /// <summary>
        /// Close the menu
        /// </summary>
        public virtual void CloseMenu() { MenuToggled?.Invoke(false, this); }

        internal virtual void OnMenuItemPressed(MenuItem item, bool isSelected)
        {
            // Case: Item is closed so nothing is selected anymore
            if (item == currentSelected && isSelected == false)
            {
                currentSelected = null;
                EnableFullAlpha();
                return;
            }

            // Case: There is already an item open, close the already opened item
            if (currentSelected != null && isSelected)
                currentSelected.ToggleItem();

            previousSelected = currentSelected;
            currentSelected = item;
            FadeOutInactiveItems();
        }

        /// <summary>
        /// Method to fade out the not active items and show only the active one
        /// </summary>
        internal virtual void FadeOutInactiveItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                DOTween.Kill(items[i].canvasGroup, true);
                if (currentSelected == items[i])
                {
                    // This item is selected
                    if (previousSelected != null) items[i].canvasGroup.alpha = 1f;
                    else items[i].canvasGroup.DOFade(1, fadeDuration);
                }
                else
                {
                    // Fade out the not selected items
                    if (previousSelected != null) previousSelected.canvasGroup.alpha = fadeOutAlpha;
                    else items[i].canvasGroup.DOFade(fadeOutAlpha, fadeDuration);
                }
            }
        }

        internal virtual void EnableFullAlpha()
        {
            for (int i = 0; i < items.Count; i++)
            {
                DOTween.Kill(items[i].canvasGroup);
                items[i].canvasGroup.DOFade(1, fadeDuration);
            }
        }
    }
}