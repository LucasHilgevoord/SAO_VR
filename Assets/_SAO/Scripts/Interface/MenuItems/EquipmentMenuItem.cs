using UnityEngine;
using System;
using DG.Tweening;
using JetBrains.Annotations;

namespace PlayerInterface
{
    public class EquipmentMenuItem : MenuItem
    {
        [Header("Equipment Item")]
        public static Action<DescriptionData, bool> EquipmentItemPressed;
        public EquipmentData equipmentData;

        public ItemOptions options;
        public RectTransform visualsRect;

        public void Initialize(EquipmentData data)
        {
            equipmentData = data;
            titleString = equipmentData.title;
            iconSpriteOn = equipmentData.iconSpriteOn;
            iconSpriteOff = equipmentData.iconSpriteOff;

            if (!equipmentData.removable)
            {
                options.DestroyButton(1);
            }
        }

        [UsedImplicitly]
        public void SendDataEvent()
        {
            Debug.Log("Send Data Event");
            EquipmentItemPressed?.Invoke(equipmentData, IsSelected);
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
