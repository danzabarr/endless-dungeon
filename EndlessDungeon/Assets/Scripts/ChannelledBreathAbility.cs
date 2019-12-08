using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelledBreathAbility : Ability
{
    [SerializeField]
    private Buff debuff;

    [SerializeField]
    private ParticleSystem particles;

    public override void OnStartChannelling
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
        Transform cast = caster.GetCast();
        Instantiate(particles, cast.position + cast.forward * 1f, cast.rotation, cast).tag = "Ability.ChannelledBreathAbility";
        Vector3 castDirection = target == null ? (castTarget - caster.GetCastPosition()).normalized : (target.GetCenterPosition() - caster.GetCastPosition()).normalized;
        Vector2 damage = GetDamage(offHandSwing, caster.Stats);
        List<Unit> targets = GetTargets(caster, target, castDirection, offHandSwing);


        foreach (Unit hp in targets)
        {
            hp.Damage(this, caster.GetCastPosition(), caster, DamageType.Physical, damage.Roll(), true, true);
            //hp.Knockback(castDirection, 100 / (Vector3.Distance(caster.GetCastPosition(), hp.GetCenterPosition())));
            hp.ApplyDebuff(caster, debuff, out _, out _, true, 1, 10);
        }
    }

    public override void OnChannellingPulse
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
        Vector2 damage = GetDamage(offHandSwing, caster.Stats);
        List<Unit> targets = GetTargets(caster, target, castDirection, offHandSwing);
        foreach (Unit hp in targets)
        {
            hp.Damage(this, caster.GetCastPosition(), caster, DamageType.Physical, damage.Roll(), true, true);
            //hp.Knockback(castDirection, 100 / (Vector3.Distance(caster.GetCastPosition(), hp.GetCenterPosition())));
            hp.ApplyDebuff(caster, debuff, out _, out _, true, 1, 10);
        }
    }

    public override void OnEnd
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
        foreach (ParticleSystem child in caster.GetCast().GetComponentsInChildren<ParticleSystem>())
        {
            if (child.CompareTag("Ability.ChannelledBreathAbility"))
            {
                child.Stop();
                child.transform.SetParent(null);
            }
        }
    }

    

}
