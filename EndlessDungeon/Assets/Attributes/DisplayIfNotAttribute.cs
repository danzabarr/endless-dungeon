using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayIfNotAttribute : DisplayIfAttribute
{
    public DisplayIfNotAttribute(string comparedPropertyName, params object[] comparedValue) : base(comparedPropertyName, comparedValue)
    { }

    public DisplayIfNotAttribute(string comparedPropertyName, object comparedValue, DisablingType disablingType = DisablingType.DontDraw)
        : base(comparedPropertyName, comparedValue, disablingType)
    { }
}
