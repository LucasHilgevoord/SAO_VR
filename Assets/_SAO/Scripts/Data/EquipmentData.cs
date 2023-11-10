using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "DataObjects/Equipment", order = 1)]
public class EquipmentData : DescriptionData
{
    public GameObject objectPrefab;

    [Header("Interface")]
    public Sprite iconSpriteOn;
    public Sprite iconSpriteOff; // TODO: Make custom GUI to show sprite
}
