using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Abilities/Buff")]
public class Buff : ScriptableObject
{
    public enum Duplicates
    {
        AnyAmount,
        OnePerTarget,
        OnePerCaster,
        OnePerCasterPerTarget,
    }

    [SerializeField]
    private new string name;

    [SerializeField]
    private Texture2D icon;

    [SerializeField]
    private ParticleSystem particles;

    [SerializeField]
    [TextArea]
    private string description;

    [SerializeField]
    private float duration;

    [SerializeField]
    private float tickInterval;

    [SerializeField]
    private int maxStacks;

    [SerializeField]
    private bool resetTimerOnAddStack;

    [SerializeField]
    private bool removeStackOnExpire;

    [SerializeField]
    private Duplicates allowDuplicates;

    [SerializeField]
    private Ability.DamageType damageType;

    public string DisplayName => name;
    public Texture2D Icon => icon;
    public string Description => description;
    public float Duration => duration;
    public bool Permanent => duration <= 0;
    public float TickInterval => tickInterval;
    public int MaxStacks => maxStacks;
    public bool ResetTimerOnAddStack => resetTimerOnAddStack;
    public bool RemoveStackOnExpire => removeStackOnExpire;
    public ParticleSystem Particles => particles;

    public Ability.DamageType DamageType => damageType;
    public Duplicates AllowDuplicates => allowDuplicates;

    public virtual void OnApply
        (
            Unit caster,
            Unit target,
            int stacks,
            float elapsedTime
        )
    { }

    public virtual void OnTick
        (
            Unit caster,
            Unit target,
            int stacks,
            float elapsedTime
        )
    { }

    public virtual void OnAddStack
        (
            Unit caster,
            Unit target,
            int stacks,
            int stacksAdded,
            float elapsedTime
        )
    { }

    public virtual void OnRemoveStack
        (
            Unit remover,
            Unit caster,
            Unit target,
            int stacks,
            int stacksRemoved,
            float elapsedTime
        )
    { }

    public virtual void OnRefresh
        (
            Unit caster,
            Unit target,
            int stacks,
            float elapsedTime
        )
    { }

    public virtual void OnDispel
        (
            Unit dispeller,
            Unit caster,
            Unit target,
            int stacks,
            float elapsedTime
        )
    { }

    public virtual void OnExpire
        (
            Unit caster,
            Unit target,
            int stacks,
            float elapsedTime
        )
    { }
}
