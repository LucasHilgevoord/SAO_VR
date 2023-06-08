using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusCircle : MonoBehaviour
{
    [SerializeField] private Image[] sprites;
    private float rotateDuration = 3;

    [SerializeField] private Text statusLabel;
    [SerializeField] private Image statusBox;

    private string completeText = "OK";
    private Color completeBoxColor = new Color(0.61f, 0.94f, 0.91f);

    private Color finishedCircleColor = new Color(0.35f, 1, 0.35f);
    private Color finishedCBoxColor = new Color(0.25f, 0.9f, 0.25f);

    private void Start()
    {
        AudioManager.Instance.PlayAudio(AudioGroupType.Startup, "config_circles");
        StartRotate();
    }

    private void StartFade()
    {
        for (int i = 0; i < sprites.Length - 1; i++)
        {
            Color c = sprites[i].color;
            c.a = 0;
            sprites[i].color = c;

            sprites[i].DOFade(1, 0.5f).SetEase(Ease.Linear);
        }
    }

    /// <summary>
    /// Method to start rotating all the circle elements
    /// </summary>
    private void StartRotate()
    {
        Vector3 rot = new Vector3(0, 0, 360);
        for (int i = 1; i < sprites.Length; i++)
        {
            // Flip the way to rotate
            rot = -rot;

            sprites[i].transform.rotation = new Quaternion(0, 0, Random.Range(0, 360), 0);
            sprites[i].transform.DORotate(rot, rotateDuration, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        }
    }

    /// <summary>
    /// Method to stop rotating all the circle elements
    /// </summary>
    private void StopRotate()
    {
        for (int i = 1; i < sprites.Length; i++)
            DOTween.Kill(sprites[i].transform);
    }

    /// <summary>
    /// Method to set the status label to OK and making it green
    /// </summary>
    public void SetStatusComplete()
    {
        StopRotate();
        statusLabel.text = completeText;
        statusLabel.color = Color.white;

        statusBox.color = completeBoxColor;
        statusBox.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
    }

    /// <summary>
    /// Method to turn all the circles green.
    /// </summary>
    public void SetStatusFinished()
    {
        for (int i = 0; i < sprites.Length; i++)
            sprites[i].DOColor(finishedCircleColor, 0.5f);

        statusBox.DOColor(finishedCBoxColor, 0.5f);
    }
}
