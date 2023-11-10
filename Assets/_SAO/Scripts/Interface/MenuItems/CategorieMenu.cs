using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace PlayerInterface
{
    public class CategorieMenu : Menu
    {
        [SerializeField] private VerticalLayoutGroup vGroup;

        [SerializeField] private float itemHeight;
        [SerializeField] private float initOffset;

        [SerializeField] private float appearDelay;
        [SerializeField] private float openSpeed;
        [SerializeField] private float closeSpeed;
        [SerializeField] private MenuItem initialMenuItem;

        [SerializeField] private RectMask2D moveSelectMask;

        private void Awake()
        {
        }

        private void OnDestroy()
        {
        }

        private void Start()
        {
            moveSelectMask.enabled = false;
            foreach (MenuItem item in items)
                item.gameObject.SetActive(false);
        }

        public override void OpenMenu()
        {
            base.OpenMenu();

            //AudioManager.Instance.PlayAudio(AudioGroupType.Interface, "interface_open");
            for (int i = items.Count - 1; i >= 0; i--)
            {
                DOTween.Kill(items[i].canvasGroup);
                DOTween.Kill(items[i].transform);

                // Set the items invisible at start
                items[i].gameObject.SetActive(true);
                items[i].canvasGroup.alpha = 0;

                // Set the items on the right starting position
                Vector3 startPos = new Vector3(0, (itemHeight * i) + (vGroup.spacing * i) + initOffset, 0);
                items[i].transform.localPosition = startPos;

                // Decide the delay for appearing
                float delay = appearDelay * (items.Count - i);

                // Start the moving animation
                StartCoroutine(OpenMenuItem(items[i], delay));
            }
            StartCoroutine(OpenFirstItem(1.25f));
        }

        /// <summary>
        /// Method to smoothly visialize the opening of the menu
        /// </summary>
        /// <param name="item">Menu item to move</param>
        /// <param name="delay">Delay before moving</param>
        /// <returns></returns>
        internal IEnumerator OpenMenuItem(MenuItem item, float delay)
        {
            yield return new WaitForSeconds(delay);

            //TODO: This can be done easier with: Dotween.SetSpeedBased
            float dur = Vector3.Distance(item.transform.position, Vector3.zero) / openSpeed;
            item.canvasGroup.DOFade(1, dur * 2);
            item.transform.DOLocalMove(Vector3.zero, dur);
        }

        private IEnumerator OpenFirstItem(float delay)
        {
            yield return new WaitForSeconds(delay);
            initialMenuItem.Interact();
        }

        public override IEnumerator CloseMenu()
        {
            yield return StartCoroutine(base.CloseMenu());

            for (int i = 0; i < items.Count; i++)
            {
                //items[i].IsPressed -= OnMenuItemPressed;
                DOTween.Kill(items[i].canvasGroup);
                DOTween.Kill(items[i].transform);

                // Decide the delay for appearing
                float delay = appearDelay * i;

                // Get the starting position
                Vector3 startPos = new Vector3(0, (itemHeight * i) + (vGroup.spacing * i) + initOffset, 0);

                // Start the moving animation
                StartCoroutine(CloseMenuItem(items[i], delay, startPos));
            }
        }

        /// <summary>
        /// Method to smoothly visialize the closing of the menu
        /// </summary>
        /// <param name="item">Menu item to move</param>
        /// <param name="delay">Delay before moving</param>
        /// <returns></returns>
        private IEnumerator CloseMenuItem(MenuItem item, float delay, Vector3 pos)
        {
            yield return new WaitForSeconds(delay);

            //TODO: This can be done easier with: Dotween.SetSpeedBased
            float dur = Vector3.Distance(Vector3.zero, transform.TransformPoint(pos)) / closeSpeed;
            item.canvasGroup.DOFade(0, dur * 0.8f);
            item.transform.DOLocalMove(pos, dur);
        }

        internal IEnumerator OnCategoryItemSelected(int selectedIndex)
        {
            if (selectedIndex == 0) { yield return null; }

            moveSelectMask.enabled = true;

            List<GameObject> itemPlaceholders = new List<GameObject>();
            float duration = 0.5f;

            // Lerp with dotween all the items to the top until the selected item is on top
            float offsetY = selectedIndex * (itemHeight + vGroup.spacing);

            for (int i = 0; i < items.Count; i++)
            {
                RectTransform item = items[i].GetComponent<RectTransform>();
                item.GetComponent<CanvasGroup>().interactable = false;
                GameObject clone = null;

                if (i < selectedIndex)
                {
                    clone = Instantiate(items[i].gameObject, items[i].transform.position, items[i].transform.rotation, items[i].transform.parent);
                    itemPlaceholders.Add(clone);

                    // Place them below the last item
                    clone.transform.localPosition = new Vector3(0, items.Count * -(itemHeight + vGroup.spacing), 0);

                    RectTransform cloneRect = clone.GetComponent<RectTransform>();
                    cloneRect.DOAnchorPosY(cloneRect.anchoredPosition.y + offsetY, duration);
                }

                item.DOAnchorPosY(item.anchoredPosition.y + offsetY, duration).OnComplete(() =>
                {
                    if (clone != null)
                    {
                        item.anchoredPosition = clone.GetComponent<RectTransform>().anchoredPosition;
                        Destroy(clone);
                    }
                });
            }

            yield return new WaitForSeconds(duration);

            //Put the menu items in the right position, the selected item should be on top, the item below the second one is now the second one and so on
            List<MenuItem> itemsCopy = new List<MenuItem>();

            for (int i = 0; i < items.Count; i++)
            {
                int newIndex = (i + selectedIndex) % items.Count;
                itemsCopy.Add(items[newIndex]);
            }

            items = itemsCopy;
            for (int i = 0; i < items.Count; i++)
            {
                items[i].GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                items[i].transform.parent.SetSiblingIndex(i);
                items[i].GetComponent<CanvasGroup>().interactable = true;
            }

            moveSelectMask.enabled = false;
        }
    }
}
