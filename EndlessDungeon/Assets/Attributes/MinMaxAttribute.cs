using UnityEngine;

public class MinMaxAttribute : PropertyAttribute
{
    public float MinLimit = 0;
    public float MaxLimit = 1;

    public MinMaxAttribute(float min, float max)
    {
        MinLimit = min;
        MaxLimit = max;
    }
}