using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ItemProperty", menuName ="Item/Property")]
public class ItemProperty : ScriptableObject
{
    [HideInInspector]
    public string name;
    public string prefix, suffix;

    [Header("Stat")]
    public ItemStat.Type type;
    [MinMax(1, 1000)]
    public Vector2Int valueRange = Vector2Int.one;
    [MinMax(0, 1000)]
    public Vector2Int variableRange;

    [Header("Frequency")]
    [MinMax(0, 10)]
    public float weight = 1;

    public override string ToString()
    {
        return weight + " \t" + ItemStat.Format(type, valueRange.x, valueRange.y, variableRange.x, variableRange.y);
    }

    public ItemStat Roll()
    {
        return new ItemStat(type, Random.Range(valueRange.x, valueRange.y + 1), Random.Range(variableRange.x, variableRange.y + 1));
    }

    public void OnValidate()
    {
        name = ToString();
    }
}
