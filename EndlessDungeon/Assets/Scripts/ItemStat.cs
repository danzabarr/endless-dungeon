
using System.Collections.Generic;

[System.Serializable]
public struct ItemStat
{
    public enum Type
    {
        MaxHealth = 0,
        RegenHealth = 1,

        AddArmour = 100,
        EnhancedArmour = 101,
        EnhancedBlock = 150,

        HitRecoverySpeed = 200,
        AttackSpeed = 201,
        EnhancedSpeed = 202,
        WalkSpeed = 300,

        AddDamage = 400,
        EnhancedDamage = 401,
        /*
        AddFireDamage = 402,
        AddColdDamage = 403,
        AddLightningDamage = 404,
        AddPoisonDamage = 405,
        AddShadowDamage = 406,
        AddHolyDamage = 407,
        */

        FireSpellDamage = 500,
        ColdSpellDamage = 501,
        LightningSpellDamage = 502,
        PoisonSpellDamage = 503,
        ShadowSpellDamage = 504,
        HolySpellDamage = 505,

        FireResistance = 600,
        ColdResistance = 601,
        LightningResistance = 602,
        PoisonResistance = 603,
        ShadowResistance = 604,
        HolyResistance = 605,
    }

    public static readonly ItemStat Empty = new ItemStat(Type.MaxHealth, 0);

    public Type type;
    public int value, variable;

    public ItemStat(Type type, int value)
    {
        this.type = type;
        this.value = value;
        variable = 0;
    }

    public ItemStat(Type type, int value, int variable)
    {
        this.type = type;
        this.value = value;
        this.variable = variable;
    }

    public override string ToString() { return Format(type, value, variable); }

    public float GoldValue => StatGoldValue(type, value, variable);

    public static float StatGoldValue(Type stat, int value, int variable)
    {
        switch (stat)
        {
            case Type.MaxHealth:
                return value * 1.0f;
            case Type.RegenHealth:
                return value * 1.0f;
            case Type.AddArmour:
                return value * 1.0f;
            case Type.EnhancedArmour:
                return 0;
            case Type.EnhancedBlock:
                return 0;
            case Type.HitRecoverySpeed:
                return value * 1.0f;
            case Type.AttackSpeed:
                return value * 1.0f;
            case Type.EnhancedSpeed:
                return 0;
            case Type.WalkSpeed:
                return value * 1.0f;
            case Type.AddDamage:
                return 0;
            case Type.EnhancedDamage:
                return 0;
            case Type.FireSpellDamage:
                return value * 1.0f;
            case Type.ColdSpellDamage:
                return value * 1.0f;
            case Type.LightningSpellDamage:
                return value * 1.0f;
            case Type.PoisonSpellDamage:
                return value * 1.0f;
            case Type.ShadowSpellDamage:
                return value * 1.0f;
            case Type.HolySpellDamage:
                return value * 1.0f;
            case Type.FireResistance:
                return value * 1.0f;
            case Type.ColdResistance:
                return value * 1.0f;
            case Type.LightningResistance:
                return value * 1.0f;
            case Type.PoisonResistance:
                return value * 1.0f;
            case Type.ShadowResistance:
                return value * 1.0f;
            case Type.HolyResistance:
                return value * 1.0f;
            default:
                return 0;
        }
    }

    public static string Format(Type stat, int value, int variable)
    {
        switch (stat)
        {
            case Type.MaxHealth:
                return "+" + value + " Maximum Health";
            case Type.RegenHealth:
                return "Regenerates " + value + " Health per Second";
            case Type.HitRecoverySpeed:
                return "Increases Hit Recovery Speed by " + value + "%";
            case Type.AttackSpeed:
                return "Increases Attack Speed by " + value + "%";
            case Type.WalkSpeed:
                return "Increases Walk Speed by " + value + "%";
            case Type.AddDamage:
                if (variable == 0) return "+" + value + " Damage";
                return "+" + value + "-" + (value + variable) + " Damage";
            case Type.EnhancedDamage:
                return "+" + value + "% Enhanced Damage";
            case Type.EnhancedBlock:
                return "+" + value + "% Enhanced Chance to Block";
            case Type.EnhancedSpeed:
                return "+" + value + "% Enhanced Attack Speed";
            case Type.AddArmour:
                return "+" + value + " Armour";
            case Type.EnhancedArmour:
                return "+" + value + "% Enhanced Armour";

            case Type.FireSpellDamage:
                return "+" + value + "% Fire Spell Damage";
            case Type.ColdSpellDamage:
                return "+" + value + "% Cold Spell Damage";
            case Type.LightningSpellDamage:
                return "+" + value + "% Lightning Spell Damage";
            case Type.PoisonSpellDamage:
                return "+" + value + "% Poison Spell Damage";
            case Type.ShadowSpellDamage:
                return "+" + value + "% Shadow Spell Damage";
            case Type.HolySpellDamage:
                return "+" + value + "% Holy Spell Damage";
            case Type.FireResistance:
                return "+" + value + "% Fire Resistance";
            case Type.ColdResistance:
                return "+" + value + "% Cold Resistance";
            case Type.LightningResistance:
                return "+" + value + "% Lightning Resistance";
            case Type.PoisonResistance:
                return "+" + value + "% Poison Resistance";
            case Type.ShadowResistance:
                return "+" + value + "% Shadow Resistance";
            case Type.HolyResistance:
                return "+" + value + "% Holy Resistance";
            default:
                return "";
        }
    }

