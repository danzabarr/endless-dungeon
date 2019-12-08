using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="Ability", menuName ="Abilities/Ability")]
public class Ability : ScriptableObject
{
    public enum DamageType
    {
        Physical,       //Attack damage is not multiplied by elemental spell damage stat
        Fire,          
        Cold,           
        Lightning,
        Poison,
        Shadow,
        Holy
    }

    public enum WeaponHand
    {
        Spell,          //Cast speed and elemental spell damage stats are used instead of weapon speed and damage
        MainHand,       
        OffHand,        
        Both,           
        Alternating,
    }

    public enum AbilityType
    {
        Self,       //Does an effect to self
        Targeted,   //Does an effect to target
        Melee,      //Does an effect to the first raycast hit forward of the caster within range
        Cleave,     //Does an effect to all targets in range and in an arc in front of the caster
        Nova,       //Does an effect to all targets in range of the caster
        Projectile, //Fires a projectile in a direction
        Thrown,     //Fires a throwable towards a position
        Place,      //Places an object at a position (use for patch AOE)
        Auto,       //Ability type is determined by the equipped weapon class
    }

    public enum Affects
    {
        Enemies,
        Allies,
        Neutrals
    }

    public static bool Filter(Unit.Faction casterFaction, Unit.Faction targetFaction, Affects affects)
    {
        if (targetFaction == Unit.Faction.Neutral)
            return (int)affects == ((int)affects | (1 << (int)Affects.Neutrals));

        if (casterFaction == targetFaction)
            return (int)affects == ((int)affects | (1 << (int)Affects.Allies));

        if (casterFaction != targetFaction)
            return (int)affects == ((int)affects | (1 << (int)Affects.Enemies));

        return false;
    }

    [SerializeField]
    private new string name;

    [SerializeField]
    private Texture2D icon;

    [SerializeField]
    [TextArea]
    private string description;

    [SerializeField]
    private WeaponHand weaponHand;

    [SerializeField]
    [EnumFlags]
    private EquipmentObject.Class mainHandRequirement, offHandRequirement, activeHandRequirement;

    [SerializeField]
    private AbilityAnimation[] animations;

    [SerializeField]
    private float cooldown;

    [SerializeField]
    private float speed;

    [SerializeField]
    private AbilityType abilityType;

    [SerializeField]
    private bool channelled;

    [SerializeField]
    private float channellingTickInterval;

    [SerializeField]
    [EnumFlags]
    private Affects affects;

    [SerializeField]
    private Vector2 damage;

    [SerializeField]
    private DamageType damageType;

    [SerializeField]
    private Projectile projectile;

    [SerializeField]
    public float projectileSpeed;

    [SerializeField]
    private Throwable throwable;

    [SerializeField]
    private Placeable placeable;

    [SerializeField]
    private int maxObjects;

    public enum MaxObjectsReachedBehaviour
    {
        Nothing,
        DeleteOldest,
        DeleteFurthest,
        PreventNew
    }

    [SerializeField]
    private MaxObjectsReachedBehaviour maxObjectsReached;

    [SerializeField]
    private bool usePattern;

    [SerializeField]
    private string pattern;

    [SerializeField]
    private float range;

    [SerializeField]
    private float arc;

    [SerializeField]
    private bool useWeaponRange;
    public string Name => name;
    public string Description => description;
    public Texture2D Icon => icon;
    public AbilityType Type => abilityType;
    public bool Channelled => channelled;
    public float ChannellingTickInterval => channellingTickInterval;
    public WeaponHand Hand => weaponHand;
    public EquipmentObject.Class MainHandRequirement => mainHandRequirement;
    public EquipmentObject.Class OffHandRequirement => offHandRequirement;
    public EquipmentObject.Class ActiveHandRequirement => activeHandRequirement;
    public float Speed => speed;
    public float Cooldown => cooldown;
    public Affects AbilityAffects => affects;
    public Vector2 Damage => damage;
    public DamageType DmgType => damageType;
    public Projectile Projectile => projectile;
    public Throwable Throwable => throwable;
    public Placeable Placeable => placeable;
    public bool UsePattern => usePattern;
    public string Pattern => pattern;
    public bool Compatible(EquipmentObject.Class mainHand, EquipmentObject.Class offHand, EquipmentObject.Class activeHand)
    {
        return mainHandRequirement.IsCompatible(mainHand) && offHandRequirement.IsCompatible(offHand) && activeHandRequirement.IsCompatible(activeHand);
    }
    
