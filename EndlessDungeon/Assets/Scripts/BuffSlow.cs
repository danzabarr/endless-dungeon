using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSlow : Buff
{

    public Color color;
    public float colorTransitionDurationIn;
    public float colorTransitionDurationOut;
    public float bonusWalkSpeed;
    public float bonusAttackSpeed;

    public override void OnApply
        (
            Unit caster,
            Unit target,
            int stacks,
            float elapsedTime
        )
    {
        target.SetColor(color, colorTransitionDurationIn);
        target.Stats.bonusWalkSpeed += bonusWalkSpeed * stacks;
        target.Stats.bonusAttackSpeed += bonusAttackSpeed * stacks;
    }

    public override void OnAddStack(Unit caster, Unit target, int stacks, int stacksAdded, float elapsedTime)
    {
        target.Stats.bonusWalkSpeed += bonusWalkSpeed * stacksAdded;
        target.Stats.bonusAttackSpeed += bonusAttackSpeed * stacksAdded;
    }

    public override void OnRemoveStack(Unit remover, Unit caster, Unit target, int stacks, int stacksRemoved, float elapsedTime)
    {
        target.Stats.bonusWalkSpeed -= bonusWalkSpeed * stacksRemoved;
        target.Stats.bonusAttackSpeed -= bonusAttackSpeed * stacksRemoved;
    }

    public override void OnExpire
        (
            Unit caster,
            Unit target,
            int stacks,
            float elapsedTime
        )
    {
        target.Stats.bonusWalkSpeed -= bonusWalkSpeed * stacks;
        target.Stats.bonusAttackSpeed -= bonusAttackSpeed * stacks;
        target.SetColor(Color.clear, colorTransitionDurationOut);
    }

    public override void OnDispel
        (
            Unit dispeller,
            Unit caster,
            Unit target,
            int stacks,
            float elapsedTime
        )
    {
        target.Stats.bonusWalkSpeed -= bonusWalkSpeed * stacks;
        target.Stats.bonusAttackSpeed -= bonusAttackSpeed * stacks;
        target.SetColor(Color.clear, colorTransitionDurationOut);
    }
}
