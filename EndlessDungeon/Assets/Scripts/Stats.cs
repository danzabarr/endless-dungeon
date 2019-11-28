using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public enum Hit
    {
        Immune,
        Zero,
        Miss,
        Hit,
        Crit,
    }

    [SerializeField]
    private float maxHealthBase = 20;
    [SerializeField]
    private float maxHealthPerStrength = 10;
    [SerializeField]
    private float healthRegenBase = 1;
    [SerializeField]
    private float healthRegenPerMagic = 0.01f;
    [SerializeField]
    private float weaponDamagePerStrength = 0.01f;
    [SerializeField]
    private float hitRecoverySpeedBase = 1;
    [SerializeField]
    private float hitRecoverySpeedPerStrength = 0.01f;
    [SerializeField]
    private float attackSpeedPerDexterity = 0.001f;
    [SerializeField]
    private float walkSpeedBase = 12;
    [SerializeField]
    private float spellDamagePerMagic = 0.01f;

    private AbilityManager abilities;

    [SerializeField]
    private int strength;
    [SerializeField]
    private int dexterity;
    [SerializeField]
    private int magic;

    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int regenHealth;
    [SerializeField]
    private float armour;

    [SerializeField]
    private int fireResist;
    [SerializeField]
    private int coldResist;
    [SerializeField]
    private int lightningResist;
    [SerializeField]
    private int poisonResist;
    [SerializeField]
    private int shadowResist;
    [SerializeField]
    private int holyResist;

    [SerializeField]
    private int attackSpeed;
    [SerializeField]
    private int hitRecoverySpeed;
    [SerializeField]
    private int fireDamage;
    [SerializeField]
    private int coldDamage;
    [SerializeField]
    private int lightningDamage;
    [SerializeField]
    private int poisonDamage;
    [SerializeField]
    private int shadowDamage;
    [SerializeField]
    private int holyDamage;

    [SerializeField]
    private EquipmentObject.Class mainHandWeaponClass;
    [SerializeField]
    private Vector2 mainHandDamage;
    [SerializeField]
    private float mainHandAttacksPerSecond;
    [SerializeField]
    private float mainHandBlock;
    [SerializeField]
    private float mainHandRange;
    [SerializeField]
    private EquipmentObject.Class offHandWeaponClass;
    [SerializeField]
    private Vector2 offHandDamage;
    [SerializeField]
    private float offHandAttacksPerSecond;
    [SerializeField]
    private float offHandBlock;
    [SerializeField]
    private float offHandRange;
    [SerializeField]
    private int walkSpeed;

    public float bonusMaxHealth { get; set; }
    public float bonusRegenHealth { get; set; }
    public float bonusArmour { get; set; }
    public float bonusBlock { get; set; }
    public float bonusWeaponDamage { get; set; }
    public float bonusAttackSpeed { get; set; }
    public float bonusHitRecoverySpeed { get; set; }
    public float bonusWalkSpeed { get; set; }
    public int bonusFireResist { get; set; }
    public int bonusColdResist { get; set; }
    public int bonusLightningResist { get; set; }
    public int bonusPoisonResist { get; set; }
    public int bonusShadowResist { get; set; }
    public int bonusHolyResist { get; set; }
    public float bonusFireDamage { get; set; }
    public float bonusColdDamage { get; set; }
    public float bonusLightningDamage { get; set; }
    public float bonusPoisonDamage { get; set; }
    public float bonusShadowDamage { get; set; }
    public float bonusHolyDamage { get; set; }

    public int Strength => strength;
    public int Dexterity => dexterity;
    public int Magic => magic;
    public float MaxHealth => Mathf.Max(0, (maxHealthBase + maxHealth + strength * maxHealthPerStrength) * bonusMaxHealth);
    public float RegenHealth => (healthRegenBase + regenHealth / 100f + magic * healthRegenPerMagic) * bonusRegenHealth;
    public float Armour => armour * bonusArmour;
    public float PhysicalMitigation => 1f - Multiplier(Armour);
    public float BlockChance => Mathf.Clamp(mainHandBlock + offHandBlock, 0, .75f);
    public int FireResist => fireResist + bonusFireResist;
    public int ColdResist => coldResist + bonusColdResist;
    public int LightningResist => lightningResist + bonusLightningResist;
    public int PoisonResist => poisonResist + bonusPoisonResist;
    public int ShadowResist => shadowResist + bonusShadowResist;
    public int HolyResist => holyResist + bonusHolyResist;

    public float FireMitigation => Mathf.Max(0f, FireResist / 100f);
    public float ColdMitigation => Mathf.Max(0f, ColdResist / 100f);
    public float LightningMitigation => Mathf.Max(0f, LightningResist / 100f);
    public float PoisonMitigation => Mathf.Max(0f, PoisonResist / 100f);
    public float ShadowMitigation => Mathf.Max(0f, ShadowResist / 100f);
    public float HolyMitigation => Mathf.Max(0f, HolyResist / 100f);

    public float FireSpellDamage => Mathf.Max(0, (1 + magic * spellDamagePerMagic) * (1 + fireDamage / 100f) * bonusFireDamage);
    public float ColdSpellDamage => Mathf.Max(0, (1 + magic * spellDamagePerMagic) * (1 + coldDamage / 100f) * bonusColdDamage);
    public float LightningSpellDamage => Mathf.Max(0, (1 + magic * spellDamagePerMagic) * (1 + lightningDamage / 100f) * bonusLightningDamage);
    public float PoisonSpellDamage => Mathf.Max(0, (1 + magic * spellDamagePerMagic) * (1 + poisonDamage / 100f) * bonusPoisonDamage);
    public float ShadowSpellDamage => Mathf.Max(0, (1 + magic * spellDamagePerMagic) * (1 + shadowDamage / 100f) * bonusShadowDamage);
    public float HolySpellDamage => Mathf.Max(0, (1 + magic * spellDamagePerMagic) * (1 + holyDamage / 100f) * bonusHolyDamage);
         
    public float AttackSpeed => Mathf.Max(0, (1 + dexterity * attackSpeedPerDexterity) * (1 + attackSpeed / 100f) * bonusAttackSpeed);
    public float HitRecoverySpeed => Mathf.Max(1, (hitRecoverySpeedBase + strength * hitRecoverySpeedPerStrength) * bonusHitRecoverySpeed);
    public float HitRecoveryDuration => 1f / HitRecoverySpeed;
    public float WalkSpeed => Mathf.Max(0, walkSpeedBase * (1 + walkSpeed / 100f) * bonusWalkSpeed);

    public bool MainHandEquipped => mainHandWeaponClass != EquipmentObject.Class.Unarmed;
    public EquipmentObject.Class MainHandItemClass => mainHandWeaponClass;
    public Vector2 MainHandDamage => (mainHandDamage * (1 + strength * weaponDamagePerStrength) * bonusWeaponDamage).Max(Vector2.zero);
    public float MainHandAttacksPerSecond => Mathf.Max(0, mainHandAttacksPerSecond * AttackSpeed);
    public float MainHandBlockChance => mainHandBlock;
    public float MainHandRange => mainHandRange;

    public bool OffHandEquipped => offHandWeaponClass != EquipmentObject.Class.Unarmed;
    public EquipmentObject.Class OffHandItemClass => offHandWeaponClass;
    public Vector2 OffHandDamage => (offHandDamage * (1 + strength * weaponDamagePerStrength) * bonusWeaponDamage).Max(Vector2.zero);
    public float OffHandAttacksPerSecond => Mathf.Max(0, offHandAttacksPerSecond * AttackSpeed);
    public float OffHandBlockChance => offHandBlock;
    public float OffHandRange => offHandRange;

    public float Mitigation(Ability.DamageType damageType)
    {
        switch (damageType)
        {
            case Ability.DamageType.Physical:
                return PhysicalMitigation;
            case Ability.DamageType.Fire:
                return FireMitigation;
            case Ability.DamageType.Cold:
                return ColdMitigation;
            case Ability.DamageType.Lightning:
                return LightningMitigation;
            case Ability.DamageType.Poison:
                return PoisonMitigation;
            case Ability.DamageType.Shadow:
                return ShadowMitigation;
            case Ability.DamageType.Holy:
                return HolyMitigation;
            default:
                return 0;
        }
    }

    public void Awake()
    {
        ResetBonusStats();
    }

    public void Start()
    {
        abilities = GetComponent<AbilityManager>();
        ResetBonusStats();
        abilities.Equip(this);
    }
    
    
