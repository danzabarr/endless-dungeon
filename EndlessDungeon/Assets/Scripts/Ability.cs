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
        SnapShot snapshot,
        GameObject objects
    )
    {

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
        SnapShot snapshot,
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
        SnapShot snapshot,
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
        SnapShot snapshot,
        GameObject objects
    )
    {
        Vector3 castDirection = target == null ? (castTarget - caster.GetCastPosition()).normalized : (target.GetCenterPosition() - caster.GetCastPosition()).normalized;
        caster.LookInDirection(castDirection);
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
        SnapShot snapshot,
        GameObject objects
    )
    {
        Vector3 castDirection = target == null ? (castTarget - caster.GetCastPosition()).normalized : (target.GetCenterPosition() - caster.GetCastPosition()).normalized;

        switch (snapshot.mainHandWeaponClass.StandardAbilityType())
        {
            case AbilityType.Melee:
                List<Unit> targets = GetTargets(caster, target, castDirection, offHandSwing, snapshot);
                foreach (Unit hp in targets)
                {
                    hp.Damage(this, caster, DamageType.Physical, offHandSwing ? snapshot.offHandDamage.Roll() : snapshot.mainHandDamage.Roll(), true, true);
                    //hp.Knockback(castDirection, 100 / (Vector3.Distance(caster.GetCastPosition(), hp.GetCenterPosition())));
                }

                break;


            case AbilityType.Projectile:

                Instantiate(Projectile).Init(caster, castDirection, projectileSpeed, offHandSwing ? snapshot.offHandDamage : snapshot.mainHandDamage, DamageType.Physical);

                break;

            case AbilityType.Thrown:

                Instantiate(Throwable).ThrowAt(caster, throwTarget, offHandSwing ? snapshot.offHandDamage : snapshot.mainHandDamage, DamageType.Physical);

                break;
            
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
        SnapShot snapshot,
        GameObject objects
    )
    {
        
        
    }

    public float GetRange(bool offHandSwing, SnapShot snapshot)
    {
        if (Type == AbilityType.Projectile) return float.MaxValue;
        if (Type == AbilityType.Self) return float.MaxValue;

        if (!useWeaponRange)
            return range;

        if (Hand == WeaponHand.MainHand)
            return range * snapshot.mainHandRange;

        if (Hand == WeaponHand.OffHand)
            return range * snapshot.offHandRange;

        if (Hand == WeaponHand.Alternating)
            return range * (offHandSwing ? snapshot.offHandRange : snapshot.mainHandRange);

        if (Hand == WeaponHand.Both)
            return range * Mathf.Min(snapshot.offHandRange, snapshot.mainHandRange);

        if (Hand == WeaponHand.Spell)
            return range;

        return range;
    }

    public Vector2 GetDamage(bool offHandSwing, SnapShot snapshot)
    {
        if (Hand == WeaponHand.MainHand)
            return damage * snapshot.mainHandDamage;

        if (Hand == WeaponHand.OffHand)
            return damage * snapshot.offHandDamage;

        if (Hand == WeaponHand.Alternating)
            return damage * (offHandSwing ? snapshot.offHandDamage : snapshot.mainHandDamage);

        if (Hand == WeaponHand.Both)
            return damage * (snapshot.mainHandAttacksPerSecond + snapshot.offHandAttacksPerSecond);

        if (Hand == WeaponHand.Spell)
        {
            if (damageType == DamageType.Physical)
                return damage;

            if (damageType == DamageType.Fire)
               return damage * snapshot.fireSpellDamage;

            if (damageType == DamageType.Cold)
               return damage * snapshot.coldSpellDamage;

            if (damageType == DamageType.Lightning)
               return damage * snapshot.lightningSpellDamage;

            if (damageType == DamageType.Poison)
               return damage * snapshot.poisonSpellDamage;

            if (damageType == DamageType.Shadow)
                return damage * snapshot.shadowSpellDamage;

            if (damageType == DamageType.Holy)
                return damage * snapshot.holySpellDamage;
        }

        return damage;
    }

    public List<Unit> GetTargets
    (
        Unit caster,
        Unit target,
        Vector3 targetDirection,
        bool offHandSwing,
        SnapShot snapshot
    )
    {
        List<Unit> targets = new List<Unit>();

        bool targettingAllies = affects != Affects.Enemies;
        bool targettingEnemies = affects != Affects.Allies;
        Unit.Faction casterFaction = caster.GetFaction();
        float range = GetRange(offHandSwing, snapshot);
        

        void AddTarget(Unit hp)
        {
            if (hp != null && Filter(casterFaction, hp.GetFaction(), affects))
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