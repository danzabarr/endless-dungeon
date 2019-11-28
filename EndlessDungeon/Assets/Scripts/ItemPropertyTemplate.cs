using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="StatTemplate", menuName ="Item/PropertyTemplate")]
public class ItemPropertyTemplate : ScriptableObject
{
    [DisplayOnly]
    public float weightSum;

    public ItemProperty[] stats;

    private static bool Contains(List<ItemStat> list, ItemStat.Type type)
    {
        foreach (ItemStat stat in list)
            if (stat.type == type) return true;
        return false;
    }

    public ItemProperty Roll(List<ItemStat> exclude)
    {
        float sumWeight = 0;
        List<ItemProperty> list = new List<ItemProperty>();

        foreach (ItemProperty var in stats)
        {
            if (var == null)
                continue;
            if (Contains(exclude, var.type))
                continue;
            list.Add(var);
            sumWeight += var.weight;
        }

        float roll = Random.value * sumWeight;
        foreach(ItemProperty var in list)
        {
            if (var == null)
                continue;
            if (roll <= var.weight)
                return var;
            roll -= var.weight;
        }
        return null;
    }

    /*
    [ContextMenu("Add All Stats")]
    public void AddAllStats()
    {
        ClearStats();
        ItemStat.Type[] types = (ItemStat.Type[])System.Enum.GetValues(typeof(ItemStat.Type));

        stats = new ItemProperty[types.Length];
        for (int i = 0; i < types.Length; i++)
            stats[i] = new ItemProperty()
            {
                type = types[i],
                weight = 1
            };
        SumWeights();
        ApplyNames();
    }

    [ContextMenu("Clear Stats")]
    public void ClearStats()
    {
        stats = new ItemProperty[0];
    }
    */

    private void SumWeights()
    {
        weightSum = 0;
        foreach (ItemProperty stat in stats)
            if (stat != null)
                weightSum += stat.weight;
    }

    public void OnValidate()
    {
        SumWeights(); 
    }
}
