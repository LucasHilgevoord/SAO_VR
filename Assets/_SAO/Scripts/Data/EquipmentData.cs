using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "DataObjects/Equipment", order = 1)]
public class EquipmentData : ScriptableObject
{
    public string id;
    public GameObject objectPrefab;

    [Header("Interface")]
    public string title = "";
    public Sprite iconSpriteOn;
    public Sprite iconSpriteOff; // TODO: Make custom GUI to show sprite

    [Header("Description window")]
    [TextArea(15, 20)]
    public string description;
}
