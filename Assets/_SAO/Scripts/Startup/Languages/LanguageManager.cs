using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class LanguageManager : MonoBehaviour
{
    public static event Action SelectedLanguage;
    public static event Action ClosedLanguages;

    [SerializeField] private CanvasGroup myCanvasGroup;

    [SerializeField] private Transform languageTitleBox;
    [SerializeField] private Transform languages;
    [SerializeField] private Transform selectArrow;
    private List<LanguageButton> languageButtons;

    private int currentLanguageIndex = 0;

    private Vector3 presetSize = new Vector3(1, 0, 1);
    private float scaleDuration = 0.2f;
    private float scaleDelay = 0;
    private float hideDelay = 0.2f;

    private float selectAnimDuration = 0.2f;
    private float unselectedAlpha = 0.7f;
    private int xOffsetUnselected = 50;

    private Color originalColor;
    private Color blinkColor = new Color(0, 0.93f, 1, 0.8f);

    private void Start()
    {
        //StartLanguageSequence();
        StartCoroutine(WaitForCurvedUI());
    }

    private void Update()
    {
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            if (currentLanguageIndex == languageButtons.Count - 1)
                currentLanguageIndex = 0;
            else
                currentLanguageIndex += 1;

            SelectLanguage(languageButtons[currentLanguageIndex]);
        }

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if (currentLanguageIndex == 0)
                currentLanguageIndex = languageButtons.Count - 1;
            else
                currentLanguageIndex -= 1;

            SelectLanguage(languageButtons[currentLanguageIndex]);
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ConfirmLanguage(languageButtons[currentLanguageIndex]);
        }
    }

    private IEnumerator WaitForCurvedUI()
    {
        myCanvasGroup.alpha = 0;
        yield return new WaitForSeconds(0.1f);
        myCanvasGroup.alpha = 1;
        StartLanguageSequence();
    }

    private void GetLanguages()
    {
        languageButtons = new List<LanguageButton>();
        foreach (Transform child in languages)
        {
            LanguageButton l = child.gameObject.GetComponent<LanguageButton>();
            if (l != null) languageButtons.Add(l);
        }
    }

    public void StartLanguageSequence()
    {
        GetLanguages();

        selectArrow.gameObject.SetActive(false);

        // Animate all the visuals
        scaleDelay = 0;
        UpScaleBox(languageTitleBox);
        for (int i = 0; i < languageButtons.Count; i++)
            UpScaleBox(languageButtons[i].box.transform);
    }

    private void UpScaleBox(Transform box)
    {
        DOTween.Kill(box);
        box.transform.localScale = presetSize;
        box.DOScale(Vector3.one, scaleDuration).SetDelay(scaleDelay).OnComplete(()=> 
        {
            if (box == languageButtons[languageButtons.Count - 1].box.transform)
                SelectLanguage(languageButtons[currentLanguageIndex]);
        });
        scaleDelay += scaleDuration;
    }

    private void DownScaleBox(Transform box)
    {
        DOTween.Kill(box);
        box.DOScale(presetSize, scaleDuration).SetDelay(scaleDelay).OnComplete(()=> 
        {
            if (box == languageTitleBox)
                ClosedLanguages?.Invoke();
        });
        scaleDelay += scaleDuration;
    }


    public void SelectLanguage(LanguageButton language)
    {
        if (selectArrow.gameObject.activeInHierarchy == false)
            selectArrow.gameObject.SetActive(true);

        // Set the arrow to the new selected
        Vector2 newPos = selectArrow.localPosition;
        newPos.y = language.transform.localPosition.y;
        selectArrow.localPosition = newPos;

        // TODO: Optimizable, currently everything is getting updated even if they are already in this state.
        // Animate the languages to their next state
        for (int i = 0; i < languageButtons.Count; i++)
        {
            DOTween.Kill(languageButtons[i].box.transform);
            DOTween.Kill(languageButtons[i].canvasGroup);

            if (language != languageButtons[i])
            {
                languageButtons[i].canvasGroup.DOFade(unselectedAlpha, selectAnimDuration);

                Vector2 newBoxPos = Vector2.zero;
                newBoxPos.x = xOffsetUnselected;
                languageButtons[i].box.transform.DOLocalMove(newBoxPos, selectAnimDuration);
            } else
            {
                // Set the selected one differently from the others
                language.canvasGroup.DOFade(1, selectAnimDuration);
                language.box.transform.DOLocalMove(Vector3.zero, selectAnimDuration);
            }
        }
    }

    public void ConfirmLanguage(LanguageButton language)
    {
        originalColor = language.box.color;
        originalColor.a = blinkColor.a;

        Sequence s = DOTween.Sequence().SetLoops(3, LoopType.Restart);
        s.Append(language.box.DOColor(blinkColor, 0.1f));
        s.Append(language.box.DOColor(originalColor, 0.1f));
        s.Play().OnComplete(() => 
        {
            originalColor.a = 1;
            language.box.color = originalColor;

            SetLanguage(language.languageID);
            StartCoroutine(HideLanguages(hideDelay));
        });
    }

    private IEnumerator HideLanguages(float delay)
    {
        yield return new WaitForSeconds(delay);

        selectArrow.gameObject.SetActive(false);

        // Animate all the visuals
        scaleDelay = 0;
        for (int i = languageButtons.Count - 1; i >= 0 ; i--)
            DownScaleBox(languageButtons[i].box.transform);
        DownScaleBox(languageTitleBox);
    }


    public void SetLanguage(string language)
    {
        Debug.Log("Language chosen: " + language);
        SelectedLanguage?.Invoke();
    }
}
