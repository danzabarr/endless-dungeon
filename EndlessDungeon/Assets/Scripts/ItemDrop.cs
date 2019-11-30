using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ItemDrop", menuName ="Item/Drop")]
public class ItemDrop : ScriptableObject
{
    [System.Serializable]
    public class ItemDropOption
    {
        [HideInInspector]
        public string name;

        public ItemObject item;

        public float weight = 1;

        public float EvaluateDropChance(float luck, int cLvl, int mLvl)
        {
            return item == null ? weight : item.EvaluateDropChance(weight, luck, cLvl, mLvl);
        }
    }

    [SerializeField]
    private ItemDropOption[] dropOptions;

    private void OnValidate()
    {
        RenameOptions(0, 1, 1);
    }
    public float SumWeight(float luck, int cLvl, int mLvl)
    {
        float sumWeight = 0;
        foreach (ItemDropOption o in dropOptions)
            sumWeight += o.EvaluateDropChance(luck, cLvl, mLvl);
        return sumWeight;
    }
    public void RenameOptions(float luck, int cLvl, int mLvl)
    {
        float sumWeight = SumWeight(luck, cLvl, mLvl);

        foreach (ItemDropOption o in dropOptions)
        {
            float weight = o.EvaluateDropChance(luck, cLvl, mLvl);
            if (o.item == null)
            {
                o.name = (weight / sumWeight).ToString("0.##%") + " No Drop";
            }
            else
            {
                o.name = (weight / sumWeight).ToString("0.##%") + " " + o.item.ToString();
            }
        }
    }
    public ItemDropOption Roll(float luck, int cLvl, int mLvl)
    {
        float sumWeight = SumWeight(luck, cLvl, mLvl);

        float random = Random.value * sumWeight;
        foreach(ItemDropOption option in dropOptions)
        {
            float weight = option.EvaluateDropChance(luck, cLvl, mLvl);

            if (random < weight)
                return option;
            random -= weight;
        }
        return null;
    }
}
