using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionData : ScriptableObject
{
    public string id;
    public string title;
    public Sprite icon;

    [Header("Description window")]
    [TextArea(15, 20)]
    public string description;
}
