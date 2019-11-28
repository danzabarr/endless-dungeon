using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayIfRangeAttribute : DisplayIfAttribute
{
    public float min, max;

    public DisplayIfRangeAttribute(float min, float max, string comparedPropertyName, params object[] comparedValue) : base(comparedPropertyName, comparedValue)
    {
        this.min = min;
        this.max = max;
    }

    public DisplayIfRangeAttribute(float min, float max, string comparedPropertyName, object comparedValue, DisablingType disablingType = DisablingType.DontDraw)
        : base(comparedPropertyName, comparedValue, disablingType)
    {
        this.min = min;
        this.max = max;
    }
}
