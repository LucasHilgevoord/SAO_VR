using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerInterface
{
    public class PlayerEquipmentMenu : Menu
    {
        [Header("Player Equipment")]
        [SerializeField] private GameObject window;
        [SerializeField] private CanvasGroup windowCanvasGroup;
        private float openingDuration = 0.5f;

        [Header("Description Window")]
        [SerializeField] private RectTransform descWindow;
        [SerializeField] private CanvasGroup descCanvasGroup;
        [SerializeField] private RectTransform content;

        [SerializeField] private Image icon;
        [SerializeField] private Text title;
        [SerializeField] private Text desc;

        private EquipmentMenuItem lastOpened;

        private float toggleDuration = 0.3f;
        private bool isEnabled;

        private void Awake()
        {
            EquipmentMenuItem.EquipmentItemPressed += OnDescriptionItemClicked;
        }
        private void OnDestroy()
        {
            EquipmentMenuItem.EquipmentItemPressed -= OnDescriptionItemClicked;
        }

        private void Start()
        {
            descWindow.anchoredPosition = new Vector3(0, descWindow.rect.height, 0);
            descCanvasGroup.alpha = 0;

            window.transform.localScale = Vector3.zero;
            windowCanvasGroup.alpha = 0;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
                OnDescriptionItemClicked(null, !isEnabled);
        }

        public override void OpenMenu()
        {
            base.OpenMenu();
            DOTween.Kill(window.transform, true);
            DOTween.Kill(windowCanvasGroup, true);

            // Setting up the start values
            window.SetActive(true);

            // Opening the window
            window.transform.DOScale(Vector3.one, openingDuration);
            windowCanvasGroup.DOFade(1, openingDuration);
        }

        public override void CloseMenu()
        {
            base.CloseMenu();

            DOTween.Kill(window.transform);
            DOTween.Kill(windowCanvasGroup);

            // Closing the window
            windowCanvasGroup.DOFade(0, openingDuration);
            window.transform.DOScale(Vector3.one, openingDuration).OnComplete(()=>
            {
                window.transform.localScale = Vector3.zero;
                window.SetActive(false);
            });
        }

        private void OnDescriptionItemClicked(EquipmentMenuItem item, bool enable)
        {
            // Don't close when another item is selected which closes the older one
            if (enable == false && item != lastOpened)
                return;
            lastOpened = item;

            // Don't close when another one is enabled
            if (isEnabled != enable)
            {
                isEnabled = enable;
                ToggleDescriptionWindow();
            }

            // Assign new values from the new item data
            if (enable == true)
                SetDescriptionData(item.equipmentData);
        }

        private void SetDescriptionData(EquipmentData data)
        {
            if (data == null)
                return;

            Vector2 contentPos = content.position;
            contentPos.y = 0;
            content.position = contentPos;

            title.text = data.title;
            Debug.Log(icon);
            Debug.Log(data.iconSpriteOff);
            icon.sprite = data.iconSpriteOff;

            desc.text = data.description;

            Vector2 fixedSizeDelta = content.sizeDelta; 
            fixedSizeDelta.y = desc.GetComponent<RectTransform>().sizeDelta.y;
            content.sizeDelta = fixedSizeDelta;
            content.localPosition = Vector3.zero;
        }

        private void ToggleDescriptionWindow()
        {
            DOTween.Kill(descWindow);
            DOTween.Kill(descCanvasGroup);
            if (isEnabled)
            {
                descWindow.DOAnchorPos(Vector3.zero, toggleDuration).SetEase(Ease.InOutSine);
                descCanvasGroup.DOFade(1, toggleDuration * 2);
            } else
            {
                descWindow.DOAnchorPos(new Vector3(0, descWindow.rect.height, 0), toggleDuration).SetEase(Ease.InOutSine);
                descCanvasGroup.DOFade(0, toggleDuration);
            }
        }
    }
}
