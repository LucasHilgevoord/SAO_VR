using DG.Tweening;
using PlayerInterface;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DescriptionWindow : InfoItem
{

    [Header("Description Window")]
    [SerializeField] private RectTransform descWindow;
    [SerializeField] private CanvasGroup descCanvasGroup;
    [SerializeField] private RectTransform content;

    [SerializeField] private Image icon;
    [SerializeField] private Text title;
    [SerializeField] private Text desc;

    private float toggleDuration = 0.3f;
    private bool isEnabled;

    private EquipmentMenuItem lastOpened;

    private void Start()
    {
        descWindow.anchoredPosition = new Vector3(0, descWindow.rect.height, 0);
        descCanvasGroup.alpha = 0;
    }

    internal override void OpenWindow()
    {
        DOTween.Kill(descWindow);
        DOTween.Kill(descCanvasGroup);
        descWindow.DOAnchorPos(Vector3.zero, toggleDuration).SetEase(Ease.InOutSine);
        descCanvasGroup.DOFade(1, toggleDuration * 2);
    }

    internal override void CloseWindow()
    {
        DOTween.Kill(descWindow);
        DOTween.Kill(descCanvasGroup);
        descWindow.DOAnchorPos(new Vector3(0, descWindow.rect.height, 0), toggleDuration).SetEase(Ease.InOutSine);
        descCanvasGroup.DOFade(0, toggleDuration);
    }

    private void OnDescriptionItemClicked(EquipmentMenuItem item, bool enable)
    { 
        // Don't close when another item is selected which closes the older one
        if (enable == false && item != lastOpened)
            return;
        
        lastOpened = item;

        // Don't close when another one is enabled
        if (isEnabled != enable)
        {
            isEnabled = enable;
            if (isEnabled) { CloseWindow(); }
            else { OpenWindow(); }
        }

        // Assign new values from the new item data
        if (enable == true)
            SetDescriptionData(item.equipmentData);
    }

    private void SetDescriptionData(EquipmentData data)
    {
        if (data == null)
            return;

        Vector2 contentPos = content.position;
        contentPos.y = 0;
        content.position = contentPos;

        title.text = data.title;
        icon.sprite = data.iconSpriteOff;
        desc.text = data.description;

        Vector2 fixedSizeDelta = content.sizeDelta;
        fixedSizeDelta.y = desc.GetComponent<RectTransform>().sizeDelta.y;
        content.sizeDelta = fixedSizeDelta;
        content.localPosition = Vector3.zero;
    }
}
