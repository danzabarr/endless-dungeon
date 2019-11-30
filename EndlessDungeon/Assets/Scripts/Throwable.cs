using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Throwable : MonoBehaviour
{
    [SerializeField]
    private new Rigidbody rigidbody;

    [SerializeField]
    private float force;

    [SerializeField]
    private bool loopingArc;

    [SerializeField]
    private float pitch;

    [SerializeField]
    private LayerMask collisionMask;


    [SerializeField]
    private ParticleSystem onCollision;

    [SerializeField]
    private Placeable placeable;

    private Unit caster;

    private Vector2 damage;
    private Ability.DamageType damageType;

    [SerializeField]
    private bool aoe;

    [SerializeField]
    private float aoeRange;

    [SerializeField]
    private AnimationCurve rangeAttenuation;

    [SerializeField]
    private Buff debuff;

    [SerializeField]
    private bool debuffUseResistances;

    [SerializeField]
    private int debuffStacks = 1;

    [SerializeField]
    private int debuffMaxStacks;

    private bool triggered;

    public void ThrowAt(Unit caster, Vector3 target, Vector2 damage, Ability.DamageType damageType)
    {
        this.caster = caster;
        this.damage = damage;
        this.damageType = damageType;
        transform.position = caster.GetCastPosition();

        Ballistics.SolveArcPitch(transform.position, target, pitch, out Vector3 projectileVelocity);
        rigidbody.AddForce(projectileVelocity, ForceMode.VelocityChange);
        Physics.IgnoreCollision(GetComponent<Collider>(), (caster as MonoBehaviour).GetComponent<Collider>());
        //Ballistics.SolveArcVector(transform.position, force, target, -Physics.gravity.y, out Vector3 s0, out Vector3 s1);
        //rigidbody.AddForce(loopingArc ? s1 : s0, ForceMode.VelocityChange);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collisionMask != (collisionMask | (1 << collision.gameObject.layer))) return;
        if (collision.gameObject.GetComponent<Unit>() == caster)
            return;
        Trigger();
    }

    public virtual void Trigger()
    {
        if (triggered)
            return;
        triggered = true;

        if (onCollision)
        {
            Instantiate(onCollision).transform.position = transform.position;
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit navMeshHit, 5.0f, NavMesh.AllAreas))
            {
                Instantiate(placeable, navMeshHit.position, Quaternion.identity, transform.parent).Init(caster, damage, damageType);
            }
        }
        Destroy(gameObject);


        List<Mob> targets = Level.Instance.MobsInRadius(transform.position, aoeRange, true);
        foreach (Mob target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.GetCenterPosition());
            float attenuation = rangeAttenuation.Evaluate(distance / aoeRange);
            float dmg = damage.Roll() * attenuation;
            target.Damage(this, transform.position, caster, damageType, dmg, true, false);
            target.ApplyDebuff(caster, debuff, out _, out _, debuffUseResistances, debuffStacks, debuffMaxStacks);
        }
    }
}
