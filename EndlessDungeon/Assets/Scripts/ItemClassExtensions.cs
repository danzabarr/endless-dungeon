using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EquipmentObject;

public static class ItemClassExtensions
{
    public static Type Type(this Class item)
    {
        switch (item)
        {
            case Class.Unarmed:
                return EquipmentObject.Type.Unequippable;
            case Class.Dagger:
                return EquipmentObject.Type.OneHand;
            case Class.Sword:
                return EquipmentObject.Type.OneHand;
            case Class.TwoHandedSword:
                return EquipmentObject.Type.TwoHand;
            case Class.Axe:
                return EquipmentObject.Type.OneHand;
            case Class.TwoHandedAxe:
                return EquipmentObject.Type.TwoHand;
            case Class.Mace:
                return EquipmentObject.Type.OneHand;
            case Class.TwoHandedMace:
                return EquipmentObject.Type.TwoHand;
            case Class.Spear:
                return EquipmentObject.Type.OneHand;
            case Class.TwoHandedSpear:
                return EquipmentObject.Type.TwoHand;
            case Class.Polearm:
                return EquipmentObject.Type.TwoHand;
            case Class.Stave:
                return EquipmentObject.Type.TwoHand;
            case Class.Wand:
                return EquipmentObject.Type.OneHand;
            case Class.Shield:
                return EquipmentObject.Type.OffHand;
            case Class.Hood:
                return EquipmentObject.Type.Head;
            case Class.Hat:
                return EquipmentObject.Type.Head;
            case Class.Helmet:
                return EquipmentObject.Type.Head;
            case Class.Tunic:
                return EquipmentObject.Type.Body;
            case Class.Robe:
                return EquipmentObject.Type.Body;
            case Class.LeatherArmour:
                return EquipmentObject.Type.Body;
            case Class.ChainmailArmour:
                return EquipmentObject.Type.Body;
            case Class.PlateArmour:
                return EquipmentObject.Type.Body;
            case Class.Bow:
                return EquipmentObject.Type.TwoHand;
            case Class.CrossBow:
                return EquipmentObject.Type.TwoHand;
            case Class.Thrown:
                return EquipmentObject.Type.OneHand;
            default:
                return EquipmentObject.Type.Unequippable;
        }
    }

    public static bool Equippable(this Class item, Slot slot)
    {
        return Equippable(item.Type(), slot);
    }

    public static bool Equippable(Type type, Slot slot)
    {
        switch (type)
        {
            case EquipmentObject.Type.Unequippable:
                return false;
            case EquipmentObject.Type.Head:
                return slot == Slot.Head;
            case EquipmentObject.Type.Body:
                return slot == Slot.Body;
            case EquipmentObject.Type.Hands:
                return slot == Slot.Hands;
            case EquipmentObject.Type.Feet:
                return slot == Slot.Feet;
            case EquipmentObject.Type.Finger:
                return slot == Slot.Finger;
            case EquipmentObject.Type.Neck:
                return slot == Slot.Neck;
            case EquipmentObject.Type.OneHand:
                return slot == Slot.MainHand || slot == Slot.OffHand;
            case EquipmentObject.Type.TwoHand:
                return slot == Slot.MainHand || slot == Slot.OffHand;
            case EquipmentObject.Type.OffHand:
                return slot == Slot.OffHand;
            default:
                return false;
        }
    }

    public static bool HasArmour(this Class item)
    {
        switch (item)
        {
            case Class.Unarmed:
                return false;
            case Class.Dagger:
                return false;
            case Class.Sword:
                return false;
            case Class.TwoHandedSword:
                return false;
            case Class.Axe:
                return false;
            case Class.TwoHandedAxe:
                return false;
            case Class.Mace:
                return false;
            case Class.TwoHandedMace:
                return false;
            case Class.Spear:
                return false;
            case Class.TwoHandedSpear:
                return false;
            case Class.Polearm:
                return false;
            case Class.Stave:
                return false;
            case Class.Wand:
                return false;
            case Class.Shield:
                return true;
            case Class.Hood:
                return true;
            case Class.Hat:
                return true;
            case Class.Helmet:
                return true;
            case Class.Tunic:
                return false;
            case Class.Robe:
                return false;
            case Class.LeatherArmour:
                return true;
            case Class.ChainmailArmour:
                return true;
            case Class.PlateArmour:
                return true;
            default:
                return false;
        }
    }

    public static bool HasBlock(this Class item)
    {
        switch (item)
        {
            case Class.Unarmed:
                return false;
            case Class.Dagger:
                return true;
            case Class.Sword:
                return true;
            case Class.TwoHandedSword:
                return true;
            case Class.Axe:
                return true;
            case Class.TwoHandedAxe:
                return true;
            case Class.Mace:
                return true;
            case Class.TwoHandedMace:
                return true;
            case Class.Spear:
                return true;
            case Class.TwoHandedSpear:
                return true;
            case Class.Polearm:
                return true;
            case Class.Stave:
                return true;
            case Class.Wand:
                return false;
            case Class.Shield:
                return true;

            default:
                return false;
        }
    }

