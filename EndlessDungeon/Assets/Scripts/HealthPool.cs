using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HealthPool 
{
    GameObject GetGameObject();
    Transform GetCast();
    Transform GetCenter();
    Transform GetGround();
    Vector3 GetCenterPosition();
    Vector3 GetGroundPosition();
    Vector3 GetCastPosition();
    Quaternion GetRotation();
    Unit.Faction GetFaction();
    void SetFaction(Unit.Faction faction);
    float GetCurrentHealth();
    float GetMaxHealth();
    void Damage(Unit caster, Ability.DamageType type, float amount, bool canStun, bool blockable);
    void OnTakeDamage(Unit caster, Ability.DamageType type, float amount);
    void Heal(Unit caster, float amount);
    void Hit(float duration);
    void Kill();
    bool ApplyBuff(Unit caster, Buff buff, out BuffInstance instance, out bool newInstance, int stacks = 1, int maxStacks = 0);
    bool ApplyDebuff(Unit caster, Buff debuff, out BuffInstance instance, out bool newInstance, bool useResistances, int stacks = 1, int maxStacks = 0);
    void LookInDirection(Vector3 direction);
    void WalkTo(Vector3 position);
    void Interact(Interactive interactive);
    void OnDeath();
    void Teleport(Vector3 position);
    void SetColor(Color color, float transitionDuration);
}
