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

        private int minCellsForOffset = 4;
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
            if (items.Count <= minCellsForOffset)
            {
                float itemHeight = items[0].gameObject.GetComponent<RectTransform>().rect.height;
                float itemSpacing = layoutGroup.spacing;

                float offset = scrollView.GetComponent<RectTransform>().rect.height / 2;
                offset -= ((itemHeight / 2) * items.Count) + (itemSpacing * (items.Count - 1));
                layoutGroup.padding.top = (int)offset;

                ScaleLine(itemHeight * items.Count + itemSpacing * (items.Count - 1) + itemHeight);
            }
        }

        internal virtual void FillMenuItems() { }

        internal override void OnMenuItemPressed(MenuItem item, bool isSelected) { }

    }
}