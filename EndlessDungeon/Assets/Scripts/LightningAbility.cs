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
        bool channelling,
        SnapShot snapshot,
        GameObject objects
    )
    {
        ProceduralLightning lightning = Instantiate(lightningPrefab, caster.GetCastPosition(), Quaternion.identity, caster.GetCast());
        lightning.target = target.GetCenter();
        lightning.Generate();
        Instantiate(particles, target.GetCenterPosition(), Quaternion.identity, target.GetCenter());

        Vector2 damage = GetDamage(offHandSwing, snapshot);

        target.Damage(this, caster.GetCastPosition(), caster, DmgType, damage.Roll(), true, false);

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
            next.Damage(this, target.GetCenterPosition(), caster, DmgType, damage.Roll(), true, false);
            target = next;
            targets.Add(target);
        }
    }
}
