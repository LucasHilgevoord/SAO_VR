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
        public Action<MenuItem, bool> IsPressed;
        public UnityEvent OnSelectEvents;
        public UnityEvent OnDeselectEvents;
        public Action<MenuItem> DestroyItem;

        [Header(" References")]
        public Button selectButton;

        [SerializeField] private Text titleLabel;
        [SerializeField] internal string titleString;

        [SerializeField] private Image background;
        [SerializeField] private Image selectArrow;
        private Color selectedColor = new Color(0.92f, 0.67f, 0.05f, 0.9f);

        public CanvasGroup canvasGroup;
        [SerializeField] private Image iconImage;
        [SerializeField] internal Sprite iconSpriteOn;
        [SerializeField] internal Sprite iconSpriteOff;

        [Header(" Links")]
        /// Submenu's that will be opened when this item is selected
        public SubMenu subMenu;
        private bool isSelected;
        internal bool IsSelected { 
            get { return isSelected; } 
            set { 
                isSelected = value; 
                EnableArrowImage(value); 
            }
        }

        public void Start()
        {
            // Set the title label to the correct name if it is not empty
            if (!string.IsNullOrEmpty(titleString))
                titleLabel.text = titleString;

            // Start the icon with the off sprite
            iconImage.sprite = iconSpriteOff;
            
            if (selectArrow != null)
            {
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
            IsSelected = !IsSelected;
            ToggleVisuals(IsSelected);
        }

        public virtual void Select()
        {
            // Make sure the item is selected
            IsSelected = true;

            // Show the right visuals
            ToggleVisuals(isSelected);

            // Start the OnSelectEvents
            OnSelectEvents?.Invoke();

            // Open the submenu if there is one
            if (subMenu != null)
                subMenu.OpenMenu();
        }

        public IEnumerator Deselect()
        {
            Debug.Log("Deselect: " + gameObject.name + " - Submenu " + (subMenu != null));
            // Close the submenu's
            if (subMenu != null)
                yield return StartCoroutine(subMenu.CloseMenu());

            // Make sure the item is deselected
            IsSelected = false;

            // Show the right visuals
            ToggleVisuals(isSelected);

            // Start the OnDeselectEvents
            OnDeselectEvents?.Invoke();
        }

        public void Interact()
        {
            //AudioManager.Instance.PlayAudio(AudioGroupType.Interface, "interface_button_press");
            Debug.Log("Interact: " + gameObject.name);

            // Fire the event that this object has been clicked. The InterfaceManager will decide what to do with it and when.
            IsPressed?.Invoke(this, !IsSelected);
        }

        internal void EnableArrowImage(bool enable)
        {
            // TODO: Idk if this is the right way to do this, because catergory items also execute this while not having an arrow. Maybe a seperate class with an OnSelect Method which can be overwritten
            if (selectArrow != null)
            {
                selectArrow.gameObject.SetActive(enable);
            }
        }

        internal virtual void RemoveItem() {
            Debug.Log("RemoveItem");
            DestroyItem?.Invoke(this);
        }
    }
}
