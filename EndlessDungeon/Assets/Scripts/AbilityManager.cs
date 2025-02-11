﻿using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class AbilityManager : MonoBehaviour
{
    private Unit caster;
    private Animator anim;

    [SerializeField]
    private Ability[] abilities;
    private float[] cooldowns;
    private GameObject[] objects;
    private string[] mainHandAnimations;
    private string[] offHandAnimations;

    private int activeIndex = -1;
    private Unit target;
    private Vector3 castTarget;
    private Vector3 throwTarget;
    private Vector3 floorTarget;

    private int queuedIndex = -1;
    private Unit queuedTarget;
    private Vector3 queuedCastTarget;
    private Vector3 queuedThrowTarget;
    private Vector3 queuedFloorTarget;

    private float swingTime;
    private int patternPosition;
    private bool offHandSwing;
    private bool channelling;

    [SerializeField]
    private ErrorMessageDisplay errorMessageDisplay;

    private Stats stats;
    public Ability Active => this[activeIndex];
    public bool Casting => activeIndex != -1;
    public bool Channelling => channelling;
    public float CastTimeRemaining => channelling ? 1 : (swingTime * CastTimeTotal);
    public float CastTimeTotal => Active == null ? 0 : 1f / GetCastSpeed(Active);
    public Ability this[int index] => index < 0 || index >= abilities.Length ? null : abilities[index];
    public int Length => abilities.Length;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        caster = GetComponent<Unit>();
        cooldowns = new float[abilities.Length];
        objects = new GameObject[abilities.Length];
        for (int i = 0; i < abilities.Length; i++)
            objects[i] = new GameObject("Ability" + i + " Objects");

        for (int i = 0; i < abilities.Length; i++)
            if (abilities[i] != null)
                cooldowns[i] = abilities[i].Cooldown;
    }

    public void SetAbility(Ability ability, int index)
    {
        if (queuedIndex == index)
            CancelQueued();
        if (activeIndex == index)
            CancelActive();

        abilities[index] = ability;
        cooldowns[index] = 0;
    }

    public void Equip(Stats stats)
    {
        this.stats = stats;
        UpdateStats();
    }

    private void UpdateStats()
    {
        if (stats != null)
        {

            if (!stats.MainHandItemClass.HasDamage())
                offHandSwing = true;

            if (!stats.OffHandItemClass.HasDamage())
                offHandSwing = false;

            if (Active != null)
                anim.SetFloat("attackspeed", GetCastSpeed(Active));
        }

        if (Active != null && !Active.Compatible(stats.MainHandItemClass, stats.OffHandItemClass, offHandSwing ? stats.OffHandItemClass : stats.MainHandItemClass))
            CancelActive();
    }

    private void ToggleHandSwing()
    {
        offHandSwing = !offHandSwing;

        if (!offHandSwing && !stats.MainHandItemClass.HasDamage())
            offHandSwing = true;

        if (offHandSwing && !stats.OffHandItemClass.HasDamage())
            offHandSwing = false;
    }

    public void Update()
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            if (abilities[i] == null)
                continue;

            float cd = abilities[i].Cooldown;
            if (cooldowns[i] < cd)
                cooldowns[i] += Time.deltaTime;

            if (cooldowns[i] >= cd)
                cooldowns[i] = cd;
        }

        if (queuedTarget != null && queuedTarget.GetCurrentHealth() <= 0)
        {
            queuedTarget = null;
        }

        if (target != null && target.GetCurrentHealth() <= 0)
        {
            CancelActive();
            target = null;
        }

        Ability active = this[activeIndex];

        caster.PitchToDirection(caster.transform.forward);

        if (active != null)
        {
            UpdateStats();
        }

        if (active == null)
            swingTime = 0;
        else
        {
            if (active.Updates)
                active.OnUpdate
                (
                    new Ability.AbilityArgs
                    {
                        caster = caster,
                        target = target,
                        castTarget = castTarget,
                        throwTarget = throwTarget,
                        floorTarget = floorTarget,
                        swingTime = swingTime,
                        offHandSwing = offHandSwing,
                        patternPosition = patternPosition,
                        channelling = channelling,
                        objects = objects[activeIndex]
                    }
                );
        }

        if (active != null && !channelling && active.UsePattern && active.Pattern.Length > 0)
        {
            while (swingTime <= 1 - (1 / Mathf.Max(1, active.Pattern.Length - 1) * (patternPosition)) && patternPosition < active.Pattern.Length)
            {
                if (active.Pattern[patternPosition] == 'X')
                {
                    active.OnPulse
                    (
                        new Ability.AbilityArgs
                        {
                            caster = caster,
                            target = target,
                            castTarget = castTarget,
                            throwTarget = throwTarget,
                            floorTarget = floorTarget,
                            swingTime = swingTime,
                            offHandSwing = offHandSwing,
                            patternPosition = patternPosition,
                            channelling = channelling,
                            objects = objects[activeIndex]
                        }
                    );

                    if (stats != null)
                        stats.OnAbilityPulse(active, target, castTarget, throwTarget, floorTarget);
                }
                patternPosition++;
            }
        }

        if (swingTime <= 0)
        {
            if (active != null)
            {
                if (active.Channelled)
                {
                    if (!channelling)
                    {
                        active.OnStartChannelling
                        (
                            new Ability.AbilityArgs
                            {
                                caster = caster,
                                target = target,
                                castTarget = castTarget,
                                throwTarget = throwTarget,
                                floorTarget = floorTarget,
                                swingTime = swingTime,
                                offHandSwing = offHandSwing,
                                patternPosition = patternPosition,
                                channelling = channelling,
                                objects = objects[activeIndex]
                            }
                        );
                    }
                    else
                    {
                        active.OnChannellingPulse
                        (
                            new Ability.AbilityArgs
                            {
                                caster = caster,
                                target = target,
                                castTarget = castTarget,
                                throwTarget = throwTarget,
                                floorTarget = floorTarget,
                                swingTime = swingTime,
                                offHandSwing = offHandSwing,
                                patternPosition = patternPosition,
                                channelling = channelling,
                                objects = objects[activeIndex]
                            }
                        );
                    }

                    channelling = true;
                    swingTime += 1;
                    patternPosition = 0;

                    return;
                }

                if (!active.UsePattern)
                {
                    active.OnPulse
                    (
                        new Ability.AbilityArgs
                        {
                            caster = caster,
                            target = target,
                            castTarget = castTarget,
                            throwTarget = throwTarget,
                            floorTarget = floorTarget,
                            swingTime = swingTime,
                            offHandSwing = offHandSwing,
                            patternPosition = patternPosition,
                            channelling = channelling,
                            objects = objects[activeIndex]
                        }
                    );

                    if (stats != null)
                        stats.OnAbilityPulse(active, target, castTarget, throwTarget, floorTarget);
                }

                if (!active.Channelled)
                {
                    active.OnEnd
                    (
                        new Ability.AbilityArgs
                        {
                            caster = caster,
                            target = target,
                            castTarget = castTarget,
                            throwTarget = throwTarget,
                            floorTarget = floorTarget,
                            swingTime = swingTime,
                            offHandSwing = offHandSwing,
                            patternPosition = patternPosition,
                            channelling = channelling,
                            objects = objects[activeIndex]
                        }
                    );

                    if (stats != null)
                        stats.OnAbilityEnd(active, target, castTarget, throwTarget, floorTarget);

                    activeIndex = -1;
                    active = null;
                    ToggleHandSwing();
                }
            }

            if (swingTime <= 0 && this[queuedIndex] != null)
            {

                bool floorTargetValid = false;

                if (!Physics.Raycast(new Ray(caster.GetGroundPosition(), queuedFloorTarget - caster.GetGroundPosition()), (queuedFloorTarget - caster.GetGroundPosition()).magnitude, LayerMask.GetMask("Walls", "Doors")))
                {
                    floorTargetValid = true;
                }

                CastOutcome check = CheckValid(queuedIndex, queuedTarget, queuedCastTarget, queuedThrowTarget, queuedFloorTarget, floorTargetValid);

                if (check != CastOutcome.Valid)
                {
                    ErrorMessage(check);
                    swingTime = 0;
                    return;
                }


                activeIndex = queuedIndex;
                active = abilities[activeIndex];
                UpdateStats();
                target = queuedTarget;
                castTarget = queuedCastTarget;
                throwTarget = queuedThrowTarget;
                floorTarget = queuedFloorTarget;
                queuedIndex = -1;

                switch (active.Hand)
                {
                    case Ability.WeaponHand.Spell:
                        offHandSwing = false;
                        break;
                    case Ability.WeaponHand.MainHand:
                        offHandSwing = false;
                        break;
                    case Ability.WeaponHand.OffHand:
                        offHandSwing = true;
                        break;
                    case Ability.WeaponHand.Both:
                        offHandSwing = false;
                        break;
                    case Ability.WeaponHand.Alternating:
                        break;
                }

                string animation = active.RollAnimation(stats.MainHandItemClass, stats.OffHandItemClass, offHandSwing);
                float speed = GetCastSpeed(active);
                anim.SetFloat("attackspeed", speed);
                anim.SetBool("channelling", active.Channelled);
                anim.SetTrigger(animation);
                swingTime += 1;
                patternPosition = 0;
                channelling = false;

                cooldowns[activeIndex] = 0;

                active.OnStartCasting
                (
                    new Ability.AbilityArgs
                    {
                        caster = caster,
                        target = target,
                        castTarget = castTarget,
                        throwTarget = throwTarget,
                        floorTarget = floorTarget,
                        swingTime = swingTime,
                        offHandSwing = offHandSwing,
                        patternPosition = patternPosition,
                        channelling = channelling,
                        objects = objects[activeIndex]
                    }
                );

                if (stats != null)
                    stats.OnAbilityStart(active, target, castTarget, throwTarget, floorTarget);
            }
        }
        else
        {
            if (channelling)
                swingTime -= Time.deltaTime * GetChannellingSpeed(Active);
            else
                swingTime -= Time.deltaTime * GetCastSpeed(Active);
        }
    }
    public enum CastOutcome
    {
        Valid,
        AbilityIndexOutOfRange,
        AbilityIndexMismatch,
        NullAbility,
        IncompatibleWeapon,
        OnCooldown,
        Casting,
        OutOfRange,
        NoTarget,
        InvalidTarget,
        InvalidPosition,
    }

    public void EndChannelling()
    {
        
        Ability active = Active;

        if (active == null)
            return;

        if (!active.Channelled)
            return;

        if (!channelling)
        {
            CancelActive();
            return;
        }

        active.OnEnd
        (
            new Ability.AbilityArgs
            {
                caster = caster,
                target = target,
                castTarget = castTarget,
                throwTarget = throwTarget,
                floorTarget = floorTarget,
                swingTime = swingTime,
                offHandSwing = offHandSwing,
                patternPosition = patternPosition,
                channelling = channelling,
                objects = objects[activeIndex]
            }
        );

        if (stats != null)
            stats.OnAbilityEnd(active, target, castTarget, throwTarget, floorTarget);

        queuedIndex = -1;
        activeIndex = -1;
        channelling = false;
        anim.SetBool("channelling", false);
        ToggleHandSwing();
    }

    public float Range(int abilityIndex)
    {
        if (this[abilityIndex] == null)
            return float.MaxValue;
        return abilities[abilityIndex].GetRange(offHandSwing, stats);
    }

    public Ability.AbilityType Type(int abilityIndex)
    {
        if (this[abilityIndex] == null)
        {
            return offHandSwing ?
                  stats.OffHandItemClass.StandardAbilityType()
                : stats.MainHandItemClass.StandardAbilityType();
        }
        return abilities[abilityIndex].Type;
    }

    public void CancelQueued()
    {
        queuedIndex = -1;
    }

    public void CancelActive()
    {
        Active?.OnCancel
        (
            new Ability.AbilityArgs
            {
                caster = caster,
                target = target,
                castTarget = castTarget,
                throwTarget = throwTarget,
                floorTarget = floorTarget,
                swingTime = swingTime,
                offHandSwing = offHandSwing,
                patternPosition = patternPosition,
                channelling = channelling,
                objects = objects[activeIndex]
            }
        );
        activeIndex = -1;
        swingTime = 0;
        channelling = false;
        anim.SetBool("channelling", false);
        anim.SetTrigger("cancel");
    }

    public void Channel(Unit target, Vector3 castTarget, Vector3 throwTarget, Vector3 floorTarget)
    {
        if (!channelling)
            return;
        this.target = target;
        this.castTarget = castTarget;
        this.throwTarget = throwTarget;
        this.floorTarget = floorTarget;
    }

    public CastOutcome Cast(int abilityIndex, Unit target, Vector3 castTarget, Vector3 throwTarget, Vector3 floorTarget, bool floorTargetValid, bool interrupt = false)
    {
        if (interrupt)
            CancelActive();

        if (abilityIndex < 0 || abilityIndex >= abilities.Length)
            return CastOutcome.AbilityIndexOutOfRange;

        CastOutcome check = CheckValid(abilityIndex, target, castTarget, throwTarget, floorTarget, floorTargetValid);
        if (check == CastOutcome.Valid)
        {
            queuedTarget = target;
            queuedCastTarget = castTarget;
            queuedThrowTarget = throwTarget;
            queuedFloorTarget = floorTarget;
            queuedIndex = abilityIndex;
        }
        else
        {
            ErrorMessage(check);
        }
        return check;
    }

    public void ActivateCooldown(int index)
    {
        if (index < 0 || index >= abilities.Length || abilities[index] == null)
            return;
        cooldowns[index] = 0;
    }

    public float CooldownElapsed(int index)
    {
        if (index < 0 || index >= abilities.Length || abilities[index] == null)
            return 0;
        return cooldowns[index];
    }

    public float CooldownMax(int index)
    {
        if (index < 0 || index >= abilities.Length || abilities[index] == null)
            return 0;
        return abilities[index].Cooldown;
    }

    public float CooldownElapsedFraction(int index)
    {
        if (index < 0 || index >= abilities.Length || abilities[index] == null)
            return 0;
        return cooldowns[index] / abilities[index].Cooldown;
    }
    public float CooldownRemaining(int index)
    {
        if (index < 0 || index >= abilities.Length || abilities[index] == null)
            return 0;
        return abilities[index].Cooldown - cooldowns[index];
    }
    public bool OnCooldown(int index)
    {
        if (index < 0 || index >= abilities.Length || abilities[index] == null)
            return false;
        return cooldowns[index] < abilities[index].Cooldown;
    }

    private bool ValidPosition(Vector3 targetPosition)
    {
        if (!NavMesh.SamplePosition(targetPosition, out NavMeshHit navMeshHit, 1.0f, NavMesh.AllAreas))
            return false;

        targetPosition = navMeshHit.position;

        if (Physics.Raycast(new Ray(transform.position, targetPosition - transform.position), (targetPosition - transform.position).magnitude, LayerMask.GetMask("Walls")))
            return false;

        return true;
    }

    private Ability.AbilityType AutoStandardAbilityType => offHandSwing ? stats.OffHandItemClass.StandardAbilityType() : stats.MainHandItemClass.StandardAbilityType();
    private CastOutcome CheckValid(int abilityIndex, Unit target, Vector3 castTarget, Vector3 throwTarget, Vector3 floorTarget, bool hasFloorTargetNavMeshSample)
    {
        if (abilityIndex < 0 || abilityIndex >= abilities.Length)
            return CastOutcome.AbilityIndexOutOfRange;

        Ability ability = abilities[abilityIndex];

        if (ability == null)
            return CastOutcome.NullAbility;

        if (OnCooldown(abilityIndex))
            return CastOutcome.OnCooldown;

        if (!ability.Compatible(stats.MainHandItemClass, stats.OffHandItemClass, offHandSwing ? stats.OffHandItemClass : stats.MainHandItemClass))
        {
            Debug.LogWarning("Incompatible Weapon: " + stats.MainHandItemClass + "/" + stats.OffHandItemClass);
            return CastOutcome.IncompatibleWeapon;
        }

        if (ability.Type == Ability.AbilityType.Targeted)
        {
            if (target == null)
                return CastOutcome.NoTarget;

            if (target.GetCurrentHealth() <= 0)
                return CastOutcome.InvalidTarget;

            if (!Ability.Filter(caster.GetFaction(), target.GetFaction(), ability.AbilityAffects))
                return CastOutcome.InvalidTarget;

            float range = Range(abilityIndex);

            if (SquareDistance(caster.GetCastPosition(), target.GetCenterPosition()) > range * range)
                return CastOutcome.OutOfRange;
        }

        else if (ability.Type == Ability.AbilityType.Thrown || (ability.Type == Ability.AbilityType.Auto && AutoStandardAbilityType == Ability.AbilityType.Thrown))
        {
            float range = Range(abilityIndex);

            if (SquareDistance(caster.GetCastPosition(), throwTarget) > range * range)
                return CastOutcome.OutOfRange;

            //Perhaps assume targetPositions are always valid...
            /*
            if (!NavMesh.SamplePosition(targetPosition, out NavMeshHit navMeshHit, 1.0f, NavMesh.AllAreas))
                return CastOutcome.InvalidPosition;

            targetPosition = navMeshHit.position;

            if (Physics.Raycast(new Ray(transform.position, targetPosition - transform.position), (targetPosition - transform.position).magnitude, LayerMask.GetMask("Walls")))
                return CastOutcome.InvalidPosition;
             */
        }
        else if (ability.Type == Ability.AbilityType.Place)
        {
            if (!hasFloorTargetNavMeshSample)
                return CastOutcome.InvalidPosition;

            float range = Range(abilityIndex);

            if (SquareDistance(caster.GetCastPosition(), floorTarget) > range * range)
                return CastOutcome.OutOfRange;
        }

        return CastOutcome.Valid;
    }
    private void ErrorMessage(CastOutcome outcome)
    {
        //        Debug.LogWarning(outcome.ToString().SplitCamelCase());    
        if (errorMessageDisplay)
            errorMessageDisplay.ErrorMessage(outcome.ToString().SplitCamelCase());
    }
    public static float SquareDistance(Vector3 a, Vector3 b)
    {
        return (b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y) + (b.z - a.z) * (b.z - a.z);
    }
    private float GetCastSpeed(Ability active)
    {
        if (active == null)
            return 1;

        if (active.Hand == Ability.WeaponHand.MainHand)
            return active.Speed * stats.MainHandAttacksPerSecond;

        if (active.Hand == Ability.WeaponHand.OffHand)
            return active.Speed * stats.OffHandAttacksPerSecond;

        if (active.Hand == Ability.WeaponHand.Alternating)
            return active.Speed * (offHandSwing ? stats.OffHandAttacksPerSecond : stats.MainHandAttacksPerSecond);

        if (active.Hand == Ability.WeaponHand.Both)
            return active.Speed * Mathf.Min(stats.MainHandAttacksPerSecond, stats.OffHandAttacksPerSecond);

        if (active.Hand == Ability.WeaponHand.Spell)
            return active.Speed * stats.CastSpeed;

        return active.Speed;
    }
    private float GetChannellingSpeed(Ability active)
    {
        if (active == null)
            return 1;

        if (!active.Channelled)
            return 1;

        if (active.Hand == Ability.WeaponHand.MainHand)
            return 1f / active.ChannellingTickInterval * stats.MainHandAttacksPerSecond;

        if (active.Hand == Ability.WeaponHand.OffHand)
            return 1f / active.ChannellingTickInterval * stats.OffHandAttacksPerSecond;

        if (active.Hand == Ability.WeaponHand.Alternating)
            return 1f / active.ChannellingTickInterval * (offHandSwing ? stats.OffHandAttacksPerSecond : stats.MainHandAttacksPerSecond);

        if (active.Hand == Ability.WeaponHand.Both)
            return 1f / active.ChannellingTickInterval * Mathf.Min(stats.MainHandAttacksPerSecond, stats.OffHandAttacksPerSecond);

        if (active.Hand == Ability.WeaponHand.Spell)
            return 1f / active.ChannellingTickInterval * stats.CastSpeed;

        return 1f / active.ChannellingTickInterval;
    }
}
