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
        //[SerializeField] private int maxItems = -1;

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

        public override IEnumerator CloseMenu()
        {
            content.gameObject.SetActive(true);
            yield return StartCoroutine(base.CloseMenu());
            HideLineArrow();
            scrollView.gameObject.SetActive(false);
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
        }

        internal virtual void FillMenuItems() { }

        internal override void OnMenuItemPressed(MenuItem item, bool isSelected) { }

    }
}