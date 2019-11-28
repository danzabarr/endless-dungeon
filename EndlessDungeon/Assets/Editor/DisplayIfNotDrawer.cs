using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisplayIfNotAttribute))]
public class DisplayIfNotDrawer : DisplayIfDrawer
{
    public override bool Invert()
    {
        return true;
    }
}
