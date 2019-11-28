using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityAnimation", menuName ="Abilities/Animation")]
public class AbilityAnimation : ScriptableObject
{
    public string trigger;
    public bool offHandSwing;
    [EnumFlags]
    public EquipmentObject.Class mainHand, offHand;
}
