using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class OverlayEffects : Singleton<OverlayEffects>
{
    private enum OverlayType
    {
        SphereOverlay,
        CanvasOverlay,
        PlaneOverlay
    }
    [SerializeField] private OverlayType overlayType;
    private Camera _mainCamera;

    public static event Action fadeInComplete;
    public static event Action fadeOutComplete;

    [SerializeField] private Canvas _overlayCanvas;
    [SerializeField] private Image _overlay;
    
    [SerializeField] private GameObject _sphereOverlay;
    [SerializeField] private GameObject _planeOverlay;
    [SerializeField] private float forwardOffset = 0.1f;

    private bool _isFading;
    private object _fadeObject;

    internal override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        switch (overlayType)
        {
            case OverlayType.SphereOverlay:
                //_sphereOverlay.gameObject.SetActive(true);
                _fadeObject = _sphereOverlay;
                break;
            case OverlayType.PlaneOverlay:
                //_planeOverlay.gameObject.SetActive(true);
                _fadeObject = _planeOverlay;
                break;
            default:
            case OverlayType.CanvasOverlay:
                _overlayCanvas.gameObject.SetActive(true);
                _fadeObject = _overlay;
                break;
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_mainCamera != null && _isFading)
        {
            if (overlayType == OverlayType.SphereOverlay)
            {
            _sphereOverlay.transform.position = _mainCamera.transform.position;

            } else if (overlayType == OverlayType.PlaneOverlay)
            {
                _planeOverlay.transform.position = _mainCamera.transform.position + _mainCamera.transform.forward * forwardOffset;
                Vector3 directionToCamera = _mainCamera.transform.position - _planeOverlay.transform.position;
                _planeOverlay.transform.up = directionToCamera;
            }
        }
    }

    public void FadeInOut(float duration, Color color, float delay = 0, Action callback = null)
    {
        FadeIn(duration, color, delay, () =>
        {
            callback?.Invoke();
            FadeOut(duration, color, 1f, null);
        });
    }

    public void FadeIn(float duration, Color color, float delay = 0, Action callback = null)
    {
        _isFading = true;
        if (_fadeObject is Image overlayImage)
        {
            FadeInImage(overlayImage, duration, color, delay, callback);
        }
        else if (_fadeObject is GameObject overlayObject)
        {
            FadeInMaterial(overlayObject, duration, color, delay, callback);
        }
    }
    public void FadeInImage(Image overlay, float duration, Color color, float delay = 0, Action callback = null)
    {
        overlay.gameObject.SetActive(true);
        DOTween.Kill(overlay);

        Color myColor = color;
        myColor.a = 0;

        overlay.color = myColor;
        overlay.DOFade(1, duration).SetDelay(delay).SetEase(Ease.Linear)
            .OnComplete(()=> 
            { 
                fadeInComplete?.Invoke(); 
                callback?.Invoke();
            });
    }

    private void FadeInMaterial(GameObject obj, float duration, Color color, float delay = 0, Action callback = null)
    {
        obj.gameObject.SetActive(true);
        DOTween.Kill(obj);

        Color myColor = color;
        myColor.a = 0;

        Material material = obj.GetComponent<Renderer>().material;
        material.color = myColor;
        material.DOFade(1, duration).SetDelay(delay).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                fadeInComplete?.Invoke();
                callback?.Invoke();
            });
    }

    public void FadeOut(float duration, Color color, float delay = 0, Action callback = null)
    {
        if (_fadeObject is Image overlayImage)
        {
            FadeOutImage(overlayImage, duration, color, delay, callback);
        }
        else if (_fadeObject is GameObject overlayObject)
        {
            FadeOutMaterial(overlayObject, duration, color, delay, callback);
        }
    }

    public void FadeOutImage(Image overlay, float duration, Color color, float delay = 0, Action callback = null)
    {
        DOTween.Kill(overlay);
        Color myColor = color;
        myColor.a = 1;

        overlay.color = color;
        overlay.DOFade(0, duration).SetDelay(delay).SetEase(Ease.Linear).OnComplete(() => 
        {
            fadeOutComplete?.Invoke();
            overlay.gameObject.SetActive(false);
            _isFading = false;
            callback?.Invoke();
        });
    }

    private void FadeOutMaterial(GameObject obj, float duration, Color color, float delay = 0, Action callback = null)
    {
        DOTween.Kill(obj);
        Color myColor = color;
        myColor.a = 1;

        Material material = obj.GetComponent<Renderer>().material;
        material.color = color;
        material.DOFade(0, duration).SetDelay(delay).SetEase(Ease.Linear).OnComplete(() =>
        {
            fadeOutComplete?.Invoke();
            obj.gameObject.SetActive(false);
            _isFading = false;
            callback?.Invoke();
        });
    }
}
