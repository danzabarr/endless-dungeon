﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour
{

    public enum Faction
    {
        Player,
        Enemy,
        Neutral
    }

    [SerializeField]
    protected Transform center;

    [SerializeField]
    protected Transform cast;

    [SerializeField]
    protected Transform mainHandWeapon;

    [SerializeField]
    protected Transform offHandWeapon;

    [SerializeField]
    protected Transform mainHand;

    [SerializeField]
    protected Transform offHand;

    [SerializeField]
    protected Transform spine;

    [SerializeField]
    protected Faction faction;

    protected Projectile cockedMHProjectile, cockedOHProjectile;
    protected Throwable cockedMHThrowable, cockedOHThrowable;

    public Projectile CockedMHProjectile
    {
        get => cockedMHProjectile;
        set
        {
            if (cockedMHProjectile)
                Destroy(cockedMHProjectile.gameObject);
            if (cockedMHThrowable)
                Destroy(cockedMHThrowable.gameObject);
            cockedMHProjectile = value;
            if (cockedMHProjectile)
                cockedMHProjectile.transform.SetParent(mainHand, false);
        }
    }
    public Projectile CockedOHProjectile
    {
        get => cockedMHProjectile;
        set
        {
            if (cockedOHProjectile)
                Destroy(cockedOHProjectile.gameObject);
            if (cockedOHThrowable)
                Destroy(cockedOHThrowable.gameObject);
            cockedOHProjectile = value;
            if (cockedOHProjectile)
                cockedOHProjectile.transform.SetParent(offHand, false);
        }
    }

    public Throwable CockedMHThrowable
    {
        get => cockedMHThrowable;
        set
        {
            if (cockedMHProjectile)
                Destroy(cockedMHProjectile.gameObject);
            if (cockedMHThrowable)
                Destroy(cockedMHThrowable.gameObject);
            cockedMHThrowable = value;
            if (cockedMHThrowable)
                cockedMHThrowable.transform.SetParent(mainHand, false);
        }
    }
    public Throwable CockedOHThrowable
    {
        get => cockedOHThrowable;
        set
        {
            if (cockedOHProjectile)
                Destroy(cockedOHProjectile.gameObject);
            if (cockedOHThrowable)
                Destroy(cockedOHThrowable.gameObject);
            cockedOHThrowable = value;
            if (cockedOHThrowable)
                cockedOHThrowable.transform.SetParent(offHand, false);
        }
    }

    public void ShootMHProjectile(Vector3 direction, float velocity, Vector2 damage, Ability.DamageType damageType, Ability.Affects affects)
    {
        if (!cockedMHProjectile)
            return;
        cockedMHProjectile.Shoot(this, direction, velocity, damage, damageType, affects);
        cockedMHProjectile = null;
    }

    public void ShootOHProjectile(Vector3 direction, float velocity, Vector2 damage, Ability.DamageType damageType, Ability.Affects affects)
    {
        if (!cockedOHProjectile)
            return;
        cockedOHProjectile.Shoot(this, direction, velocity, damage, damageType, affects);
        cockedOHProjectile = null;
    }

    protected float health;

    [SerializeField]
    [Range(0, 1)]
    protected float stunThreshold = 0.1f;

    [SerializeField]
    [Range(0, 0.9999999f)]
    protected float knockbackDampening = 0.5f;

    [SerializeField]
    protected float knockbackMultiplier = 1;

    protected List<BuffInstance> buffs = new List<BuffInstance>();
    protected List<BuffInstance> debuffs = new List<BuffInstance>();

    protected float stunTimer;

    [SerializeField]
    protected Color color;

    [SerializeField]
    protected Renderer[] mainMaterialRenderers;

    private Material material;
    public Stats Stats { get; private set; }
    public Animator Animator { get; private set; }
    public AbilityManager Abilities { get; private set; }
    public NavMeshAgent Agent { get; private set; }

    public event GameEventSystem.Handler DeathEvent;

    [SerializeField]
    protected AbilityAnimation[] blockAnimations;

    private string[] offHandBlockTriggers;
    private string[] mainHandBlockTriggers;
    public virtual void Awake()
    {
        Stats = GetComponent<Stats>();
        Animator = GetComponent<Animator>();
        Abilities = GetComponent<AbilityManager>();
        Agent = GetComponent<NavMeshAgent>();

        DeathEvent += GameEventSystem.UnitDeaths;
    }

    public virtual void Start()
    {
        health = GetMaxHealth();

        material = new Material(mainMaterialRenderers[0].sharedMaterial);

        foreach (Renderer renderer in mainMaterialRenderers)
            renderer.sharedMaterial = material;
    }

    protected virtual void Update()
    {
        UpdateBuffs();
        stunTimer = Mathf.Max(0, stunTimer - Time.deltaTime);
    }

    public virtual void FixedUpdate()
    {
        if (knockbackForce > 1f)
        {
            Agent.updateRotation = false;
            Agent.velocity = knockbackDirection * knockbackForce;
            knockbackForce *= knockbackDampening;
        }
        else
        {
            knockbackForce = 0;
            Agent.updateRotation = true;
        }


        /*
        if (agent.velocity.sqrMagnitude < 1)
        {
            agent.enabled = false;
            obstacle.enabled = true;
        }
        else
        {
            obstacle.enabled = false;
            agent.enabled = true;
        }
        */
    }

    public void IdentifyBlockAnimations()
    {
        if (Stats == null)
            Stats = GetComponent<Stats>();
        List<string> offHandBlockTriggers = new List<string>();
        List<string> mainHandBlockTriggers = new List<string>();
        foreach (AbilityAnimation a in blockAnimations)
        {
            //Ignore animations whose main hand requirements aren't met by the main hand.
            if (!a.mainHand.IsCompatible(Stats.MainHandItemClass))
                continue;

            //Ignore animations whose off hand requirements aren't met by the off hand.
            if (!a.offHand.IsCompatible(Stats.OffHandItemClass))
                continue;

            //Ignore animations for the wrong hand.
            if (a.offHandSwing)
                offHandBlockTriggers.Add(a.trigger);
            else
                mainHandBlockTriggers.Add(a.trigger);
        }
        this.offHandBlockTriggers = offHandBlockTriggers.ToArray();
        this.mainHandBlockTriggers = mainHandBlockTriggers.ToArray();
    }

    public Color Color
    {
        get => color;
        set
        {
            color = value;
            foreach (Renderer renderer in mainMaterialRenderers)
                renderer.sharedMaterial.color = color;
        }
    }

    private Coroutine colorAnimation;

    public void SetColor(Color color, float transitionDuration)
    {
        if (colorAnimation != null) StopCoroutine(colorAnimation);
        colorAnimation = StartCoroutine(ToColor(color, transitionDuration));
    }

    private IEnumerator ToColor(Color color, float transitionDuration)
    {
        Color original = Color;
        for (float t = 0; t < transitionDuration; t += Time.deltaTime)
        {
            Color = Color.Lerp(original, color, t);
            yield return null;
        }
        Color = color;
    }

    public void OnDestroy()
    {
        Destroy(material);
    }

    protected void UpdateBuffs()
    {
        foreach (BuffInstance buff in buffs)
            buff.UpdateTimer();
        foreach (BuffInstance debuff in debuffs)
            debuff.UpdateTimer();

        for (int i = buffs.Count - 1; i >= 0; i--)
            if (buffs[i] == null || buffs[i].Finished) buffs.RemoveAt(i);

        for (int i = debuffs.Count - 1; i >= 0; i--)
            if (debuffs[i] == null || debuffs[i].Finished) debuffs.RemoveAt(i);
    }

    public virtual GameObject GetGameObject() => gameObject;
    public virtual Transform GetGround() => transform;
    public virtual Vector3 GetGroundPosition() => transform.position;
    public virtual Quaternion GetRotation() => transform.rotation;
    public virtual Transform GetCast() => cast;
    public virtual Vector3 GetCastPosition() => cast.position;
    public virtual Transform GetCenter() => center;
    public virtual Vector3 GetCenterPosition() => center.position;
    public virtual Transform GetMainHand() => mainHandWeapon;
    public virtual Transform GetOffHand() => offHandWeapon;

    private Vector3 knockbackDirection;
    private float knockbackForce;

    public void Knockback(Vector3 direction, float force)
    {
        force *= knockbackMultiplier;
        if (force <= 0)
            return;
        Abilities.CancelActive();
        Vector3 knockback = knockbackDirection * knockbackForce + direction.normalized * force;
        knockbackForce = knockback.magnitude;
        knockbackDirection = knockback / knockbackForce;
        Agent.updateRotation = false;
        Agent.velocity = knockbackDirection * knockbackForce;
    }

    public void Knockback(Vector3 origin, float force, bool distanceAttenuate)
    {
        Vector3 delta = GetCenterPosition() - origin;
        if (distanceAttenuate) force /= delta.magnitude;
        Knockback(delta, force);
    }

    public void Knockback(Vector3 origin, Vector3 direction, float force, bool distanceAttenuate)
    {
        force *= knockbackMultiplier;
        if (force <= 0)
            return;
        Abilities.CancelActive();
        if (distanceAttenuate) force /= (GetCenterPosition() - origin).magnitude;
        Vector3 knockback = knockbackDirection * knockbackForce + direction.normalized * force;
        knockbackForce = knockback.magnitude;
        knockbackDirection = knockback / knockbackForce;
        Agent.updateRotation = false;
        Agent.velocity = knockbackDirection * knockbackForce;
    }

    public virtual void Kill()
    {
        if (health > 0)
        {
            health = 0;
            OnDeath();
        }
    }
    public virtual void OnDeath()
    {
        
        Stats.OnDeath();
        cast.DetachChildren();
        center.DetachChildren();
        DeathEvent.Invoke(new GameEventSystem.EventArgs(this));
        //Debug.Log("Urrrkkk.... I'm dying...");
    }

    public virtual void Teleport(Vector3 position)
    {
        transform.position = position;
    }

    public virtual void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    public Faction GetFaction()
    {
        return faction;
    }

    public virtual void SetFaction(Faction faction)
    {
        this.faction = faction;
    }

    public virtual float GetCurrentHealth()
    {
        return health;
    }

    public virtual float GetMaxHealth()
    {
        return Stats.MaxHealth;
    }

    public int IntegerHealth => Mathf.CeilToInt(GetCurrentHealth());
    public int IntegerMaxHealth => Mathf.CeilToInt(GetMaxHealth());
    public float FractionHealth => GetCurrentHealth() / GetMaxHealth();
    public float PercentHealth => GetCurrentHealth() / GetMaxHealth() * 100;
    public bool Stunned => stunTimer > 0;
    public bool InFront(Vector3 position)
    {
        return Vector3.Dot(transform.forward, position - transform.position) >= 0;
    }
    private string RollBlockAnimation(bool offHandSwing)
    {
        return "";
    }
    public void Damage(object source, Vector3 sourcePosition, Unit attacker, Ability.DamageType type, float amount, bool canStun, bool blockable)
    {
        Damage(source, sourcePosition, attacker, type, amount, canStun, blockable, out _, out _, out _, out _);
    }
    public void Damage(object source, Vector3 sourcePosition, Unit attacker, Ability.DamageType type, float amount, bool canStun, bool blockable, out float dealt, out bool blocked, out bool killingBlow, out bool deathAverted)
    {
        dealt = amount * (1 - Stats.Mitigation(type));

        bool infront = InFront(sourcePosition);

        bool offHandBlock = infront && blockable && !Stunned && Stats.OffHandItemClass.HasBlock() && Stats.OffHandBlockChance > Random.value;
        bool mainHandBlock = !offHandBlock && infront && blockable && !Stunned && Stats.MainHandItemClass.HasBlock() && Stats.MainHandBlockChance > Random.value;

        blocked = offHandBlock || mainHandBlock;

        killingBlow = false;
        deathAverted = false;

        if (canStun && dealt / Stats.MaxHealth > stunThreshold)
        {
            Abilities.CancelActive();
            
            Animator.SetFloat("hitrecoveryspeed", 1f / stunTimer);
            if (offHandBlock)
            {
                if (offHandBlockTriggers != null && offHandBlockTriggers.Length > 0)
                {
                    Animator.SetFloat("hitrecoveryspeed", Stats.OffHandBlockSpeed);
                    Animator.SetTrigger(offHandBlockTriggers[Random.Range(0, offHandBlockTriggers.Length)]);
                }
                stunTimer = Stats.OffHandBlockDuration;
                
            }
            else if (mainHandBlock)
            {
                if (mainHandBlockTriggers != null && mainHandBlockTriggers.Length > 0)
                {
                    Animator.SetFloat("hitrecoveryspeed", Stats.MainHandBlockSpeed);
                    Animator.SetTrigger(mainHandBlockTriggers[Random.Range(0, mainHandBlockTriggers.Length)]);
                }
                stunTimer = Stats.MainHandBlockDuration;
            }
            else
            {
                Animator.SetFloat("hitrecoveryspeed", Stats.HitRecoverySpeed);
                Animator.SetTrigger("hit");
                stunTimer = Stats.HitRecoveryDuration;
            }
        }

        if (blocked)
        {
            Stats.OnBlock(source, attacker, dealt, type);
        }
        else
        {
            FloatingCombatTextManager.Add(GetCenterPosition(), dealt, 10, faction == Faction.Player ? Color.red : Color.white);
            dealt = health - Mathf.Clamp(health - dealt, 0, GetMaxHealth());
            killingBlow = health > 0 && health - dealt <= 0;
            health -= dealt;
            if (dealt > 0)
            {
                OnTakeDamage(attacker, type, dealt);
                attacker.Stats.OnDealDamage(source, this, dealt, type);
                Stats.OnReceiveDamage(source, attacker, dealt, type);
            }

            if (killingBlow)
            {
                if (TryAvertDeath(source, attacker, type, amount, out float healthFraction) && healthFraction > 0)
                {
                    Heal(this, GetMaxHealth() * healthFraction - health);
                    deathAverted = true;
                }
                else
                {
                    attacker.Stats.OnDealKillingBlow(source, this);
                    Stats.OnReceiveKillingBlow(source, attacker);
                    OnDeath();
                }
            }
        }
    }

    public virtual void OnTakeDamage(Unit attacker, Ability.DamageType type, float amount)
    {

    }

    private bool TryAvertDeath(object source, Unit attacker, Ability.DamageType type, float amount, out float healthFraction)
    {

        healthFraction = 0;
        return false;
    }

    public virtual void Hit()
    {
        stunTimer = Stats.HitRecoveryDuration;
        Animator.SetFloat("hitrecoveryspeed", 1f / stunTimer);
        Animator.SetTrigger("hit");
    }

    public virtual void Heal(Unit caster, float amount)
    {
        FloatingCombatTextManager.Add(GetCenterPosition(), amount, 10, Color.green);
        health = Mathf.Clamp(health + amount, 0, GetMaxHealth());
    }

    public virtual bool ApplyBuff(Unit caster, Buff buff, out BuffInstance instance, out bool newInstance, int stacks = 1, int maxStacks = 0)
    {
        if (maxStacks > 0)
            stacks = Mathf.Min(stacks, maxStacks);

        foreach (BuffInstance i in buffs)
            if (i.IsMatchToOverwrite(caster, this, buff))
            {
                newInstance = false;
                instance = null;
                if (maxStacks <= 0 || i.Stacks <= maxStacks)
                {
                    int add = stacks;
                    if (maxStacks > 0)
                        add = Mathf.Min(maxStacks, i.Stacks + add) - i.Stacks;
                    i.AddStack(add);
                    instance = i;
                }
                return true;
            }

        //TODO track and remove previous application if buff is PerCaster

        buffs.Add(instance = new BuffInstance(caster, this, buff, stacks));
        buff.OnApply(caster, this, stacks, 0);
        newInstance = true;
        return true;
    }

    public virtual bool ApplyDebuff(Unit caster, Buff debuff, out BuffInstance instance, out bool newInstance, bool useResistances, int stacks = 1, int maxStacks = 0)
    {
        
        if (useResistances)
        {
            float resist = Stats.Mitigation(debuff.DamageType);

            if (resist > 0)
            {
                if (resist < 1)
                {
                    int s = stacks;
                    for (int i = 0; i < s; i++)
                        if (resist > Random.value)
                            stacks--;
                }
                else
                    stacks = 0;

                if (stacks <= 0)
                {
                    instance = null;
                    newInstance = false;
                    return false;
                }
            }
        }

        if (maxStacks > 0)
            stacks = Mathf.Min(stacks, maxStacks);

        foreach (BuffInstance i in debuffs)
            if (i.IsMatchToOverwrite(caster, this, debuff))
            {
                newInstance = false;
                instance = null;
                if (maxStacks <= 0 || i.Stacks <= maxStacks)
                {
                    int add = stacks;
                    if (maxStacks > 0)
                        add = Mathf.Min(maxStacks, i.Stacks + add) - i.Stacks;
                    i.AddStack(add);
                    instance = i;
                }
                return true;
            }

        //TODO track and remove previous application if buff is PerCaster

        debuffs.Add(instance = new BuffInstance(caster, this, debuff, stacks));
        debuff.OnApply(caster, this, stacks, 0);
        newInstance = true;
        return true;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Unit>())
        {
            Agent.velocity = Vector3.zero;
        }
    }
    public virtual void Interact(Interactive interactive)
    {
        
    }

    public virtual void YawToDirection(Vector3 direction)
    {
        
    }

    public virtual void PitchToDirection(Vector3 direction)
    {

    }


    public virtual void WalkTo(Vector3 position)
    {
        
    }

    public virtual void Hit(float duration)
    {
        
    }
}
