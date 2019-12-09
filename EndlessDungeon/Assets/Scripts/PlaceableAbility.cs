using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableAbility : Ability
{
    public override void OnPulse(AbilityArgs args)
    {
        Instantiate(Placeable, args.floorTarget, Quaternion.identity, args.objects.transform).Init(args.caster, GetDamage(args.offHandSwing, args.caster.Stats), DmgType);
    }
}
