using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningAbility : Ability
{
    [SerializeField]
    private ProceduralLightning lightningPrefab;

    [SerializeField]
    private ParticleSystem particles;

    [SerializeField]
    private int chainCount;

    [SerializeField]
    private float chainRange;


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

        ProceduralLightning lightning = Instantiate(lightningPrefab, caster.GetCastPosition(), Quaternion.identity, caster.GetCast());
        lightning.target = target.GetCenter();
        lightning.Generate();
        Instantiate(particles, target.GetCenterPosition(), Quaternion.identity, target.GetCenter());
        target.Damage(this, caster, DmgType, Damage.Roll() * damageMultiplier, true, false);

        List<Unit> targets = new List<Unit>();
        targets.Add(target);

        for (int i = 0; i < chainCount; i++)
        {
            List<Mob> mobs = Level.Instance.MobsInRadius(target.GetCenterPosition(), chainRange, true);

            float nearest = float.MaxValue;
            Mob next = null;

            foreach(Mob mob in mobs)
            {
                if (mob == null)
                    continue;
                if (targets.Contains(mob as Unit))
                    continue;
                float distanceSquared = Level.SquareDistance(target.GetCenterPosition(), mob.GetCenterPosition());
                if (nearest > distanceSquared)
                {
                    nearest = distanceSquared;
                    next = mob;
                }
            }

            if (next == null)
            {
                return;
            }

            lightning = Instantiate(lightningPrefab, target.GetCenterPosition(), Quaternion.identity, target.GetCenter());
            lightning.target = next.GetCenter();
            lightning.Generate();
            Instantiate(particles, next.GetCenterPosition(), Quaternion.identity, next.GetCenter());
            next.Damage(this, caster, DmgType, Damage.Roll() * damageMultiplier, true, false);
            target = next;
            targets.Add(target);
        }
    }
}
