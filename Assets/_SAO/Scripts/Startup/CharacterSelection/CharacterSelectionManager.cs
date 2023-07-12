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
    public CanvasGroup myCanvasGroup;

    private Vector3 presetHeaderScale = new Vector3(0, 1, 1);
    public GameObject headerBox;
    public CanvasGroup headerBoxCanvasGroup;

    private Vector3 presetBodyScale = new Vector3(1, 0, 1);
    public GameObject bodyBox;
    public CanvasGroup bodyBoxCanvasGroup;

    private float scaleDuration = 0.2f;

    [Header("Character Creation")]
    public GameObject characterCreation;
    public NameHandler nameHandler;
    public GameObject mainLightSource;
    public Volume postProcessingVolume;
    public VolumeProfile CharacterCreatorProfile;
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
        OverlayEffects.Instance.FadeIn(1f, Color.white, 0, EnableCharacterCreator);
    }

    private void EnableCharacterCreator()
    {
        characterCreation.SetActive(true);

        // Set the visuals of the scene
        previousProfile = postProcessingVolume.profile;
        postProcessingVolume.profile = CharacterCreatorProfile;
        mainLightSource.SetActive(false);

        OverlayEffects.Instance.FadeOut(3f, Color.white, 0.5f, ()=> { StartCoroutine(ShowNameInput()); });
    }

    private IEnumerator ShowNameInput()
    {
        yield return new WaitForSeconds(0.5f);

        nameHandler.gameObject.SetActive(true);
        NameHandler.CredentialsAccepted += OnCredentialsAccepted;
    }

    private void OnCredentialsAccepted()
    {
        NameHandler.CredentialsAccepted -= OnCredentialsAccepted;
        OverlayEffects.Instance.FadeIn(1f, Color.white, 0, DisableCharacterCreator);
    }

    private void DisableCharacterCreator()
    {
        // Set the visuals of the scene
        postProcessingVolume.profile = previousProfile;
        mainLightSource.SetActive(true);
        characterCreation.SetActive(false);

        OverlayEffects.Instance.FadeOut(1f, Color.white, 0.5f, () => { CharacterSelected?.Invoke(); });
    }

    #endregion
}
