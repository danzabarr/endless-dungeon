using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisplayOnlyPerSecondAttribute))]
public class DisplayOnlyPerSecondDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        string valueStr;

        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                valueStr = prop.intValue.ToString() + "/s";
                break;
            case SerializedPropertyType.Float:
                valueStr = prop.floatValue.ToString("0.##") + "/s";
                break;
            default:
                valueStr = "(not supported)";
                break;
        }

        EditorGUI.LabelField(position, label.text, valueStr);
    }
}