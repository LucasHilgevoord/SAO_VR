using DG.Tweening;
using PlayerInterface;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DescriptionWindow : InfoItem
{
    [Header("Description Window")]
    [SerializeField] private RectTransform content;

    [SerializeField] private Image icon;
    [SerializeField] private Text title;
    [SerializeField] private Text desc;

    private float toggleDuration = 0.3f;
    private bool isEnabled;

    private DescriptionData lastOpened;

    private void Start()
    {
        window.anchoredPosition = new Vector3(0, window.rect.height, 0);
        windowCanvasGroup.alpha = 0;

        EquipmentMenuItem.EquipmentItemPressed += OnDescriptionItemClicked;
    }

    internal override void OpenWindow()
    {
        DOTween.Kill(window);
        DOTween.Kill(windowCanvasGroup);
        window.gameObject.SetActive(true);
        window.DOAnchorPos(Vector3.zero, toggleDuration).SetEase(Ease.InOutSine);
        windowCanvasGroup.DOFade(1, toggleDuration * 2);
    }

    internal override void CloseWindow()
    {
        DOTween.Kill(window);
        DOTween.Kill(windowCanvasGroup);
        window.DOAnchorPos(new Vector3(0, window.rect.height, 0), toggleDuration).SetEase(Ease.InOutSine);
        windowCanvasGroup.DOFade(0, toggleDuration).OnComplete(() =>
        {
            window.gameObject.SetActive(false);
        });
    }
    private void OnDescriptionItemClicked(DescriptionData data, bool enable)
    {
        if (enable)
        {
            // Clicking on the same item that is already open should close it
            if (data == lastOpened && isEnabled)
            {
                CloseWindow();
                isEnabled = false;
            }
            else
            {
                // Assign new values and open the window
                SetDescriptionData(data);
                OpenWindow();
                lastOpened = data;
                isEnabled = true;
            }
        }
        else
        {
            // Clicking on the same item that is already closed should do nothing
            if (data == lastOpened && !isEnabled)
            {
                return;
            }

            // Clicking on another item while the current one is open should close it
            if (isEnabled)
            {
                CloseWindow();
                isEnabled = false;
            }
        }
    }

    private void SetDescriptionData(DescriptionData data)
    {
        if (data == null)
            return;

        Vector2 contentPos = content.position;
        contentPos.y = 0;
        content.position = contentPos;

        title.text = data.title;
        icon.sprite = data.icon;
        desc.text = data.description;

        Vector2 fixedSizeDelta = content.sizeDelta;
        fixedSizeDelta.y = desc.GetComponent<RectTransform>().sizeDelta.y;
        content.sizeDelta = fixedSizeDelta;
        content.localPosition = Vector3.zero;
    }
}
