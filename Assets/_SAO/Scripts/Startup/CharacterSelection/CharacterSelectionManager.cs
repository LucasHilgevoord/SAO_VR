using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR;

public class CharacterSelectionManager : MonoBehaviour
{
    public event Action CharacterSelected;
    private List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
    private bool _allowInput = true;

    [Header("Character Selection")]
    public CanvasGroup myCanvasGroup;
    private Vector3 presetHeaderScale = new Vector3(0, 1, 1);
    public GameObject headerBox;
    public CanvasGroup headerBoxCanvasGroup;
    private Vector3 presetBodyScale = new Vector3(1, 0, 1);
    public GameObject bodyBox;
    public CanvasGroup bodyBoxCanvasGroup;

    private float scaleDuration = 0.2f;

    private string _username;
    private string _gender;
    [SerializeField] private Color selectColor, unselectColor;
    [SerializeField] private Text _usernameLabel;
    [SerializeField] private Image _yesBg;
    [SerializeField] private Image _noBg;
    [SerializeField] private Image _usernameBg;
    private Image _selectedButton;

    [SerializeField] private CharacterCreation _characterCreation;
    private bool _isCharacterSelection;

    private void Awake()
    {
        StartCoroutine(WaitForCurvedUI());
    }

    private void CheckUserData()
    {
        if (UserDataAvailable())
        {
            OpenCharacterSelection();
        }
        else
            StartCharacterCreation();
    }

    private bool UserDataAvailable()
    {
        _username = PlayerPrefs.GetString("playerName", "Kirito");
        _gender = PlayerPrefs.GetString("playerGender", "(X)");
        return true;
    }

    private void Update()
    {
        if (!_isCharacterSelection) return;

        // Keyboard
        if (InputHandler.Instance.wasKeyPressedThisFrame(Key.RightArrow))
            SetSelectedButton(_noBg);
        if (InputHandler.Instance.wasKeyPressedThisFrame(Key.LeftArrow))
            SetSelectedButton(_yesBg);
        if (InputHandler.Instance.wasKeyPressedThisFrame(Key.Enter))
            OnEnterPress();

        // Controller
        foreach (UnityEngine.XR.InputDevice controller in InputHandler.Instance.GetDevices())
        {
            if (controller.isValid && _allowInput)
            {
                if (controller.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue))
                {
                    if (primary2DAxisValue.x < -0.5f)
                    {
                        SetSelectedButton(_yesBg);
                        StartCoroutine(DelayInput());
                    }
                    else if (primary2DAxisValue.x > 0.5f)
                    {
                        SetSelectedButton(_noBg);
                        StartCoroutine(DelayInput());
                    }
                }

                if (controller.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
                    OnEnterPress();
            }
        }
    }

    private void OnEnterPress()
    {
        if (_selectedButton == _yesBg)
            CloseCharacterSelection(() => { CharacterSelected?.Invoke(); });
        else if (_selectedButton == _noBg)
            CloseCharacterSelection(StartCharacterCreation) ;
    }

    private void SetSelectedButton(Image button)
    {
        if (_selectedButton == button) return;

        if (_selectedButton != null)
        {
            DOTween.Kill(_selectedButton);
            _selectedButton.color = unselectColor;
        }

        _selectedButton = button;
        _selectedButton.DOColor(selectColor, 0.3f)
            .SetEase(Ease.Flash)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private IEnumerator DelayInput()
    {
        _allowInput = false;
        yield return new WaitForSeconds(0.2f);
        _allowInput = true;
    }

    private IEnumerator WaitForCurvedUI()
    {
        myCanvasGroup.alpha = 0;
        yield return new WaitForSeconds(0.1f);
        CheckUserData();
    }

    private void OpenCharacterSelection()
    {
        _isCharacterSelection = true;
        myCanvasGroup.alpha = 1;

        // Assign preset values
        headerBox.transform.localScale = presetHeaderScale;
        bodyBox.transform.localScale = presetBodyScale;
        headerBoxCanvasGroup.alpha = 0;
        bodyBoxCanvasGroup.alpha = 0;

        // Animate window
        headerBox.transform.DOScale(Vector3.one, scaleDuration);
        headerBoxCanvasGroup.DOFade(1, scaleDuration);

        bodyBox.transform.DOScale(Vector3.one, scaleDuration).SetDelay(scaleDuration * 0.5f);
        bodyBoxCanvasGroup.DOFade(1, scaleDuration).SetDelay(scaleDuration * 0.5f);

        _usernameLabel.text = _username + " " + _gender;

        SetSelectedButton(_yesBg);
        _usernameBg.DOColor(selectColor, 0.4f)
            .SetEase(Ease.Flash)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void CloseCharacterSelection(Action callback)
    {
        bodyBox.transform.DOScale(presetBodyScale, scaleDuration);
        bodyBoxCanvasGroup.DOFade(0, scaleDuration);

        headerBox.transform.DOScale(presetHeaderScale, scaleDuration).SetDelay(scaleDuration * 0.5f);
        headerBoxCanvasGroup.DOFade(0, scaleDuration).SetDelay(scaleDuration * 0.5f).OnComplete(() =>
        {
            if (callback != null)
                callback?.Invoke();
        });
    }

    private void StartCharacterCreation()
    {
        _isCharacterSelection = false;
        _characterCreation.StartCharacterCreation();

        _characterCreation.CharacterCreated += OnCharacterCreated;
    }

    private void OnCharacterCreated()
    {
        _characterCreation.CharacterCreated -= OnCharacterCreated;
        CharacterSelected?.Invoke();
    }
}