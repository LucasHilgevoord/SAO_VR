using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerInterface
{
    public class CategorieMenu : Menu
    {
        [SerializeField] private VerticalLayoutGroup vGroup;

        [SerializeField] private float itemHeight;
        [SerializeField] private float startOffset;

        [SerializeField] private float appearDelay;
        [SerializeField] private float openSpeed;
        [SerializeField] private float closeSpeed;

        private void Start()
        {
            foreach (MenuItem item in items)
                item.gameObject.SetActive(false);
        }

        public override void OpenMenu()
        {
            base.OpenMenu();

            for (int i = items.Count - 1; i >= 0; i--)
            {
                //items[i].IsPressed += OnMenuItemPressed;
                DOTween.Kill(items[i].canvasGroup);
                DOTween.Kill(items[i].transform);

                // Set the items invisible at start
                items[i].gameObject.SetActive(true);
                items[i].canvasGroup.alpha = 0;

                // Set the items on the right starting position
                Vector3 startPos = new Vector3(0, (itemHeight * i) + (vGroup.spacing * i) + startOffset, 0);
                items[i].transform.localPosition = startPos;

                // Decide the delay for appearing
                float delay = appearDelay * (items.Count - i);

                // Start the moving animation
                StartCoroutine(OpenMenuItem(items[i], delay));
            }

            //AudioManager.Instance.PlayAudio(AudioGroupType.Interface, "interface_open");
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

        public override void CloseMenu()
        {
            base.CloseMenu();

            for (int i = 0; i < items.Count; i++)
            {
                //items[i].IsPressed -= OnMenuItemPressed;
                DOTween.Kill(items[i].canvasGroup);
                DOTween.Kill(items[i].transform);

                // Decide the delay for appearing
                float delay = appearDelay * i;

                // Get the starting position
                Vector3 startPos = new Vector3(0, (itemHeight * i) + (vGroup.spacing * i) + startOffset, 0);

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
    }
}
