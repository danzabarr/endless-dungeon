using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DistributionCurveAttribute))]
public class DistributionCurveDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DistributionCurveAttribute distributionCurveAttribute = attribute as DistributionCurveAttribute;

        if (property.propertyType == SerializedPropertyType.AnimationCurve)
        {
            Keyframe[] keyframes = new Keyframe[4];

            AnimationCurve curve = property.animationCurveValue;
            Validate(curve);

            float minLimit = curve.keys[0].time;
            float minValue = curve.keys[1].time;
            float maxValue = curve.keys[2].time;
            float maxLimit = curve.keys[3].time;

            position.height = EditorGUIUtility.singleLineHeight;
            Vector4 vector = EditorGUI.Vector4Field(position, label, new Vector4(minLimit, minValue, maxValue, maxLimit));
            minLimit = vector.x;
            minValue = vector.y;
            maxValue = vector.z;
            maxLimit = vector.w;

            position.y += EditorGUIUtility.singleLineHeight; 
            EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, minLimit, maxLimit);
            position.y += EditorGUIUtility.singleLineHeight;
            position.height = 50;
            EditorGUI.CurveField(position, property.animationCurveValue);

            keyframes[0] = new Keyframe(minLimit, 0);
            keyframes[1] = new Keyframe(minValue, 1);
            keyframes[2] = new Keyframe(maxValue, 1);
            keyframes[3] = new Keyframe(maxLimit, 0);

            property.animationCurveValue = new AnimationCurve(keyframes);
        }


        if (property.propertyType == SerializedPropertyType.Vector4)
        {
            Keyframe[] keyframes = new Keyframe[4];

            position.height = EditorGUIUtility.singleLineHeight;
            property.vector4Value = Validate(EditorGUI.Vector4Field(position, label, property.vector4Value));
            float minLimit = property.vector4Value.x;
            float minValue = property.vector4Value.y;
            float maxValue = property.vector4Value.z;
            float maxLimit = property.vector4Value.w;

            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, minLimit, maxLimit);

            property.vector4Value = new Vector4(minLimit, minValue, maxValue, maxLimit);

            position.y += EditorGUIUtility.singleLineHeight;
            position.height = 50;

            keyframes[0] = new Keyframe(minLimit, 0);
            keyframes[1] = new Keyframe(minValue, 1);
            keyframes[2] = new Keyframe(maxValue, 1);
            keyframes[3] = new Keyframe(maxLimit, 0);

            EditorGUI.CurveField(position, new AnimationCurve(keyframes));
        }

    }

    private Vector4 Validate(Vector4 vector)
    {
        vector.y = Mathf.Max(vector.x, vector.y);
        vector.z = Mathf.Max(vector.y, vector.z);
        vector.w = Mathf.Max(vector.z, vector.w);
        return vector;
    }

    private void Validate(AnimationCurve curve)
    {
        Keyframe[] keyframes = new Keyframe[4];

        if (curve.length > 0)
            keyframes[0] = curve.keys[0];
        else
            keyframes[0] = new Keyframe(0, 0);

        if (curve.length > 1)
            keyframes[1] = curve.keys[1];
        else
            keyframes[1] = new Keyframe(1, 1);

        if (curve.length > 2)
            keyframes[2] = curve.keys[2];
        else
            keyframes[2] = new Keyframe(2, 1);

        if (curve.length > 3)
            keyframes[3] = curve.keys[3];
        else
            keyframes[3] = new Keyframe(3, 0);

        curve.keys = keyframes;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 + 50;
    }
}
