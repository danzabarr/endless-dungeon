using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDamageOverTime : Buff
{
    public Vector2 damageAmount;
    public float additionalStackDamageMultiplier;

    public override void OnTick
        (
            Unit caster,
            Unit target,
            int stacks,
            float elapsedTime
        )
    {
        target.Damage(this, target.GetCenterPosition(), caster, DamageType, damageAmount.Roll() * (1 + (stacks - 1) * additionalStackDamageMultiplier), false, false);
    }
}
