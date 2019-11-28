using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEDamage : Placeable
{
    [SerializeField]
    protected float aoeRange;

    [SerializeField]
    protected AnimationCurve attenuation;

    [SerializeField]
    [EnumFlags]
    protected Ability.Affects affects;

    public override void Tick()
    {
        foreach (Unit unit in Level.Instance.UnitsInRadius(transform.position, aoeRange, true, casterFaction, affects))
        {
            float distance = Vector3.Distance(transform.position, unit.GetCenterPosition());
            float atten = attenuation.Evaluate(Mathf.Clamp(distance / aoeRange, 0, 1));
            unit.Damage(this, caster, damageType, damage.Roll() * atten, false, false);
        }
    }
}
