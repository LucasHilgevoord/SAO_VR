using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PlayerInterface
{
    public class MenuItem : MonoBehaviour
    {
        public System.Action<MenuItem, bool> MenuItemPressed;

        [SerializeField] private Image icon;
        public Text title;

        [SerializeField] private Image box;
        [SerializeField] private Image selectArrow;

        public string titleString;
        public Sprite iconSpriteOn;
        public Sprite iconSpriteOff;

        public CanvasGroup canvasGroup;
        public Collider myCollider;

        internal bool isSelected;
        private Color selectedColor = new Color(0.92f, 0.67f, 0.05f, 0.9f);

        [Header("Actions when triggered")]
        public UnityEvent OnEnableEvents;
        public UnityEvent OnDisableEvents;

        public void Initialize(string title, Sprite iconOn, Sprite iconOff)
        {
            titleString = title;
            iconSpriteOn = iconOn;
            iconSpriteOff = iconOff;
        }

        public void Start()
        {
            if (title != null && titleString != "")
                title.text = titleString;

            icon.sprite = iconSpriteOff;
        }

        /// <summary>
        /// Assigining the visuals to the corresponding objects
        /// </summary>
        public void ToggleVisuals(bool isSelected)
        {
            if (selectArrow != null) selectArrow.color = isSelected ? selectedColor : Color.white;
            if (title != null) title.color = isSelected ? Color.white : Color.black;
            if (box != null) box.color = isSelected ? selectedColor : Color.white;
            icon.sprite = isSelected ? iconSpriteOn : iconSpriteOff;
        }

        internal void ToggleActions()
        {
            if (isSelected)
            {
                OnEnableEvents?.Invoke();
            }
            else
            {
                OnDisableEvents?.Invoke();
            }
        }

        /// <summary>
        /// Method to fire all actions after the item has been clicked
        /// </summary>
        public virtual void ToggleItem()
        {
            isSelected = !isSelected;
            ToggleVisuals(isSelected);

            ToggleActions();
        }

        internal void Interact()
        {
            ToggleItem();

            // TODO: Create a enum/dictionary with all sound names
            AudioManager.Instance.PlayAudio(AudioGroupType.Interface, "interface_button_press");

            MenuItemPressed?.Invoke(this, isSelected);
        }

        private void OnMouseDown()
        {
            Interact();
        }
    }
}
