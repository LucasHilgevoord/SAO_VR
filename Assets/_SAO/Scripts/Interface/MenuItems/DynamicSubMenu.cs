using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerInterface
{
    public class DynamicSubMenu : SubMenu
    {
        public ScriptableObject objectDataList;
        public GameObject menuItemPrefab;

        /// <summary> Location to create the menuItems. </summary>
        public Transform content;
        public VerticalLayoutGroup layoutGroup;
        
        [SerializeField] private Transform scrollView;
        [SerializeField] private Transform worldUICanvas; // TODO: I don't want to set this manually
        [SerializeField] private Material scrollViewFadeMat;

        private int minCellsForOffset = 6;
        [SerializeField] private int maxItems = -1;

        public void Start() 
        { 
            FillMenuItems();
            SetPadding();

            scrollView.gameObject.SetActive(false);
        }

        public override void OpenMenu()
        {
            ShowLineArrow();
            content.gameObject.SetActive(true);
            scrollView.gameObject.SetActive(true);
            float offsetY = worldUICanvas.InverseTransformPoint(scrollView.TransformPoint(scrollView.position)).y;
            scrollViewFadeMat.SetFloat("_OffsetY", -offsetY);
            base.OpenMenu();
        }

        public override void CloseMenu()
        {
            HideLineArrow();
            content.gameObject.SetActive(true);
            scrollView.gameObject.SetActive(false);
            base.CloseMenu();
        }

        private void SetPadding()
        {
            if (items.Count > minCellsForOffset)
                return;

            int totalOffsetCells = minCellsForOffset - items.Count;

            float totalTopPadding = 0;
            float itemHeight = items[0].gameObject.GetComponent<RectTransform>().rect.height;

            for (int i = 0; i < totalOffsetCells; i++)
            {
                totalTopPadding += itemHeight;
                totalTopPadding += layoutGroup.spacing;
            }

            //totalTopPadding = -totalTopPadding;
            layoutGroup.padding.top = (int)totalTopPadding;
            Debug.Log(totalTopPadding);
        }

        internal virtual void FillMenuItems() { }

        internal override void OnMenuItemPressed(MenuItem item, bool isSelected)
        {
            // Case: Item is closed so nothing is selected anymore
            if (item == currentSelected && isSelected == false)
            {
                currentSelected = null;
                return;
            }

            // Case: There is already an item open, close the already opened item
            if (currentSelected != null && isSelected)
                currentSelected.ToggleItem();

            previousSelected = currentSelected;
            currentSelected = item;
        }

    }
}