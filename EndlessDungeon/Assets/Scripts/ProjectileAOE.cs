using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ProjectileAOE : Projectile
{
    [SerializeField]
    private float aoeRange;

    [SerializeField]
    private AnimationCurve rangeAttenuation;
    public override void OnCollision(Collider other)
    {
        hit = true;
        
        if (onCollision)
            Instantiate(onCollision).transform.position = transform.position;
        Destroy(gameObject);


        List<Unit> targets = Level.Instance.UnitsInRadius(transform.position, aoeRange, true, faction, affects);
        Debug.Log(aoeRange);
        Debug.Log(faction);
        Debug.Log(affects);
        Debug.Log(targets.ToArray());
        foreach(Unit target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.GetCenterPosition());
            float attenuation = rangeAttenuation.Evaluate(distance / aoeRange);
            float dmg = damage.Roll() * attenuation;
            target.Damage(this, transform.position, caster, damageType, dmg, true, false);

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
