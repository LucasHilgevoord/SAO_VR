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

[CustomPropertyDrawer(typeof(PlayerWindowSlot))]
public class PlayerWindowSlotDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Indent the content of the property drawer
        position = EditorGUI.IndentedRect(position);

        // Display the property fields for SlotType and SlotSide
        SerializedProperty slotTypeProp = property.FindPropertyRelative("slotType");
        SerializedProperty slotSideProp = property.FindPropertyRelative("slotSide");

        EditorGUI.PropertyField(position, slotTypeProp, new GUIContent("Slot Type"));
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(position, slotSideProp, new GUIContent("Slot Side"));

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Two property fields, one for SlotType and one for SlotSide
        return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
    }
}


[System.Serializable]
public class PlayerWindowSlotEvent : UnityEvent<PlayerWindowSlot[]> { }