    public static string Format(Type stat, int valueMin, int valueMax, int variableMin, int variableMax)
    {
        if (valueMin == valueMax && variableMin == variableMax) return Format(stat, valueMin, variableMin);

        switch (stat)
        {
            case Type.MaxHealth:
                return "+[" + valueMin + " - " + valueMax + "] Maximum Health";


            case Type.RegenHealth:
                return "Regenerates [" + valueMin + " - " + valueMax + "] Health per Second";



            case Type.HitRecoverySpeed:
                return "Increases Hit Recovery Speed by [" + valueMin + " - " + valueMax + "]%";
            case Type.AttackSpeed:
                return "Increases Attack Speed by [" + valueMin + " - " + valueMax + "]%";
            case Type.WalkSpeed:
                return "Increases Walk Speed by [" + valueMin + " - " + valueMax + "]%";

            case Type.AddArmour:
                return "+[" + valueMin + " - " + valueMax + "] Armour";

            case Type.EnhancedArmour:
                return "+[" + valueMin + " - " + valueMax + "]% Enhanced Armour";

            case Type.EnhancedBlock:
                return "+[" + valueMin + " - " + valueMax + "]% Enhanced Chance to Block";

            case Type.AddDamage:
                if (variableMin == 0 && variableMax == 0) return "+[" + valueMin + " - " + valueMax + "] Damage";
                return "+[" + valueMin + " - " + valueMax + "]-[" + (valueMin + variableMin) + " - " + (valueMax + variableMax) + "] Damage";

            case Type.EnhancedDamage:
                return "+[" + valueMin + " - " + valueMax + "]% Enhanced Damage";

            case Type.EnhancedSpeed:
                return "+[" + valueMin + " - " + valueMax + "]% Enhanced Attack Speed";

            case Type.FireSpellDamage:
                return "+[" + valueMin + " - " + valueMax + "]% Fire Spell Damage";
            case Type.ColdSpellDamage:
                return "+[" + valueMin + " - " + valueMax + "]% Cold Spell Damage";
            case Type.LightningSpellDamage:
                return "+[" + valueMin + " - " + valueMax + "]% Lightning Spell Damage";
            case Type.PoisonSpellDamage:
                return "+[" + valueMin + " - " + valueMax + "]% Poison Spell Damage";
            case Type.ShadowSpellDamage:
                return "+[" + valueMin + " - " + valueMax + "]% Shadow Spell Damage";
            case Type.HolySpellDamage:
                return "+[" + valueMin + " - " + valueMax + "]% Holy Spell Damage";

            case Type.FireResistance:
                return "+[" + valueMin + " - " + valueMax + "]% Fire Resistance";
            case Type.ColdResistance:
                return "+[" + valueMin + " - " + valueMax + "]% Cold Resistance";
            case Type.LightningResistance:
                return "+[" + valueMin + " - " + valueMax + "]% Lightning Resistance";
            case Type.PoisonResistance:
                return "+[" + valueMin + " - " + valueMax + "]% Poison Resistance";
            case Type.ShadowResistance:
                return "+[" + valueMin + " - " + valueMax + "]% Shadow Resistance";
            case Type.HolyResistance:
                return "+[" + valueMin + " - " + valueMax + "]% Holy Resistance";
            default:
                return "";
        }
    }
    public static ItemStat[] Combine(ItemStat[] stats)
    {
        List<ItemStat> list = new List<ItemStat>();
        void Add(ItemStat stat)
        {
            for (int i = 0; i < stats.Length; i++)
                if (stats[i].type == stat.type)
                {
                    stats[i].value += stat.value;
                    return;
                }
            list.Add(stat);
        }

        foreach (ItemStat stat in stats)
            Add(stat);

        return list.ToArray();
    }
}