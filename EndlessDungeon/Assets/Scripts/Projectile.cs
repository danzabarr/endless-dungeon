using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{

    [SerializeField]
    private float life;
    [SerializeField]
    private LayerMask collisionMask;
    private Unit caster;
    private float velocity;
    private bool hit;
    [SerializeField]
    private ParticleSystem onCollision;

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
    private int debuffStacks;

    [SerializeField]
    private int debuffMaxStacks;

    [SerializeField]
    private float knockbackForce;

    [SerializeField]
    private bool knockbackDistanceAttenuate;

    [SerializeField]
    private bool knockbackAlignWithProjectile;

    public void Init(Unit caster, Vector3 direction, float velocity, Vector2 damage, Ability.DamageType damageType)
    {
        this.caster = caster;
        this.velocity = velocity;
        transform.position = caster.GetCastPosition();
        transform.rotation = Quaternion.LookRotation(direction);
        this.damage = damage;
        this.damageType = damageType;
    }
    public void Update()
    {
        if (!hit)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, velocity * Time.deltaTime, collisionMask, QueryTriggerInteraction.Ignore))
            {
                transform.position += transform.forward * (hitInfo.distance - 0.01f);
                OnTriggerEnter(hitInfo.collider);
            }
            else
            {
                transform.position += transform.forward * velocity * Time.deltaTime;
            }
        }
        life -= Time.deltaTime;

        if (life <= 0)
            Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (hit) return;
        if (collisionMask != (collisionMask | (1 << other.gameObject.layer))) return;


        hit = true;
        //Debug.Log("HIT " + other);
        if (onCollision)
        {
            if (onCollision)
                Instantiate(onCollision).transform.position = transform.position;
            Destroy(gameObject);


            List<Mob> targets = Level.Instance.MobsInRadius(transform.position, aoeRange, true);
            foreach(Mob target in targets)
            {
                float distance = Vector3.Distance(transform.position, target.GetCenterPosition());
                float attenuation = rangeAttenuation.Evaluate(distance / aoeRange);
                float dmg = damage.Roll() * attenuation;
                target.Damage(this, caster, damageType, dmg, true, false);

                if (debuff)
                    target.ApplyDebuff(caster, debuff, out _, out _, debuffUseResistances, debuffStacks, debuffMaxStacks);

                if (knockbackAlignWithProjectile)
                    target.Knockback(transform.position, transform.forward, knockbackForce, knockbackDistanceAttenuate);
                else
                    target.Knockback(transform.position, knockbackForce, knockbackDistanceAttenuate);
            }
        }
    }

    public virtual void OnDestroy()
    {

    }
}
