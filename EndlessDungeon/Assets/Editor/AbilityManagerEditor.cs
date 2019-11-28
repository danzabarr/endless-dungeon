
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbilityManager)), CanEditMultipleObjects]
public class AbilityManagerEditor : Editor
{
    private SerializedObject obj;
    private AbilityManager abilityManager;

    public void OnEnable()
    {
        obj = serializedObject;
        abilityManager = obj.targetObject as AbilityManager;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(obj.FindProperty("abilities"), true);
        //EditorGUILayout.PropertyField(obj.FindProperty("active"));
        EditorGUILayout.PropertyField(obj.FindProperty("errorMessageDisplay"));
        EditorGUILayout.Space();

        if (abilityManager.GetComponent<Stats>() != null)
        {
            EditorGUILayout.LabelField("(The following properties are driven by this unit's Stats component)");
            GUI.enabled = false;
        }
        DrawPropertiesExcluding(obj, "m_Script", "abilities", "active", "errorMessageDisplay");
        obj.ApplyModifiedProperties();
    }
}