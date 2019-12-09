using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableAbility : Ability
{
    public override void OnPulse(AbilityArgs args)
    {
        Instantiate(Throwable, args.caster.GetCastPosition(), args.caster.GetCast().rotation * Quaternion.Euler(-45, 0, 0), args.objects.transform).ThrowAt(args.caster, args.throwTarget, GetDamage(args.offHandSwing, args.caster.Stats), DmgType);
    }
}
