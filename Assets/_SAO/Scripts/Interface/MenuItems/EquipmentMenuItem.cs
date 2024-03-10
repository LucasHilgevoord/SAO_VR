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

        public ItemOptions options;
        public RectTransform visualsRect;

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
            options.OpenOptions();
        }

        /// <summary>
        /// Close the options overlay
        /// Called from the inspector
        /// </summary>
        public void CloseOptions()
        {
            // Disable button interaction
            options.CloseOptions();
        }
    }
}
