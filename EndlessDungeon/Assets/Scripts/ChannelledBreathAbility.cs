using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelledBreathAbility : Ability
{
    [SerializeField]
    private Buff debuff;

    [SerializeField]
    private ParticleSystem particles;

    public override void OnStartChannelling(AbilityArgs args)
    {
        Transform cast = args.caster.GetCast();
        Instantiate(particles, cast.position + cast.forward * 1f, cast.rotation, cast).tag = "Ability.ChannelledBreathAbility";
        Vector3 castDirection = args.target == null ? (args.castTarget - args.caster.GetCastPosition()).normalized : (args.target.GetCenterPosition() - args.caster.GetCastPosition()).normalized;
        Vector2 damage = GetDamage(args.offHandSwing, args.caster.Stats);
        List<Unit> targets = GetTargets(args.caster, args.target, castDirection, args.offHandSwing);


        foreach (Unit hp in targets)
        {
            hp.Damage(this, args.caster.GetCastPosition(), args.caster, DamageType.Physical, damage.Roll(), true, true);
            //hp.Knockback(castDirection, 100 / (Vector3.Distance(caster.GetCastPosition(), hp.GetCenterPosition())));
            hp.ApplyDebuff(args.caster, debuff, out _, out _, true, 1, 10);
        }
    }

    public override void OnChannellingPulse(AbilityArgs args)
    {

        Vector3 castDirection = args.target == null ? (args.castTarget - args.caster.GetCastPosition()).normalized : (args.target.GetCenterPosition() - args.caster.GetCastPosition()).normalized;
        Vector2 damage = GetDamage(args.offHandSwing, args.caster.Stats);
        List<Unit> targets = GetTargets(args.caster, args.target, castDirection, args.offHandSwing);
        foreach (Unit hp in targets)
        {
            hp.Damage(this, args.caster.GetCastPosition(), args.caster, DamageType.Physical, damage.Roll(), true, true);
            //hp.Knockback(castDirection, 100 / (Vector3.Distance(caster.GetCastPosition(), hp.GetCenterPosition())));
            hp.ApplyDebuff(args.caster, debuff, out _, out _, true, 1, 10);
        }
    }

    public override void OnEnd(AbilityArgs args)
    {
        foreach (ParticleSystem child in args.caster.GetCast().GetComponentsInChildren<ParticleSystem>())
        {
            if (child.CompareTag("Ability.ChannelledBreathAbility"))
            {
                child.Stop();
                child.transform.SetParent(null);
            }
        }
    }

    

}
