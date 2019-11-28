
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EquipmentManager)), CanEditMultipleObjects]
public class EquipmentManagerEditor : Editor
{
    public static readonly int pickerWidth = 18;

    private SerializedObject obj;
    private EquipmentManager equipmentManager;

    private SerializedProperty head;
    private SerializedProperty body;
    private SerializedProperty hands;
    private SerializedProperty feet;
    private SerializedProperty finger;
    private SerializedProperty neck;
    private SerializedProperty mainHand;
    private SerializedProperty offHand;

    public void OnEnable()
    {
        obj = serializedObject;
        equipmentManager = obj.targetObject as EquipmentManager;
        head = obj.FindProperty("head");
        body = obj.FindProperty("body");
        hands = obj.FindProperty("hands");
        feet = obj.FindProperty("feet");
        finger = obj.FindProperty("finger");
        neck = obj.FindProperty("neck");
        mainHand = obj.FindProperty("mainHand");
        offHand = obj.FindProperty("offHand");
    }

    public override void OnInspectorGUI()
    {
        //DrawPropertiesExcluding(obj);

        
        float width = EditorGUIUtility.currentViewWidth;

        EditorGUILayout.BeginVertical(GUILayout.MinHeight(600));
        //EditorGUILayout.PropertyField(head);
        Center(head, 0, 16, 128, 128);
        Center(body, 0, 160, 128, 192);
        Center(feet, 0, 368, 128, 128);
        Center(mainHand, -192, 16 + 128 + 64, 128, 256);
        Center(offHand, 192, 16 + 128 + 64, 128, 256);
        Center(hands, -192, 64, 128, 128);
        Center(neck, 160, 48, 64, 64);
        Center(finger, 160, 128, 64, 64);

        EditorGUILayout.Space();
        

        // EditorGUI.PropertyField(new Rect(0, 0, 0, 0), body);
        // EditorGUI.PropertyField(new Rect(0, 0, 0, 0), hands);
        // EditorGUI.PropertyField(new Rect(0, 0, 0, 0), feet);
        // EditorGUI.PropertyField(new Rect(0, 0, 0, 0), finger);
        // EditorGUI.PropertyField(new Rect(0, 0, 0, 0), neck);
        // EditorGUI.PropertyField(new Rect(0, 0, 0, 0), mainHand);
        // EditorGUI.PropertyField(new Rect(0, 0, 0, 0), offHand);

        EditorGUILayout.EndVertical();
        obj.ApplyModifiedProperties();
    }

    private void Center(SerializedProperty property, float x, float y, float width, float height)
    {
        EditorGUI.PropertyField(new Rect((EditorGUIUtility.currentViewWidth - width) / 2 - pickerWidth + x, y, width, height), property);
    }

}