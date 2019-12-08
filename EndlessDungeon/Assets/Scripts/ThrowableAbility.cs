using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableAbility : Ability
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
        bool channelled,
        GameObject objects
    )
    {
        Instantiate(Throwable, caster.GetCastPosition(), caster.GetCast().rotation * Quaternion.Euler(-45, 0, 0), objects.transform).ThrowAt(caster, throwTarget, GetDamage(offHandSwing, caster.Stats), DmgType);
    }
}
