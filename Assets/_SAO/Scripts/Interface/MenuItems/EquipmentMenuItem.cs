using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PlayerInterface
{
    public class EquipmentMenuItem : MenuItem
    {
        [Header("Equipment Item")]
        public static Action<EquipmentMenuItem, bool> EquipmentItemPressed;
        public EquipmentData equipmentData;

        public override void ToggleItem()
        {
            base.ToggleItem();
            EquipmentItemPressed?.Invoke(this, isSelected);
        }
    }
}
