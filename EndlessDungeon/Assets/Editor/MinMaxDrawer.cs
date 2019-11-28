
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxAttribute))]
public class MinMaxDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // cast the attribute to make life easier
        MinMaxAttribute minMax = attribute as MinMaxAttribute;

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            float minLimit = minMax.MinLimit; // the limit for both min and max, min cant go lower than minLimit and maax cant top maxLimit
            float maxLimit = minMax.MaxLimit;


            property.intValue = Mathf.Clamp(property.intValue, Mathf.CeilToInt(minLimit), Mathf.FloorToInt(maxLimit));
            property.intValue = EditorGUI.IntField(position, property.displayName + " (" + minLimit + "-" + maxLimit + ")", property.intValue);
        }

        else if (property.propertyType == SerializedPropertyType.Float)
        {
            position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            float minLimit = minMax.MinLimit;
            float maxLimit = minMax.MaxLimit;

            property.floatValue = Mathf.Clamp(property.floatValue, minLimit, maxLimit);
            property.floatValue = EditorGUI.FloatField(position, property.displayName + " (" + minLimit + "-" + maxLimit + ")", property.floatValue);
        }

        else if (property.propertyType == SerializedPropertyType.Vector2)
        {
            
            position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // pull out a bunch of helpful min/max values....
            float minValue = property.vector2Value.x; // the currently set minimum and maximum value
            float maxValue = property.vector2Value.y;
            float minLimit = minMax.MinLimit; // the limit for both min and max, min cant go lower than minLimit and maax cant top maxLimit
            float maxLimit = minMax.MaxLimit;

            minValue = Mathf.Clamp(minValue, minLimit, maxLimit);
            maxValue = Mathf.Clamp(maxValue, minLimit, maxLimit);

            maxValue = Mathf.Max(minValue, maxValue);


            property.vector2Value = new Vector2(minValue, maxValue); // shove the values and limits into a vector4 and draw them all at once
            property.vector2Value = EditorGUI.Vector2Field(position, property.displayName + " (" + minLimit + "-" + maxLimit + ")", property.vector2Value);
        }
        else if (property.propertyType == SerializedPropertyType.Vector2Int)
        {
            position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // pull out a bunch of helpful min/max values....
            float minValue = property.vector2IntValue.x; // the currently set minimum and maximum value
            float maxValue = property.vector2IntValue.y;
            int minLimit = Mathf.RoundToInt(minMax.MinLimit); // the limit for both min and max, min cant go lower than minLimit and maax cant top maxLimit
            int maxLimit = Mathf.RoundToInt(minMax.MaxLimit);

            minValue = Mathf.Clamp(minValue, minLimit, maxLimit);
            maxValue = Mathf.Clamp(maxValue, minLimit, maxLimit);

            maxValue = Mathf.Max(minValue, maxValue);


            property.vector2IntValue = new Vector2Int(Mathf.RoundToInt(minValue), Mathf.RoundToInt(maxValue));
            property.vector2IntValue = EditorGUI.Vector2IntField(position, property.displayName + " (" + minLimit + "-" + maxLimit + ")", property.vector2IntValue);
        }
    }
}