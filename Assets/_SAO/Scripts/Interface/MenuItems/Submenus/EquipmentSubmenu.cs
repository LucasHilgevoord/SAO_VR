using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerInterface
{
    public class EquipmentSubmenu : DynamicSubMenu
    {
        EquipmentList dataList;
        
        internal override void FillMenuItems() 
        {
            base.FillMenuItems();
            dataList = (EquipmentList)objectDataList;
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

                itemPrefab.DestroyItem += OnItemDestroyed;
                Debug.Log("Added " + itemPrefab.name);
            }

            AddBarrierItems(2);
        }

        private void OnItemDestroyed(MenuItem item)
        {
            RemoveItem(item);
        }

        public override void RemoveItem(MenuItem item)
        {
            // Move the scrollview to another item
            int index = items.IndexOf(item);
            if (index != 0)
            {
                ScrollToItem(index - 1);
            } else if (index != items.Count - 1)
            {
                ScrollToItem(index + 1);
            }

            // Remove from list
            base.RemoveItem(item);

            // Remove listener
            item.DestroyItem -= RemoveItem;

            // Animate the visuals to go upwards
            RectTransform visuals = ((EquipmentMenuItem)item).visualsRect;
            visuals.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            visuals.SetParent(item.transform.parent);
            visuals.DOAnchorPosY(200, 0.5f).OnComplete(()=> { Destroy(visuals.gameObject); });

            // Remove the main item object
            Destroy(item.gameObject);

            // TODO: Completely remove out of the inventory
            //dataList.items.Remove(item.equipmentData);
        }
    }
}