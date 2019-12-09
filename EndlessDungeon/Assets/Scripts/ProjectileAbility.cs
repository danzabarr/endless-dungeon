using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : Ability
{
    public override void OnPulse(AbilityArgs args)
    {
        Vector3 direction = args.target == null ? (args.castTarget - args.caster.GetCastPosition()).normalized : (args.target.GetCenterPosition() - args.caster.GetCastPosition()).normalized;
        Instantiate(Projectile, args.objects.transform).Shoot(args.caster, direction, projectileSpeed, GetDamage(args.offHandSwing, args.caster.Stats), DmgType, AbilityAffects);
    }
}
