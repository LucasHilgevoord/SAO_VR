using UnityEngine;
using System;
using DG.Tweening;

namespace PlayerInterface
{
    public class EquipmentMenuItem : MenuItem
    {
        [Header("Equipment Item")]
        public static Action<DescriptionData, bool> EquipmentItemPressed;
        public EquipmentData equipmentData;

        public RectTransform optionsRect;

        public void Initialize(string title, Sprite iconOn, Sprite iconOff)
        {
            titleString = title;
            iconSpriteOn = iconOn;
            iconSpriteOff = iconOff;
        }

        public void SendDataEvent()
        {
            EquipmentItemPressed?.Invoke(equipmentData, isSelected);
        }

        /// <summary>
        /// Open the options overlay
        /// Called from the inspector
        /// </summary>
        public void OpenOptions()
        {
            Debug.Log("Open Options " + gameObject.name);
            selectButton.interactable = false;
            optionsRect.gameObject.SetActive(true);
            optionsRect.DOAnchorPosX(0, 0.25f);
        }

        /// <summary>
        /// Close the options overlay
        /// Called from the inspector
        /// </summary>
        public void CloseOptions()
        {
            // Disable button interaction
            optionsRect.DOAnchorPosX(optionsRect.rect.width, 0.2f).OnComplete(() => {
                optionsRect.gameObject.SetActive(false);
                selectButton.interactable = true;
            });
        }
    }
}
