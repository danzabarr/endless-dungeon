using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    [SerializeField]
    protected float duration;

    protected float elapsed, tickTimer;

    [SerializeField]
    protected float tickInterval;

    [SerializeField]
    protected ParticleSystem particles;

    protected Vector2 damage;
    protected Ability.DamageType damageType;

    protected Unit caster;
    protected Unit.Faction casterFaction;

    public void Init(Unit caster, Vector2 damage, Ability.DamageType damageType)
    {
        this.caster = caster;
        casterFaction = caster.GetFaction();
        this.damage = damage;
        this.damageType = damageType;
    }

    public void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
        {

            particles.transform.SetParent(null);
            particles.Stop();
            Destroy(gameObject);
            return;
        }

        if (tickInterval > 0)
        {
            tickTimer += Time.deltaTime;

            while(tickTimer >= tickInterval)
            {
                tickTimer -= tickInterval;
                Tick();        
            }
        }
    }

    public virtual void Tick()
    {
        
    }
}
