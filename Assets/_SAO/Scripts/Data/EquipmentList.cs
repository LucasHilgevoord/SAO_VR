using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentList", menuName = "DataObjects/EquipmentList", order = 0)]
public class EquipmentList : ScriptableObject
{
    public List<EquipmentData> items;
}
