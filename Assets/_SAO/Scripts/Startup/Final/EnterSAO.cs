using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnterSAO : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] private Camera mainCamera;

    private Color backgroundColor = new Color(0.5f, 0.5f, 0.5f);
    private float recolorDuration = 0.5f;

    [Header("Welcome Text")]
    [SerializeField] private Transform welcomeText;
    [SerializeField] private CanvasGroup welcomeTextCanvasGroup;
    private Vector3 textEndPos = new Vector3(0, 0, -1);
    private float fadeDuration = 0.5f;
    private float moveDelay = 1;
    private float moveDuration = 3;

    [Header("Particles")]
    [SerializeField] private ParticleSystem enterParticles;
    private float particleDelay = 2;

    [Header("Bloom Effect")]
    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private VolumeProfile postProcessingProfile;
    [SerializeField] private Image bloomElement;
    [SerializeField] private Material groundMat;
    private float bloomDelay = 2f;
    private float bloomingDuration = 3;

    internal void StartSequence()
    {
        welcomeText.gameObject.SetActive(true);
        welcomeTextCanvasGroup.alpha = 0;

        FadeBackground(backgroundColor);
        StartCoroutine(StartWelcomeAnimation());
        StartCoroutine(StartParticles());
        StartCoroutine(StartBloomEffect());
    }

    private void FadeBackground(Color newColor, float delay = 0)
    {
        mainCamera.DOColor(newColor, recolorDuration).SetDelay(delay);

        Color mycolor = RenderSettings.fogColor;
        DOTween.To(() => mycolor, x => mycolor = x, newColor, recolorDuration).SetDelay(delay).OnUpdate(()=> 
        {
            RenderSettings.fogColor = mycolor;
        });
    }

    private IEnumerator StartWelcomeAnimation()
    {
        yield return new WaitForSeconds(recolorDuration + 0.5f);
        welcomeTextCanvasGroup.DOFade(1, fadeDuration).OnComplete(() => 
        {
            welcomeText.DOMove(textEndPos, moveDuration).SetDelay(moveDelay);
            welcomeTextCanvasGroup.DOFade(0, fadeDuration).SetDelay(moveDuration / 2);
        });
    }

    private IEnumerator StartParticles()
    {
        yield return new WaitForSeconds(particleDelay);
        enterParticles.gameObject.SetActive(true);
    }

    private IEnumerator StartBloomEffect()
    {
        yield return new WaitForSeconds(bloomDelay);
        postProcessingVolume.profile = postProcessingProfile;

        Bloom b;
        postProcessingVolume.profile.TryGet(out b);
       
        float intentsity = 0;
        DOTween.To(() => intentsity, x => intentsity = x, 10, bloomingDuration / 2).OnUpdate(() =>
        {
            b.intensity.value = intentsity;
        });

        bloomElement.gameObject.SetActive(true);


        // Quick work around because only scaling does not work really well (0 -> 1 looks faster)
        bloomElement.DOFade(1, bloomingDuration);
        bloomElement.transform.DOScale(2, bloomingDuration * 2);
        //groundMat.DOColor(Color.white, bloomingDuration * 2);
        bloomElement.transform.DOLocalMove(new Vector3(0, 0, -2070), bloomingDuration).SetDelay(bloomingDuration * 0.2f);
        FadeBackground(Color.white, bloomingDuration);

        yield return new WaitForSeconds(bloomingDuration * 2);
        SceneLoader.Instance.LoadScene((int)SceneType.Interface, true, Color.white);
    }
}
