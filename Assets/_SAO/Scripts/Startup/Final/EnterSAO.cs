using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ExternalPropertyAttributes;

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
    private float moveDuration = 3f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem enterParticles;
    private float particleDelay = 2.5f;

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

        mainCamera.DOColor(backgroundColor, recolorDuration);
        StartCoroutine(StartWelcomeAnimation());
        StartCoroutine(StartParticles());
        //StartCoroutine(StartBloomEffect());
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

        float fogDensity = RenderSettings.fogStartDistance;
        DOTween.To(() => fogDensity, x => fogDensity = x, 0, 1f).SetDelay(enterParticles.main.duration * 0.5f).OnUpdate(() =>
        {
            RenderSettings.fogStartDistance = fogDensity;
        }).OnComplete(()=>
        {
            SceneLoader.Instance.LoadScene((int)SceneType.Interface, true, Color.white, 0.75f);
        });

        
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

        yield return new WaitForSeconds(bloomingDuration * 2);
        SceneLoader.Instance.LoadScene((int)SceneType.Interface, true, Color.white);
    }
}
