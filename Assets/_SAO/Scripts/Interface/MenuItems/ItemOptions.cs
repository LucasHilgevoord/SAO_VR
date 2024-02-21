using PlayerInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOptions : MonoBehaviour
{
    [SerializeField] private EquipmentMenuItem item;

    public void Close()
    {
        item.Interact();
    }

    public void Equip()
    {

    }

    public void Delete()
    {

    }
}
