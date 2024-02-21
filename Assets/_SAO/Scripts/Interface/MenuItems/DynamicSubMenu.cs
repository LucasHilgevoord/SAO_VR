using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static UnityEditor.Progress;

namespace PlayerInterface
{
    public class DynamicSubMenu : SubMenu
    {
        public ScriptableObject objectDataList;
        public GameObject menuItemPrefab, itemBarrier;

        /// <summary> Location to create the menuItems. </summary>
        public RectTransform content;
        public VerticalLayoutGroup layoutGroup;
        [SerializeField] private Transform scrollView;

        private int minCellsForOffset = 4;
        //[SerializeField] private int maxItems = -1;

        public void Start()
        {
            FillMenuItems();
            SetPadding();

            ScrollToItem(items.Count / 2, true);
            scrollView.gameObject.SetActive(false);
        }

        public override void OpenMenu()
        {
            ShowLineArrow();
            content.gameObject.SetActive(true);
            scrollView.gameObject.SetActive(true);
            ScrollToItem(items.Count / 2, true);
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
                ScaleLine(itemHeight * items.Count + itemSpacing * (items.Count - 1) + itemHeight);
                
                // Note: Not needed anymore after adding barrier items
                //float offset = scrollView.GetComponent<RectTransform>().rect.height / 2;
                //offset -= ((itemHeight / 2) * items.Count) + (itemSpacing * (items.Count - 1));
                //layoutGroup.padding.top = (int)offset;
            }
        }

        internal void AddBarrierItems(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Instantiate(itemBarrier, content);
            }
        }

        internal virtual void FillMenuItems() { }

        internal override void OnMenuItemPressed(MenuItem item, bool isSelected) {
            if (!items.Contains(item)) { return; }
            ScrollToItem(items.IndexOf(item));
        }

        private void ScrollToItem(int index, bool snap = false)
        {
            float scrollY = index * (menuItemPrefab.GetComponent<RectTransform>().rect.height + layoutGroup.spacing);
            content.DOLocalMoveY(scrollY, 0.2f, snap);
        }
    }
}