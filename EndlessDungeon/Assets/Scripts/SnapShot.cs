using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SnapShot
{
    public EquipmentObject.Class mainHandWeaponClass;
    public Vector2 mainHandDamage;
    public float mainHandAttacksPerSecond;
    public float mainHandRange;
    public EquipmentObject.Class offHandWeaponClass;
    public Vector2 offHandDamage;
    public float offHandAttacksPerSecond;
    public float offHandRange;
    public float fireSpellDamage;
    public float coldSpellDamage;
    public float lightningSpellDamage;
    public float poisonSpellDamage;
    public float shadowSpellDamage;
    public float holySpellDamage;
    public float castSpeed;
}
