
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ability), true), CanEditMultipleObjects]
public class AbilityEditor : Editor
{
    private SerializedObject obj;
    private Ability ability;

    public void OnEnable()
    {
        obj = serializedObject;
        ability = obj.targetObject as Ability;
    }

    public override void OnInspectorGUI()
    {


        /*
    [SerializeField]
    [DisplayIf("abilityType", AbilityType.Projectile)]
    private Projectile projectile;

    [SerializeField]
    [DisplayIf("abilityType", AbilityType.Thrown)]
    private Throwable throwable;

    [SerializeField]
    [DisplayIf("abilityType", AbilityType.Place)]
    private Placeable placeable;

    //[DisplayIfNot("abilityType", AbilityType.Projectile, AbilityType.Thrown, AbilityType.Place)]
    [SerializeField]
    public UnityEvent effect;

    [SerializeField]
    [DisplayIf("abilityType", AbilityType.Targeted, AbilityType.Melee, AbilityType.Cleave, AbilityType.Nova, AbilityType.Projectile, AbilityType.Thrown)]
    private string pattern;

    [SerializeField]
    [DisplayIfNot("abilityType", AbilityType.Self, AbilityType.Projectile)]
    private float range;

    [SerializeField]
    [DisplayIfRange(0, 360, "abilityType", AbilityType.Cleave)]
    private float arc;

    [SerializeField]
    [DisplayIfNot("abilityType", AbilityType.Self)]
    private Affects affects;
         */


        List<string> exclude = new List<string>();

        switch (ability.Type)
        {
            case Ability.AbilityType.Self:
                exclude.Add("projectile");
                exclude.Add("projectileSpeed");
                exclude.Add("throwable");
                exclude.Add("placeable");
                exclude.Add("range");
                exclude.Add("useWeaponRange");
                exclude.Add("arc");
                exclude.Add("affects");
                
                break;

            case Ability.AbilityType.Targeted:
                exclude.Add("projectile");
                exclude.Add("projectileSpeed");
                exclude.Add("throwable");
                exclude.Add("placeable");
                exclude.Add("arc");
                
                break;

            case Ability.AbilityType.Melee:
                exclude.Add("projectile");
                exclude.Add("projectileSpeed");
                exclude.Add("throwable");
                exclude.Add("placeable");
                exclude.Add("arc");
                
                break;

            case Ability.AbilityType.Cleave:
                exclude.Add("projectile");
                exclude.Add("projectileSpeed");
                exclude.Add("throwable");
                exclude.Add("placeable");
                
                break;
            case Ability.AbilityType.Nova:
                exclude.Add("projectile");
                exclude.Add("projectileSpeed");
                exclude.Add("throwable");
                exclude.Add("placeable");
                exclude.Add("arc");
                
                break;
            case Ability.AbilityType.Projectile:
                exclude.Add("throwable");
                exclude.Add("placeable");
                exclude.Add("range");
                exclude.Add("arc");
                
                break;

            case Ability.AbilityType.Thrown:
                exclude.Add("projectile");
                exclude.Add("projectileSpeed");
                exclude.Add("placeable");
                exclude.Add("arc");
                
                break;

            case Ability.AbilityType.Place:
                exclude.Add("projectile");
                exclude.Add("projectileSpeed");
                exclude.Add("throwable");
                exclude.Add("arc");
                
                break;

            case Ability.AbilityType.Auto:
                break;
        }

        if (!obj.FindProperty("usePattern").boolValue) exclude.Add("pattern");

        DrawPropertiesExcluding(obj, exclude.ToArray());
        obj.ApplyModifiedProperties();
    }
}
