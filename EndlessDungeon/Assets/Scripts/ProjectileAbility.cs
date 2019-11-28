using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : Ability
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

        EquipmentObject.Class mainHandWeaponClass,
        Vector2 mainHandDamage,
        float mainHandAttacksPerSecond,
        float mainHandRange,

        EquipmentObject.Class offHandWeaponClass,
        Vector2 offHandDamage,
        float offHandAttacksPerSecond,
        float offHandRange,

        float fireSpellDamage,
        float coldSpellDamage,
        float lightningSpellDamage,
        float poisonSpellDamage,
        float shadowSpellDamage,
        float holySpellDamage,
        float spellAttacksPerSecond,
        GameObject objects
    )
    {
        float damageMultiplier = 1;
        switch (DmgType)
        {
            case DamageType.Physical:
                damageMultiplier = 1;
                break;
            case DamageType.Fire:
                damageMultiplier = fireSpellDamage;
                break;
            case DamageType.Cold:
                damageMultiplier = coldSpellDamage;
                break;
            case DamageType.Lightning:
                damageMultiplier = lightningSpellDamage;
                break;
            case DamageType.Poison:
                damageMultiplier = poisonSpellDamage;
                break;
            case DamageType.Shadow:
                damageMultiplier = shadowSpellDamage;
                break;
            case DamageType.Holy:
                damageMultiplier = holySpellDamage;
                break;
        }

        Vector3 direction = target == null ? (castTarget - caster.GetCastPosition()).normalized : (target.GetCenterPosition() - caster.GetCastPosition()).normalized;
        Instantiate(Projectile, objects.transform).Init(caster, direction, projectileSpeed, Damage * damageMultiplier, DmgType);
    }
}
