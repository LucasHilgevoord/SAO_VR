using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerInterface
{
    public class PlayerEquipmentWindow : InfoItem
    {
        [Header("Player Equipment")]
        [SerializeField] private SlotHandler _slotHandler;
        private float openingDuration = 0.5f;

        private void Start()
        {
            windowCanvasGroup.alpha = 0;
        }

        internal override void OpenWindow()
        {
            Debug.Log("ah");
            window.gameObject.SetActive(true);
            windowCanvasGroup.DOFade(1, openingDuration).OnComplete(() => {
                _slotHandler.ShowAllSlots();
            });

            // TODO Wait until the slots are fully initialized
            // TODO Then, set open to true
            // TODO Then, Don't allow selecting slots before fully open

            // Start listening to if a menu is being opened
        }

        private void OnMenuItemOpened()
        {
            // TODO Toggle slots based on which item it is.
        }

        internal override void CloseWindow()
        {
            StartCoroutine(CloseWindowRoutine());
        }

        private IEnumerator CloseWindowRoutine()
        {
            DOTween.Kill(windowCanvasGroup);
            _slotHandler.FadeLine(openingDuration, 0);
            _slotHandler.HideAllSlots();

            yield return new WaitForSeconds(1f);
            windowCanvasGroup.DOFade(0, openingDuration).OnComplete(() => {
                window.gameObject.SetActive(false);
            });
        }


        public void SelectSlot(int typeIndex)
        {
            // Only choosing one slottype is currently available, not sure how to specify the side as well.
            _slotHandler.SelectSlot(typeIndex);
        }
    }
}