    public string RollAnimation(EquipmentObject.Class mainHand, EquipmentObject.Class offHand, bool offHandSwing)
    {
        List<string> strings = new List<string>();

        foreach (AbilityAnimation a in animations)
        {
            //Ignore animations for the wrong hand.
            if (a.offHandSwing != offHandSwing)
                continue;

            //Ignore animations whose main hand requirements aren't met by the main hand.
            if (!a.mainHand.IsCompatible(mainHand))
                continue;

            //Ignore animations whose off hand requirements aren't met by the off hand.
            if (!a.offHand.IsCompatible(offHand))
                continue;

            strings.Add(a.trigger);
        }

        if (strings.Count <= 0)
        {
            Debug.LogWarning("No suitable animation available for the ability.");
            return "";
        }

        return strings[Random.Range(0, strings.Count)];
    }
    public virtual void OnStartCasting
    (
        Unit caster,
        Unit target,
        Vector3 castTarget,
        Vector3 throwTarget,
        Vector3 floorTarget,
        float swingTime,
        bool offHandSwing,
        int patternPosition,
        bool channelling,
        GameObject objects
    )
    {
        if (Type == AbilityType.Auto)
        {
            AbilityType abilityType = offHandSwing ? caster.Stats.OffHandItemClass.StandardAbilityType() : caster.Stats.MainHandItemClass.StandardAbilityType();
            if (abilityType == AbilityType.Projectile)
            {
                Projectile projectile = Projectile;
                if (projectile == null)
                    projectile = offHandSwing ? caster.Stats.OffHandProjectile : caster.Stats.MainHandProjectile;

                caster.CockedMHProjectile = Instantiate(projectile);
            }
        }
    }

    public virtual void OnStartChannelling
    (
        Unit caster,
        Unit target,
        Vector3 castTarget,
        Vector3 throwTarget,
        Vector3 floorTarget,
        float swingTime,
        bool offHandSwing,
        int patternPosition,
        bool channelling,
        GameObject objects
    )
    {

    }

    public virtual void OnChannellingPulse
    (
       Unit caster,
        Unit target,
        Vector3 castTarget,
        Vector3 throwTarget,
        Vector3 floorTarget,
        float swingTime,
        bool offHandSwing,
        int patternPosition,
        bool channelling,
        GameObject objects
    )
    {

    }

    public virtual void OnUpdate
    (
        Unit caster,
        Unit target,
        Vector3 castTarget,
        Vector3 throwTarget,
        Vector3 floorTarget,
        float swingTime,
        bool offHandSwing,
        int patternPosition,
        bool channelling,
        GameObject objects
    )
    {
        Vector3 castDirection = target == null ? (castTarget - caster.GetCastPosition()).normalized : (target.GetCenterPosition() - caster.GetCastPosition()).normalized;
        caster.YawToDirection(castDirection);
        caster.PitchToDirection(castDirection);
    }
    public virtual void OnPulse
    (
        Unit caster,
        Unit target,
        Vector3 castTarget,
        Vector3 throwTarget,
        Vector3 floorTarget,
        float swingTime,
        bool offHandSwing,
        int patternPosition,
        bool channelling,
        GameObject objects
    )
    {
        if (Type == AbilityType.Auto)
        {
            Vector3 castDirection;
            AbilityType abilityType = offHandSwing ? caster.Stats.OffHandItemClass.StandardAbilityType() : caster.Stats.MainHandItemClass.StandardAbilityType();
            switch (abilityType)
            {
                case AbilityType.Melee:
                    castDirection = target == null ? (castTarget - caster.GetCastPosition()).normalized : (target.GetCenterPosition() - caster.GetCastPosition()).normalized;
                    List<Unit> targets = GetTargets(caster, target, castDirection, offHandSwing);
                    foreach (Unit hp in targets)
                    {
                        hp.Damage(this, caster.GetCastPosition(), caster, damageType, ((offHandSwing ? caster.Stats.OffHandDamage : caster.Stats.MainHandDamage) * damage).Roll(), true, true);
                        //hp.Knockback(castDirection, 100 / (Vector3.Distance(caster.GetCastPosition(), hp.GetCenterPosition())));
                    }

                    break;


                case AbilityType.Projectile:

                    if (caster.CockedMHProjectile != null)
                    {
                        Vector3 projectilePosition = caster.CockedMHProjectile.transform.position;
                        castDirection = target == null ? (castTarget - projectilePosition).normalized : (target.GetCenterPosition() - projectilePosition).normalized;
                        caster.ShootMHProjectile(castDirection, projectileSpeed, (offHandSwing ? caster.Stats.OffHandDamage : caster.Stats.MainHandDamage) * damage, damageType, affects);
                    }

                    break;

                case AbilityType.Thrown:

                    Instantiate(Throwable).ThrowAt(caster, throwTarget, (offHandSwing ? caster.Stats.OffHandDamage : caster.Stats.MainHandDamage) * damage, damageType);

                    break;
            
            }
        }
    }

