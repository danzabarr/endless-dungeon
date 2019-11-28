using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisplayIfRangeAttribute))]
public class DisplayIfRangeDrawer : DisplayIfDrawer
{
    public override void PropertyField(Rect position, SerializedProperty property)
    {
        DisplayIfRangeAttribute dira = attribute as DisplayIfRangeAttribute;

        property.floatValue = EditorGUI.Slider(position, property.displayName, property.floatValue, dira.min, dira.max);
        

    }
}
