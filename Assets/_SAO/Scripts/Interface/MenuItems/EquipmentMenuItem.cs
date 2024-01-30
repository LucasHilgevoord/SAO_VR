using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PlayerInterface
{
    public class EquipmentMenuItem : MenuItem
    {
        [Header("Equipment Item")]
        public static Action<DescriptionData, bool> EquipmentItemPressed;
        public EquipmentData equipmentData;

        public void SendDataEvent()
        {
            EquipmentItemPressed?.Invoke(equipmentData, isSelected);
        }
    }
}
