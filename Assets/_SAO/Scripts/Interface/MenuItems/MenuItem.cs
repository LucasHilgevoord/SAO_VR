using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
        private CurvedUICollider _curvedUICollider;
        [SerializeField] private Vector3 _colliderSize;

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

        private void CreateCurvedUICollider()
        {
            GameObject newObject = new GameObject("CurvedUI Collider");
            newObject.transform.parent = transform;
            newObject.transform.localScale = Vector3.one;
            newObject.AddComponent<RectTransform>();
            newObject.AddComponent<CurvedUICollider>().Init(_colliderSize, out _curvedUICollider);
            _curvedUICollider.InteractionDetected += Interact;
        }

        public void Start()
        {
            Button button = GetComponent<Button>();
            if (button != null) {
                button.onClick.AddListener(Interact); 
            }

            if (title != null && titleString != "")
                title.text = titleString;

            icon.sprite = iconSpriteOff;
            //CreateCurvedUICollider();
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

        public void ToggleActions()
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

        public void Interact()
        {
            ToggleItem();
            //AudioManager.Instance.PlayAudio(AudioGroupType.Interface, "interface_button_press");
            MenuItemPressed?.Invoke(this, isSelected);
        }
    }
}
