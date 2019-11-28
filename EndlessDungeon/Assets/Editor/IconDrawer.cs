
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(IconAttribute))]
public class IconDrawer : PropertyDrawer
{

    public static readonly int pickerWidth = 18;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        IconAttribute iconAttribute = attribute as IconAttribute;
        SerializedProperty textureProperty = property.objectReferenceValue == null ? null : new SerializedObject(property.objectReferenceValue).FindProperty(iconAttribute.texturePropertyName);

        Texture2D texture = textureProperty?.objectReferenceValue as Texture2D;

        int width = iconAttribute.width + pickerWidth;
        int height = iconAttribute.height;


        /*
         
        GUILayout.BeginArea(new Rect(position.x, position.y, width, height));

        EditorGUILayout.ObjectField(property, new GUIContent(GUIContent.none), GUILayout.Width(width), GUILayout.Height(height));

        if (texture)
        {
            //EditorGUI.DrawTextureTransparent(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, texture.width / 2, texture.height / 2), texture);
            //position = GUILayoutUtility.GetLastRect();
            //GUILayout.Box(texture);
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.x += (iconAttribute.width - texture.width) / 2;
            rect.y += (iconAttribute.height - texture.height) / 2;
            rect.width = texture.width;
            rect.height = texture.height;

            GUI.DrawTexture(rect, texture);

        }
        GUILayout.EndArea();
        */


        position.width = width;
        
        //position.height = height;
        EditorGUI.PropertyField(position, property, new GUIContent(GUIContent.none));

        if (texture)
        {
            //EditorGUI.DrawTextureTransparent(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, texture.width / 2, texture.height / 2), texture);
            //position = GUILayoutUtility.GetLastRect();
            //GUILayout.Box(texture);

            Rect rect = position;
            rect.x += (iconAttribute.width - texture.width / 2) / 2;
            rect.y += (iconAttribute.height - texture.height / 2) / 2;
            rect.width = texture.width / 2;
            rect.height = texture.height / 2;

            GUI.DrawTexture(rect, texture);

        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (attribute as IconAttribute).height;
    }

}
