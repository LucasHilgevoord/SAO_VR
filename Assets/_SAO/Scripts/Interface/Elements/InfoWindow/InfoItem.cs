using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InfoItem : MonoBehaviour
{
    public static Action<InfoItem> OpenRequest;

    public CanvasGroup windowCanvasGroup;
    public Transform window;

    internal abstract void OpenWindow();
    internal abstract void CloseWindow();
}