#if UNITY_EDITOR
    public void OnValidate()
    {
        //abilities = GetComponent<AbilityManager>();
        //ResetBonusStats();
        //abilities.Equip(this);

    }
#endif

     
    [ContextMenu("Reset Bonus Stats")]
    public void ResetBonusStats()
    {
        bonusMaxHealth = 1;
        bonusRegenHealth = 1;
        bonusArmour = 1;
        bonusBlock = 1;
        bonusWeaponDamage = 1;
        bonusAttackSpeed = 1;
        bonusHitRecoverySpeed = 1;
        bonusWalkSpeed = 1;
        bonusFireResist = 0;
        bonusColdResist = 0;
        bonusLightningResist = 0;
        bonusPoisonResist = 0;
        bonusShadowResist = 0;
        bonusHolyResist = 0;
        bonusFireDamage = 1;
        bonusColdDamage = 1;
        bonusLightningDamage = 1;
        bonusPoisonDamage = 1;
        bonusShadowDamage = 1;
        bonusHolyDamage = 1;
    }



    public void OnDealDamage(object source, Unit target, float amount, Ability.DamageType damageType)
    {

    }

    public void OnReceiveDamage(object source, Unit attacker, float amount, Ability.DamageType damageType)
    {

    }

    public void OnBlock(object source, Unit attacker, float amountBlocked, Ability.DamageType damageType)
    {

    }

    public void OnDealKillingBlow(object source, Unit target)
    {

    }

    public void OnReceiveKillingBlow(object source, Unit target)
    {

    }

    public void OnDeath()
    {

    }

    public void OnAbilityStart(Ability ability, Unit target, Vector3 castTarget, Vector3 throwTarget, Vector3 floorTarget)
    {

    }

    public void OnAbilityPulse(Ability ability, Unit target, Vector3 castTarget, Vector3 throwTarget, Vector3 floorTarget)
    {

    }

    public void OnAbilityEnd(Ability ability, Unit target, Vector3 castTarget, Vector3 throwTarget, Vector3 floorTarget)
    {

    }

    [ContextMenu("Reset Gear Stats")]
    public void ResetGearStats()
    {
        mainHandWeaponClass = EquipmentObject.Class.Unarmed;
        mainHandDamage = Vector2.zero;
        mainHandAttacksPerSecond = 1;
        mainHandBlock = 0;
        mainHandRange = 1;

        offHandWeaponClass = EquipmentObject.Class.Unarmed;
        offHandDamage = Vector2.zero;
        offHandAttacksPerSecond = 1;
        offHandBlock = 0;
        offHandRange = 1;

        maxHealth = 0;
        regenHealth = 0;
        armour = 0;
        attackSpeed = 0;
        hitRecoverySpeed = 0;
        walkSpeed = 0;
        fireResist = 0;
        coldResist = 0;
        lightningResist = 0;
        poisonResist = 0;
        shadowResist = 0;
        holyResist = 0;
        fireDamage = 0;
        coldDamage = 0;
        lightningDamage = 0;
        poisonDamage = 0;
        shadowDamage = 0;
        holyDamage = 0;
    }

    public void RecalculateGearStats
        (
            EquipmentObject mainHand, 
            EquipmentObject offHand,
            EquipmentObject head,
            EquipmentObject body,
            EquipmentObject hands,
            EquipmentObject feet,
            EquipmentObject finger,
            EquipmentObject neck
        )
    {
        ResetGearStats();

        void AddStats(EquipmentObject e)
        {
            if (e == null)
                return;

            armour += e.Armour;

            for (int i = 0; i < e.StatsLength; i++)
            {
                ItemStat s = e.Stat(i);
                if (s.type == ItemStat.Type.MaxHealth)
                    maxHealth += s.value;
                else if (s.type == ItemStat.Type.RegenHealth)
                    regenHealth += s.value;
                else if (s.type == ItemStat.Type.HitRecoverySpeed)
                    hitRecoverySpeed += s.value;
                else if (s.type == ItemStat.Type.AttackSpeed)
                    attackSpeed += s.value;
                else if (s.type == ItemStat.Type.WalkSpeed)
                    walkSpeed += s.value;
                else if (s.type == ItemStat.Type.FireSpellDamage)
                    fireDamage += s.value;
                else if (s.type == ItemStat.Type.ColdSpellDamage)
                    coldDamage += s.value;
                else if (s.type == ItemStat.Type.LightningSpellDamage)
                    lightningDamage += s.value;
                else if (s.type == ItemStat.Type.PoisonSpellDamage)
                    poisonDamage += s.value;
                else if (s.type == ItemStat.Type.ShadowSpellDamage)
                    shadowDamage += s.value;
                else if (s.type == ItemStat.Type.HolySpellDamage)
                    holyDamage += s.value;
                else if (s.type == ItemStat.Type.FireResistance)
                    fireResist += s.value;
                else if (s.type == ItemStat.Type.ColdResistance)
                    fireResist += s.value;
                else if (s.type == ItemStat.Type.LightningResistance)
                    lightningResist += s.value;
                else if (s.type == ItemStat.Type.PoisonResistance)
                    poisonResist += s.value;
                else if (s.type == ItemStat.Type.ShadowResistance)
                    shadowResist += s.value;
                else if (s.type == ItemStat.Type.HolyResistance)
                    holyResist += s.value;
            }
        }

        if (mainHand != null)
        {
            mainHandWeaponClass = mainHand.ItemClass;
            mainHandDamage = mainHand.Damage;
            mainHandAttacksPerSecond = mainHand.AttacksPerSecond;
            mainHandBlock = mainHand.Block;
            mainHandRange = mainHand.Range;
        }

        if (offHand != null)
        {
            offHandWeaponClass = offHand.ItemClass;
            offHandDamage = offHand.Damage;
            offHandAttacksPerSecond = offHand.AttacksPerSecond;
            offHandBlock = offHand.Block;
            offHandRange = offHand.Range;
        }

        AddStats(head);
        AddStats(neck);
        AddStats(body);
        AddStats(hands);
        AddStats(finger);
        AddStats(feet);
        AddStats(mainHand);
        AddStats(offHand);
    }

    //Returns Hit, Miss or Crit depending on attack vs defence rating
    public static Hit RollHit(float attackerRating, float defenderRating, float bonusHit, float bonusCrit)
    {
        float random = Random.value;
        float hitChance = Mathf.Clamp(attackerRating / (attackerRating + defenderRating) * 1.5f, 0f, 1f);
        float critChance = hitChance * Mathf.Clamp(attackerRating / (attackerRating + defenderRating) * 0.5f, 0f, 1f);

        hitChance = Mathf.Clamp(hitChance + bonusHit, 0f, 1f);
        critChance = Mathf.Clamp(critChance + bonusCrit, 0f, 1f);

        if (random <= critChance)
            return Hit.Crit;

        if (random <= Mathf.Clamp(hitChance, 0.05f, 0.95f))
            return Hit.Hit;
       
            return Hit.Miss;
    }

    public static float Multiplier(float armour)
    {
        return armour >= 0 ?
                 100f / (100f + armour) :
            2f - 100f / (100f - armour);
    }
}
