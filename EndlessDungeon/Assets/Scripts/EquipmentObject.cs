using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentObject : ItemObject
{
    public enum Class
    {
        Unarmed = 0,
        Dagger = 1,
        Sword = 2,
        TwoHandedSword = 3,
        Axe = 4,
        TwoHandedAxe = 5,
        Mace = 6,
        TwoHandedMace = 7,
        Spear = 8,
        TwoHandedSpear = 9,
        Polearm = 10,
        Stave = 11,
        Wand = 12,
        Shield = 13,
        Bow = 14,
        CrossBow = 15,
        Thrown = 16,

        Hood = 100,
        Hat = 101,
        Helmet = 102,

        Tunic = 200,
        Robe = 201,
        LeatherArmour = 203,
        ChainmailArmour = 204,
        PlateArmour = 205,
    }

    public enum Type
    {
        Unequippable,
        Head,
        Body,
        Hands,
        Feet,
        Finger,
        Neck,
        OneHand,
        TwoHand,
        OffHand,
    }

    public enum Slot
    {
        None,
        Head,
        Body,
        Hands,
        Feet,
        Neck,
        Finger,
        MainHand,
        OffHand
    }

    public override void Awake()
    {
        RecalculateBaseAttributes();
        NullUnusedFields();
        base.Awake();
    }
    public virtual void OnValidate()
    {
        UpdateDisplayName();
        RecalculateBaseAttributes();
    }
    public override void UpdateDisplayName()
    {
        if (quality == Quality.Rare || quality == Quality.Normal)
            displayName = (prefix + " " + baseName + " " + suffix).Trim();
        else
            displayName = baseName;

        if (label)
        {
            label.LabelText = displayName;
            label.LabelTextColor = QualityColor(quality);
        }
    }
    
    [SerializeField]
    private Class itemClass;

    [SerializeField]
    private string wornItemName;

    [SerializeField]
    private Mesh heldItemMesh;

    [SerializeField]
    private Vector3 heldItemPosition;

    [SerializeField]
    private Vector3 heldItemEulerRotation;

    [SerializeField]
    private Projectile projectile;

    [SerializeField]
    private Throwable throwable;
    public string WornItemName => wornItemName;
    public Mesh HeldItemMesh => heldItemMesh;
    public Vector3 HeldItemPosition => heldItemPosition;
    public Vector3 HeldItemEulerRotation => heldItemEulerRotation;
    public Class ItemClass => itemClass;
    public Type ItemType => itemClass.Type();
    public string DescriptiveName => ((quality == Quality.Rare || quality == Quality.Legendary) ? (quality + " ") : "") + itemClass.Name();
    public Projectile Projectile => projectile;
    public Throwable Throwable => throwable;

    #region Requirements
    [SerializeField]
    private int requiresLevel;
    [SerializeField]
    private int requiresStrength;
    [SerializeField]
    private int requiresDexterity;
    [SerializeField]
    private int requiresMagic;
    public int RequiresLevel => requiresLevel;
    public int RequiresStrength => requiresStrength;
    public int RequiresDexterity => requiresDexterity;
    public int RequiresMagic => requiresMagic;
    #endregion

    #region Stats
    [System.Serializable]
    public class ItemStats : IReadOnlyList<ItemStat>
    {
        public ItemStats()
        {
            array = new ItemStat[0];
        }

        public ItemStats(List<ItemStat> stats)
        {
            array = stats.ToArray();
        }

        public ItemStat[] array;

        public ItemStat this[int index] => array[index];

        public int Count => array.Length;

        public IEnumerator<ItemStat> GetEnumerator()
        {
            return ((IEnumerable<ItemStat>)array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [SerializeField]
    private ItemPropertyTemplate[] statsTemplate;

    [SerializeField]
    [DisplayOnly]
    protected string prefix, suffix;

    [SerializeField]
    [DisplayOnlyArray]
    protected ItemStats stats;
    public ItemStat Stat(int index) { return stats[index]; }
    public int StatsLength => stats.Count;

    [ContextMenu("Roll Stats")]
    public virtual void RollStats()
    {
        List<ItemProperty> properties = new List<ItemProperty>();
        List<ItemStat> array = new List<ItemStat>();

        prefix = null;
        suffix = null;

        for (int i = 0; i < statsTemplate.Length; i++)
        {
            if (statsTemplate[i] == null)
                continue;
            ItemProperty var = statsTemplate[i].Roll(array);
            if (var == null)
                continue;
            properties.Add(var);
            array.Add(var.Roll());
        }

        int count = array.Count;
        if (count == 1)
        {
            if (Random.value > .5f)
                prefix = properties[0].prefix;
            else
                suffix = properties[0].suffix;
        }
        else if (count == 2)
        {
            prefix = properties[0].prefix;
            suffix = properties[1].suffix;
        }
        else if (count > 2)
        {
            float r1 = Random.value;
            float r2 = Random.value;
            float c = 1f / properties.Count;

            foreach(ItemProperty p in properties)
            {
                if (prefix == null && r1 < c)
                    prefix = p.prefix;
                if (suffix == null && r2 < c)
                    suffix = p.suffix;
                r1 -= c;
                r2 -= c;
            }
        }

        stats = new ItemStats(array);
        UpdateDisplayName();
        RecalculateBaseAttributes();
    }

    [ContextMenu("Clear Stats")]
    public void ClearStats()
    {
        stats = new ItemStats();
        prefix = "";
        suffix = "";
        UpdateDisplayName();
        RecalculateBaseAttributes();
    }

    public ItemStat GetStat(ItemStat.Type type)
    {
        foreach (ItemStat stat in stats)
            if (stat.type == type)
                return stat;
        return ItemStat.Empty;
    }

    #endregion

    #region Base Attributes
    [SerializeField]
    private int baseArmour;
    [SerializeField]
    private int baseBlock;
    [SerializeField]
    private Vector2Int baseDamage;
    [SerializeField]
    private int baseAttacksPer100Seconds;
    [SerializeField]
    private int meleeRange;

    [DisplayOnly]
    private float armour;
    [DisplayOnly]
    private Vector2 damage;
    [DisplayOnly]
    private float attacksPerSecond;
    [DisplayOnly]
    private float block;
    [DisplayOnly]
    private bool indestructible;

    public float Armour => armour;
    public float AttacksPerSecond => attacksPerSecond;
    public Vector2 Damage => damage;
    public float DamagePerSecond => damage.Average() * AttacksPerSecond;
    public float Block => block;
    public float Range => meleeRange;
    public bool Indestructible => indestructible;
    #endregion

    public static readonly float GOLD_VALUE_ARMOUR = 1.0f;
    public static readonly float GOLD_VALUE_BLOCK = 1.0f;
    public static readonly float GOLD_VALUE_DPS = 1.0f;
    
    public virtual void Recalculate()
    {
        UpdateDisplayName();
        RecalculateBaseAttributes();
        NullUnusedFields();
    }
    private void NullUnusedFields()
    {
        if (!itemClass.HasArmour())
        {
            baseArmour = 0;
            armour = 0;
        }
        if (!itemClass.HasBlock())
        {
            baseBlock = 0;
            block = 0;
        }
        if (!itemClass.HasDamage())
        {
            baseDamage = Vector2Int.zero;
            damage = Vector2.zero;
        }

        if (!itemClass.HasSpeed())
        {
            baseAttacksPer100Seconds = 0;
            attacksPerSecond = 0;
        }

        if (!itemClass.IsHeld())
        {
            heldItemMesh = null;
            heldItemPosition = Vector3.zero;
            heldItemEulerRotation = Vector3.zero;
        }

        if (!itemClass.IsVisibleWornObject())
        {
            wornItemName = "";
        }
    }

    private void RecalculateBaseAttributes()
    {
        float addDamage = 0;
        float addVariableDamage = 0;
        float multiplyDamage = 1;

        float multiplySpeed = 1;

        float addArmour = 0;
        float multiplyArmour = 1;

        float multiplyBlock = 1;

        float sellValue = 0;


        if (stats == null) stats = new ItemStats();

        foreach (ItemStat stat in stats)
        {
            sellValue += stat.GoldValue;

            if (stat.type == ItemStat.Type.AddDamage)
            {
                addDamage += stat.value;
                addVariableDamage += stat.variable;
            }
            else if (stat.type == ItemStat.Type.EnhancedDamage)
                multiplyDamage = 1 + stat.value / 100f;

            else if (stat.type == ItemStat.Type.EnhancedSpeed)
                multiplySpeed = 1 + stat.value / 100f;

            else if(stat.type == ItemStat.Type.AddArmour)
                addArmour = stat.value;
            else if(stat.type == ItemStat.Type.EnhancedArmour)
                multiplyArmour = 1 + stat.value / 100f;

            else if (stat.type == ItemStat.Type.EnhancedBlock)
                multiplyBlock = 1 + stat.value / 100f;
        }

        damage = new Vector2(
            baseDamage.x * multiplyDamage + addDamage,
            baseDamage.y * multiplyDamage + addVariableDamage
        );

        attacksPerSecond = baseAttacksPer100Seconds / 100f * multiplySpeed;
        block = baseBlock / 100f * multiplyBlock;
        armour = baseArmour * multiplyArmour + addArmour;

        sellValue += (damage.x + damage.y / 2f) * attacksPerSecond * GOLD_VALUE_DPS;
        sellValue += block * GOLD_VALUE_BLOCK;
        sellValue += armour * GOLD_VALUE_ARMOUR;

        this.sellValue = Mathf.FloorToInt(sellValue);
    }

    
}
