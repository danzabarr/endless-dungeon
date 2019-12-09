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


    public override void OnPulse(AbilityArgs args)
    {
        ProceduralLightning lightning = Instantiate(lightningPrefab, args.caster.GetCastPosition(), Quaternion.identity, args.caster.GetCast());
        lightning.target = args.target.GetCenter();
        lightning.Generate();
        Instantiate(particles, args.target.GetCenterPosition(), Quaternion.identity, args.target.GetCenter());

        Vector2 damage = GetDamage(args.offHandSwing, args.caster.Stats);

        args.target.Damage(this, args.caster.GetCastPosition(), args.caster, DmgType, damage.Roll(), true, false);

        List<Unit> targets = new List<Unit>();
        targets.Add(args.target);

        for (int i = 0; i < chainCount; i++)
        {
            List<Mob> mobs = Level.Instance.MobsInRadius(args.target.GetCenterPosition(), chainRange, true);

            float nearest = float.MaxValue;
            Mob next = null;

            foreach(Mob mob in mobs)
            {
                if (mob == null)
                    continue;
                if (targets.Contains(mob as Unit))
                    continue;
                float distanceSquared = Level.SquareDistance(args.target.GetCenterPosition(), mob.GetCenterPosition());
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

            lightning = Instantiate(lightningPrefab, args.target.GetCenterPosition(), Quaternion.identity, args.target.GetCenter());
            lightning.target = next.GetCenter();
            lightning.Generate();
            Instantiate(particles, next.GetCenterPosition(), Quaternion.identity, next.GetCenter());
            next.Damage(this, args.target.GetCenterPosition(), args.caster, DmgType, damage.Roll(), true, false);
            args.target = next;
            targets.Add(args.target);
        }
    }
}
