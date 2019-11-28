using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DebugOnlyAttribute))]
class DebugOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) { }
}