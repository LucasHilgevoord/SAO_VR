using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum SlotType
{
    Skill,
    Arm,
    Chest,
    Wrist,
    Hand,
    Leg
}

[Serializable]
public enum SlotSide
{
    None,
    Left,
    Right,
    Top,
    Bottom
}

[Serializable]
public class PlayerWindowSlot
{
    public SlotType slotType;
    public SlotSide slotSide;

    public PlayerWindowSlot(SlotType type, SlotSide side = SlotSide.None)
    {
        slotType = type;
        slotSide = side;
    }
}