    public virtual void OnEnd
    (
        Unit caster,
        Unit target,
        Vector3 castTarget,
        Vector3 throwTarget,
        Vector3 floorTarget,
        float swingTime,
        bool offHandSwing,
        int patternPosition,
        bool channelling,
        GameObject objects
    )
    {
        
        if (Type == AbilityType.Auto)
        {
            AbilityType abilityType = offHandSwing ? caster.Stats.OffHandItemClass.StandardAbilityType() : caster.Stats.MainHandItemClass.StandardAbilityType();
            if (abilityType == AbilityType.Projectile)
                caster.CockedMHProjectile = null;
        }
    }

    public virtual void OnCancel
    (
        Unit caster,
        Unit target,
        Vector3 castTarget,
        Vector3 throwTarget,
        Vector3 floorTarget,
        float swingTime,
        bool offHandSwing,
        int patternPosition,
        bool channelling,
        GameObject objects
    )
    {
        AbilityType abilityType = offHandSwing ? caster.Stats.OffHandItemClass.StandardAbilityType() : caster.Stats.MainHandItemClass.StandardAbilityType();
        if (abilityType == AbilityType.Projectile)
        {
            caster.CockedMHProjectile = null;
        }
    }

    public float GetRange(bool offHandSwing, Stats stats)
    {
        if (Type == AbilityType.Projectile)
            return float.MaxValue;

        if (Type == AbilityType.Self)
            return float.MaxValue;

        if (!useWeaponRange)
            return range;

        if (Type == AbilityType.Auto)
        {
            if (Hand == WeaponHand.MainHand || (Hand == WeaponHand.Alternating && !offHandSwing))
                switch (stats.MainHandItemClass.StandardAbilityType())
                {
                    case AbilityType.Melee:
                        return range * stats.MainHandRange;
                    
                    case AbilityType.Projectile:
                        return float.MaxValue;

                    case AbilityType.Thrown:
                        return range * stats.MainHandRange;
                }
            else if (Hand == WeaponHand.OffHand || (Hand == WeaponHand.Alternating && offHandSwing))
            {
                switch (stats.OffHandItemClass.StandardAbilityType())
                {
                    case AbilityType.Melee:
                        return range * stats.OffHandRange;

                    case AbilityType.Projectile:
                        return float.MaxValue;

                    case AbilityType.Thrown:
                        return range * stats.OffHandRange;
                }
            }
            else if (Hand == WeaponHand.Both)
            {
                switch (stats.OffHandItemClass.StandardAbilityType())
                {
                    case AbilityType.Melee:
                        return range * Mathf.Min(stats.OffHandRange, stats.MainHandRange);

                    case AbilityType.Projectile:
                        return float.MaxValue;

                    case AbilityType.Thrown:
                        return range * Mathf.Min(stats.OffHandRange, stats.MainHandRange);
                }
            }
        }

        if (Hand == WeaponHand.MainHand)
            return range * stats.MainHandRange;

        if (Hand == WeaponHand.OffHand)
            return range * stats.OffHandRange;

        if (Hand == WeaponHand.Alternating)
            return range * (offHandSwing ? stats.OffHandRange : stats.MainHandRange);

        if (Hand == WeaponHand.Both)
            return range * Mathf.Min(stats.OffHandRange, stats.MainHandRange);

        if (Hand == WeaponHand.Spell)
            return range;

        return range;
    }