    public static bool HasDamage(this Class item)
    {
        switch (item)
        {
            case Class.Dagger:
                return true;
            case Class.Sword:
                return true;
            case Class.TwoHandedSword:
                return true;
            case Class.Axe:
                return true;
            case Class.TwoHandedAxe:
                return true;
            case Class.Mace:
                return true;
            case Class.TwoHandedMace:
                return true;
            case Class.Spear:
                return true;
            case Class.TwoHandedSpear:
                return true;
            case Class.Polearm:
                return true;
            case Class.Stave:
                return true;
            case Class.Bow:
                return true;
            case Class.CrossBow:
                return true;
            case Class.Thrown:
                return true;
            default:
                return false;
        }
    }

    public static bool HasSpeed(this Class item)
    {
        switch (item)
        {
            case Class.Dagger:
                return true;
            case Class.Sword:
                return true;
            case Class.TwoHandedSword:
                return true;
            case Class.Axe:
                return true;
            case Class.TwoHandedAxe:
                return true;
            case Class.Mace:
                return true;
            case Class.TwoHandedMace:
                return true;
            case Class.Spear:
                return true;
            case Class.TwoHandedSpear:
                return true;
            case Class.Polearm:
                return true;
            case Class.Stave:
                return true;
            case Class.Bow:
                return true;
            case Class.CrossBow:
                return true;
            case Class.Thrown:
                return true;
            case Class.Shield:
                return true;
            default:
                return false;
        }
    }

    public static bool IsMeleeWeapon(this Class item)
    {
        switch (item)
        {
            case Class.Dagger:
                return true;
            case Class.Sword:
                return true;
            case Class.TwoHandedSword:
                return true;
            case Class.Axe:
                return true;
            case Class.TwoHandedAxe:
                return true;
            case Class.Mace:
                return true;
            case Class.TwoHandedMace:
                return true;
            case Class.Spear:
                return true;
            case Class.TwoHandedSpear:
                return true;
            case Class.Polearm:
                return true;
            case Class.Stave:
                return true;

            default:
                return false;
        }
    }

    public static bool IsHeld(this Class item)
    {
        switch (item.Type())
        {
            case EquipmentObject.Type.OneHand:
                return true;
            case EquipmentObject.Type.TwoHand:
                return true;
            case EquipmentObject.Type.OffHand:
                return true;
        }
        return false;
    }

    public static bool IsRangedWeapon(this Class item)
    {
        switch (item)
        {
            case Class.Bow:
                return true;
            case Class.CrossBow:
                return true;
            case Class.Thrown:
                return true;
        }
        return false;
    }

    public static bool ShootsProjectile(this Class item)
    {
        switch (item)
        {
            case Class.Bow:
                return true;
            case Class.CrossBow:
                return true;
        }
        return false;
    }

    public static bool ShootsThrowable(this Class item)
    {
        switch (item)
        {
            case Class.Thrown:
                return true;
        }
        return false;
    }

    public static Ability.AbilityType StandardAbilityType(this Class item)
    {
        switch(item)
        {
            case Class.Bow:
                return Ability.AbilityType.Projectile;
            case Class.CrossBow:
                return Ability.AbilityType.Projectile;
            case Class.Thrown:
                return Ability.AbilityType.Thrown;
        }
        return Ability.AbilityType.Melee;
    }

    public static bool IsVisibleWornObject(this Class item)
    {
        switch (item.Type())
        {
            case EquipmentObject.Type.Head:
                return true;
            case EquipmentObject.Type.Body:
                return true;
            case EquipmentObject.Type.Hands:
                return true;
            case EquipmentObject.Type.Feet:
                return true;
            case EquipmentObject.Type.Unequippable:
                return false;
            case EquipmentObject.Type.Finger:
                return false;
            case EquipmentObject.Type.Neck:
                return false;
        }

        //Special case for bows/xbows because they need animating.
        if (item == Class.Bow)
            return true;
        if (item == Class.CrossBow)
            return true;

        return false;
    }


    public static string Name(this Class item)
    {
        switch (item)
        {
            case Class.Unarmed:
                return "Unarmed";
            case Class.Dagger:
                return "Dagger";
            case Class.Sword:
                return "Sword";
            case Class.TwoHandedSword:
                return "Sword";
            case Class.Axe:
                return "Axe";
            case Class.TwoHandedAxe:
                return "Axe";
            case Class.Mace:
                return "Mace";
            case Class.TwoHandedMace:
                return "Mace";
            case Class.Spear:
                return "Spear";
            case Class.TwoHandedSpear:
                return "Spear";
            case Class.Polearm:
                return "Polearm";
            case Class.Stave:
                return "Stave";
            case Class.Wand:
                return "Wand";
            case Class.Shield:
                return "Shield";
            case Class.Hood:
                return "Hood";
            case Class.Hat:
                return "Hat";
            case Class.Helmet:
                return "Helmet";
            case Class.Tunic:
                return "Tunic";
            case Class.Robe:
                return "Robe";
            case Class.LeatherArmour:
                return "Leather Armour";
            case Class.ChainmailArmour:
                return "Chainmail";
            case Class.PlateArmour:
                return "Plate Armour";
            case Class.Bow:
                return "Bow";
            case Class.CrossBow:
                return "Crossbow";
            case Class.Thrown:
                return "Thrown Weapon";
            default:
                return "";
        }
    }

    public static bool IsCompatible(this Class requirement, EquipmentObject weapon)
    {
        return (int)requirement == ((int)requirement | (1 << (int)(weapon == null ? Class.Unarmed : weapon.ItemClass)));
    }
    public static bool IsCompatible(this Class requirement, Class weapon)
    {
        return (int)requirement == ((int)requirement | (1 << (int)weapon));
    }
}
