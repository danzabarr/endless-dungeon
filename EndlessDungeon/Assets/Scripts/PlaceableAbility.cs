using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableAbility : Ability
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
        GameObject objects
    )
        {
        
        Instantiate(Placeable, floorTarget, Quaternion.identity, objects.transform).Init(caster, GetDamage(offHandSwing, caster.Stats), DmgType);
    }
}
