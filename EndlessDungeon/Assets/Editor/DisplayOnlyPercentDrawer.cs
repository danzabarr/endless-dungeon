﻿using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisplayOnlyPercentAttribute))]
public class DisplayOnlyPercentDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        string valueStr;

        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                valueStr = prop.intValue.ToString() + "%";
                break;
            case SerializedPropertyType.Boolean:
                valueStr = prop.boolValue ? "100%" : "0%";
                break;
            case SerializedPropertyType.Float:
                valueStr = prop.floatValue.ToString("0.##%");
                break;
            default:
                valueStr = "(not supported)";
                break;
        }

        EditorGUI.LabelField(position, label.text, valueStr);
    }
}