using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterSelectionManager : MonoBehaviour
{
    public static event Action CharacterSelected;

    [Header("Data check")]
    [SerializeField] private CanvasGroup myCanvasGroup;

    private Vector3 presetHeaderScale = new Vector3(0, 1, 1);
    [SerializeField] private GameObject headerBox;
    [SerializeField] private CanvasGroup headerBoxCanvasGroup;

    private Vector3 presetBodyScale = new Vector3(1, 0, 1);
    [SerializeField] private GameObject bodyBox;
    [SerializeField] private CanvasGroup bodyBoxCanvasGroup;

    private float scaleDuration = 0.2f;

    [Header("Character Creation")]
    [SerializeField] private GameObject characterCreation;
    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private VolumeProfile CharacterCreatorProfile;
    private VolumeProfile previousProfile;

    private void Awake()
    {
        StartCoroutine(WaitForCurvedUI());
    }

    private IEnumerator WaitForCurvedUI()
    {
        myCanvasGroup.alpha = 0;
        yield return new WaitForSeconds(0.1f);
        CheckUserData();
    }

    private void CheckUserData()
    {
        if (UserDataAvailable())
            OpenCharacterSelection();
        else
            StartCharacterCreation();
    }

    private bool UserDataAvailable()
    {
        // TESTING
        return false;


        if (PlayerPrefs.GetString("playerName", "") == "")
        {
            Debug.Log("No player data found!");
            return false;
        }

        return true;
    }

    #region Character Selection
    private void OpenCharacterSelection()
    {
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

    public void OnAcceptButton()
    {
        Action closeCallback = () => { CharacterSelected?.Invoke(); };
        CloseCharacterSelection(closeCallback);
    }

    public void OnDenyButton()
    {
        CloseCharacterSelection(StartCharacterCreation);
    }
    #endregion

    #region Character Creation
    private void StartCharacterCreation()
    {
        OverlayEffects.fadeInComplete += OnFadeInCompleted;
        OverlayEffects.Instance.FadeInOverlay(1f, Color.black);
    }

    private void OnFadeInCompleted()
    {
        OverlayEffects.fadeInComplete -= OnFadeInCompleted;
        characterCreation.SetActive(true);

        // Set the post processing
        previousProfile = postProcessingVolume.profile;
        postProcessingVolume.profile = CharacterCreatorProfile;

        OverlayEffects.Instance.FadeOutOverlay(3f, Color.black, 0.5f);
    }

    private void OnFadeOutCompleted() 
    {
         
    }
    #endregion
}
