using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class OverlayEffects : Singleton<OverlayEffects>
{
    public static event Action fadeInComplete;
    public static event Action fadeOutComplete;

    [SerializeField] private Image overlay;

    public void FadeInOverlay(float duration, Color color, float delay = 0, Action callback = null)
    {
        overlay.gameObject.SetActive(true);
        DOTween.Kill(overlay);

        Color myColor = color;
        myColor.a = 0;

        overlay.color = myColor;
        overlay.DOFade(1, duration).SetDelay(delay).SetEase(Ease.Linear)
            .OnComplete(()=> 
            { 
                fadeInComplete.Invoke(); 
                callback?.Invoke(); 
            });
    }

    public void FadeOutOverlay(float duration, Color color, float delay = 0, Action callback = null)
    {
        DOTween.Kill(overlay);
        Color myColor = color;
        myColor.a = 1;

        overlay.color = color;
        overlay.DOFade(0, duration).SetDelay(delay).SetEase(Ease.Linear).OnComplete(() => 
        {
            fadeOutComplete?.Invoke();
            overlay.gameObject.SetActive(false);
            callback?.Invoke();
        });
    }
}
