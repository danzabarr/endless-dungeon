using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    protected float life;
    [SerializeField]
    protected LayerMask collisionMask;
    [SerializeField]
    protected Transform collisionPoint;

    protected Unit caster;
    protected float velocity;
    protected bool shot, hit;

    [SerializeField]
    protected Unit.Faction faction;

    [SerializeField]
    [EnumFlags]
    protected Ability.Affects affects;

    [SerializeField]
    protected ParticleSystem onCollision;

    //Damage and type gets overwritten in Init.
    [SerializeField]
    protected Vector2 damage;
    [SerializeField]
    protected Ability.DamageType damageType;

    [SerializeField]
    protected float knockbackForce;
    [SerializeField]
    protected bool knockbackDistanceAttenuate;
    [SerializeField]
    protected bool knockbackAlignWithProjectile;

    [SerializeField]
    protected Buff debuff;
    [SerializeField]
    protected bool debuffUseResistances;
    [SerializeField]
    protected int debuffStacks;
    [SerializeField]
    protected int debuffMaxStacks;

    public void Shoot(Vector3 position, Vector3 velocity)
    {
        transform.parent = null;
        this.velocity = velocity.magnitude;
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(velocity / this.velocity);
        shot = true;
    }
    public void Shoot(Unit caster, Vector3 direction, float velocity)
    {
        transform.parent = null;
        Physics.IgnoreCollision(GetComponent<Collider>(), caster.GetComponent<Collider>());
        this.caster = caster;
        faction = caster.GetFaction();
        this.velocity = velocity;
        transform.position = caster.GetCastPosition();
        transform.rotation = Quaternion.LookRotation(direction);
        shot = true;
    }
    public void Shoot(Unit caster, Vector3 direction, float velocity, Vector2 damage, Ability.DamageType damageType, Ability.Affects affects)
    {
        transform.parent = null;
        Physics.IgnoreCollision(GetComponent<Collider>(), caster.GetComponent<Collider>());
        this.caster = caster;
        faction = caster.GetFaction();
        this.affects = affects;
        this.velocity = velocity;
        transform.position = caster.GetMainHand().position;
        transform.rotation = Quaternion.LookRotation(direction);
        this.damage = damage;
        this.damageType = damageType;
        shot = true;
    }
    public void Update()
    {
        if (!shot)
            return;
        if (!hit)
        {
            if (Physics.Raycast(collisionPoint.position, transform.forward, out RaycastHit hitInfo, velocity * Time.deltaTime, collisionMask, QueryTriggerInteraction.Ignore) && CheckCollision(hitInfo.collider))
            {
                transform.position += transform.forward * (hitInfo.distance - 0.01f);
                OnCollision(hitInfo.collider);
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

    public bool CheckCollision(Collider other)
    {
        if (other == null) return false;
        if (other.isTrigger) return false;
        if (hit) return false;
        if (collisionMask != (collisionMask | (1 << other.gameObject.layer))) return false;
        if (other == caster.GetComponent<Collider>()) return false;
        return true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (CheckCollision(other))
            OnCollision(other);
    }
    public virtual void OnCollision(Collider other)
    {

        hit = true;
        if (onCollision)
            Instantiate(onCollision).transform.position = transform.position;
        Destroy(gameObject);

        Unit target = other.GetComponent<Unit>();
        if (target)
        {
            target.Damage(this, transform.position, caster, damageType, damage.Roll(), true, false);

            if (debuff)
                target.ApplyDebuff(caster, debuff, out _, out _, debuffUseResistances, debuffStacks, debuffMaxStacks);

            if (knockbackForce > 0)
            {
                if (knockbackAlignWithProjectile)
                    target.Knockback(transform.position, transform.forward, knockbackForce, knockbackDistanceAttenuate);
                else
                    target.Knockback(transform.position, knockbackForce, knockbackDistanceAttenuate);
            }
        }
    }
}
