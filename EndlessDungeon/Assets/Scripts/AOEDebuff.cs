using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEDebuff : Placeable
{
    [SerializeField]
    protected Buff debuff;

    [SerializeField]
    protected float aoeRange;

    [SerializeField]
    protected bool useResistances;

    [SerializeField]
    protected int stacksPerTick = 1;

    [SerializeField]
    protected int maxStacks;

    [SerializeField]
    [EnumFlags]
    protected Ability.Affects affects;

    public override void Tick()
    {
        foreach (Unit units in Level.Instance.UnitsInRadius(transform.position, aoeRange, true, casterFaction, affects))
        {
            units.ApplyDebuff(caster, debuff, out _, out _, useResistances, stacksPerTick, maxStacks);
        }
    }
}
