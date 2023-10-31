using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PlayerInterface
{
    public class MenuItem : MonoBehaviour
    {
        [Header(" Actions when triggered")]
        public static Action<MenuItem, bool> IsPressed;
        public UnityEvent OnSelectEvents;
        public UnityEvent OnDeselectEvents;

        [Header(" References")]
        [SerializeField] private Text titleLabel;
        [SerializeField] private string titleString;

        [SerializeField] private Image background;
        [SerializeField] private Image selectArrow;
        private Color selectedColor = new Color(0.92f, 0.67f, 0.05f, 0.9f);

        public CanvasGroup canvasGroup;
        [SerializeField] private Image iconImage;
        [SerializeField] private Sprite iconSpriteOn;
        [SerializeField] private Sprite iconSpriteOff;
        [SerializeField] private bool useArrow;

        [Header(" Links")]
        /// Submenu's that will be opened when this item is selected
        public SubMenu subMenu;
        internal bool isSelected;
        
        public void Initialize(string title, Sprite iconOn, Sprite iconOff)
        {
            titleString = title;
            iconSpriteOn = iconOn;
            iconSpriteOff = iconOff;
        }

        public void Start()
        {
            // Add the event trigger to the menu item
            Button button = GetComponent<Button>();
            if (button != null) {
                button.onClick.AddListener(Interact); 
            }

            // Set the title label to the correct name if it is not empty
            if (!string.IsNullOrEmpty(titleString))
                titleLabel.text = titleString;

            // Start the icon with the off sprite
            iconImage.sprite = iconSpriteOff;
            
            if (selectArrow != null)
            {
                useArrow = selectArrow.IsActive();
                selectArrow.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Assigining the visuals to the corresponding objects
        /// </summary>
        public void ToggleVisuals(bool isSelected)
        {
            if (selectArrow != null) selectArrow.color = isSelected ? selectedColor : Color.white;
            if (titleLabel != null) titleLabel.color = isSelected ? Color.white : Color.black;
            if (background != null) background.color = isSelected ? selectedColor : Color.white;
            iconImage.sprite = isSelected ? iconSpriteOn : iconSpriteOff;
        }

        /// <summary>
        /// Method to fire all actions after the item has been clicked
        /// </summary>
        public virtual void ToggleItem()
        {
            isSelected = !isSelected;
            ToggleVisuals(isSelected);
        }

        public void Select()
        {
            // Make sure the item is selected
            isSelected = true;

            // Show the right visuals
            ToggleVisuals(isSelected);

            // Start the OnSelectEvents
            Debug.Log("ab");
            OnSelectEvents?.Invoke();

            // Open the submenu
            if (subMenu != null)
                subMenu.OpenMenu();
        }

        public IEnumerator Deselect()
        {
            // Close the submenu's
            if (subMenu != null)
                yield return StartCoroutine(subMenu.CloseMenu());

            // Make sure the item is deselected
            isSelected = false;

            // Show the right visuals
            ToggleVisuals(isSelected);

            // Start the OnDeselectEvents
            OnDeselectEvents?.Invoke();
        }

        public void Interact()
        {
            //AudioManager.Instance.PlayAudio(AudioGroupType.Interface, "interface_button_press");
            isSelected = !isSelected;
            IsPressed?.Invoke(this, isSelected);
        }

        internal void EnableArrowImage(bool enable)
        {
            if (useArrow)
                selectArrow.gameObject.SetActive(enable);
        }
    }
}
