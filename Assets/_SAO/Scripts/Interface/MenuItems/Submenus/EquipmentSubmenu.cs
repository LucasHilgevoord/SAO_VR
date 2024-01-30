using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInterface
{
    public class EquipmentSubmenu : DynamicSubMenu
    {
        internal override void FillMenuItems() 
        {
            base.FillMenuItems();
            EquipmentList dataList = (EquipmentList)objectDataList;
            AddBarrierItems(2);

            for (int i = 0; i < dataList.items.Count; i++)
            {
                EquipmentData data = dataList.items[i];

                GameObject newItem = Instantiate(menuItemPrefab, content);
                newItem.name = data.title;

                EquipmentMenuItem itemPrefab = newItem.GetComponent<EquipmentMenuItem>();
                itemPrefab.Initialize(data.title, data.iconSpriteOn, data.iconSpriteOff);
                itemPrefab.equipmentData = data;
                items.Add(itemPrefab);
            }

            AddBarrierItems(2);
        }
    }
}