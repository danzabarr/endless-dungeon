using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : Ability
{
    public override void OnPulse
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
        SnapShot snapshot,
        GameObject objects
    )
    {
        Vector3 direction = target == null ? (castTarget - caster.GetCastPosition()).normalized : (target.GetCenterPosition() - caster.GetCastPosition()).normalized;
        Instantiate(Projectile, objects.transform).Init(caster, direction, projectileSpeed, GetDamage(offHandSwing, snapshot), DmgType, AbilityAffects);
    }
}
