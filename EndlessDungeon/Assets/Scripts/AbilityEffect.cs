using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Abilities/Effect")]
public class AbilityEffect : ScriptableObject 
{
    public enum DamageType
    {
        Normal,       //Attack damage is not multiplied by elemental spell damage stat
        Fire,
        Cold,
        Lightning,
        Poison,
        Shadow,
        Holy
    }

    public DamageType type;

    public ParticleSystem particles;
    public Vector2 damage;
    public Buff buff;

    public void Apply(Unit caster, Unit target)
    {

    }
}