    public Vector2 GetDamage(bool offHandSwing, Stats stats)
    {
        if (Type == AbilityType.Auto)
        {
            if (Hand == WeaponHand.MainHand || (Hand == WeaponHand.Alternating && !offHandSwing))
                return damage * stats.MainHandDamage;

            if (Hand == WeaponHand.OffHand || (Hand == WeaponHand.Alternating && offHandSwing))
                return damage * stats.OffHandDamage;

            if (Hand == WeaponHand.Both)
                return damage * (stats.MainHandAttacksPerSecond + stats.OffHandAttacksPerSecond);
        }

        if (Hand == WeaponHand.MainHand)
            return damage * stats.MainHandDamage;

        if (Hand == WeaponHand.OffHand)
            return damage * stats.OffHandDamage;

        if (Hand == WeaponHand.Alternating)
            return damage * (offHandSwing ? stats.OffHandDamage : stats.MainHandDamage);

        if (Hand == WeaponHand.Both)
            return damage * (stats.MainHandAttacksPerSecond + stats.OffHandAttacksPerSecond);

        if (Hand == WeaponHand.Spell)
        {
            if (damageType == DamageType.Physical)
                return damage;

            if (damageType == DamageType.Fire)
               return damage * stats.FireSpellDamage;

            if (damageType == DamageType.Cold)
               return damage * stats.ColdSpellDamage;

            if (damageType == DamageType.Lightning)
               return damage * stats.LightningSpellDamage;

            if (damageType == DamageType.Poison)
               return damage * stats.PoisonSpellDamage;

            if (damageType == DamageType.Shadow)
                return damage * stats.ShadowSpellDamage;

            if (damageType == DamageType.Holy)
                return damage * stats.HolySpellDamage;
        }

        return damage;
    }

    public List<Unit> GetTargets
    (
        Unit caster,
        Unit target,
        Vector3 targetDirection,
        bool offHandSwing
    )
    {
        
        if (Type == AbilityType.Auto)
        {
            if (Hand == WeaponHand.MainHand || (Hand == WeaponHand.Alternating && !offHandSwing))
                return GetTargets(caster.Stats.MainHandItemClass.StandardAbilityType());

            if (Hand == WeaponHand.OffHand || (Hand == WeaponHand.Alternating && offHandSwing))
                return GetTargets(caster.Stats.OffHandItemClass.StandardAbilityType());

            if (Hand == WeaponHand.Both)
                throw new System.NotImplementedException();
        }

        return GetTargets(Type);

        List<Unit> GetTargets(AbilityType abilityType)
        {
            List<Unit> targets = new List<Unit>();
            float range = GetRange(offHandSwing, caster.Stats);

            void AddTarget(Unit hp)
            {
                if (hp != null && Filter(caster.GetFaction(), hp.GetFaction(), affects))
                    targets.Add(hp);
            }

        
        
            switch (abilityType)
            {
                case AbilityType.Self:
                    targets.Add(caster);
                    break;

                case AbilityType.Targeted:
                    targets.Add(target);
                    break;

                case AbilityType.Melee:

                    if (target != null)
                    {
                        float distance = Vector3.Distance(caster.GetCastPosition(), target.GetCenterPosition());
                        if (distance <= range)
                        {
                            bool checkLOS = Level.CheckLOS(caster.GetCastPosition(), target.GetCenterPosition());
                            if (!checkLOS)
                            {
                                AddTarget(target);
                                break;
                            }
                        }
                    }
                
                    Collider[] colliders = caster.GetGameObject().GetComponentsInChildren<Collider>();
                    foreach (Collider c in colliders)
                        c.enabled = false;

                    int layerMask = LayerMask.GetMask("Default", "Walls", "Player", "Mobs");

                    if (Physics.Raycast(new Ray(caster.GetCastPosition(), targetDirection), out RaycastHit hitinfo, range, layerMask))
                    {
                        AddTarget(hitinfo.collider.GetComponent<Unit>());
                    }

                    //Spherecast?
                    /*
                    float radius = 1f;
                    if (Physics.SphereCast(new Ray(caster.CastPosition(), targetDirection), radius, out RaycastHit hitinfo, range, layerMask))
                    {
                        AddTarget(hitinfo.collider.GetComponent<HealthPool>());
                    }
                    */

                    foreach (Collider c in colliders)
                        c.enabled = true;

                

                    break;
                case AbilityType.Cleave:

                    float yaw = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

                    if (Level.PlayerInRadius(caster.GetCastPosition(), range, yaw, arc, true))
                        AddTarget(Player.Instance);

                    foreach (Mob mob in Level.Instance.MobsInRadius(caster.GetCastPosition(), range, yaw, arc, true))
                        AddTarget(mob);

                    break;
                case AbilityType.Nova:

                    if (Level.PlayerInRadius(caster.GetCastPosition(), range, true))
                        AddTarget(Player.Instance);

                    foreach (Mob mob in Level.Instance.MobsInRadius(caster.GetCastPosition(), range, true))
                        targets.Add(mob);

                    break;
                case AbilityType.Projectile:
                    break;
                case AbilityType.Thrown:
                    break;
                case AbilityType.Place:
                    break;
            }
            return targets;
        }
    }
}