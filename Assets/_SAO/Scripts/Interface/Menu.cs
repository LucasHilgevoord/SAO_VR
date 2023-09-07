using UnityEngine;
using System;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

namespace PlayerInterface
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private string menuID;
        public string MenuID => menuID;

        public List<MenuItem> items = new List<MenuItem>();
        internal MenuItem currentSelected;
        internal MenuItem previousSelected;

        private float fadeOutAlpha = 0.2f;
        internal float fadeDuration = 0.2f;
        internal float hideDuration = 0.1f;

        /// <summary>
        /// Open the menu
        /// </summary>
        public virtual void OpenMenu() {  }


        /// <summary>
        /// Close the menu
        /// </summary>
        /// <returns>Close duration</returns>
        public virtual IEnumerator CloseMenu() {
            // Close the current selected items
            foreach (MenuItem item in items)
            {
                if (item.isSelected)
                    yield return StartCoroutine(item.Deselect());
            }
        }

        //internal virtual void OnMenuItemPressed(MenuItem item, bool isSelected)
        //{
        //    Debug.Log("AAAAAH");
        //    // Case: Item is closed so nothing is selected anymore
        //    if (item == currentSelected && isSelected == false)
        //    {
        //        currentSelected = null;
        //        EnableFullAlpha();
        //        return;
        //    }

        //    // Case: There is already an item open, close the already opened item
        //    if (currentSelected != null && isSelected)
        //        currentSelected.Interact();

        //    previousSelected = currentSelected;
        //    currentSelected = item;
        //    FadeOutInactiveItems();
        //}

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