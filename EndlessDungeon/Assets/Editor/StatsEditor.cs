
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Stats)), CanEditMultipleObjects]
public class StatsEditor : Editor
{
    public static readonly int pickerWidth = 18;

    private SerializedObject obj;
    private Stats stats;
    private GUIStyle header;

    private SerializedProperty maxHealthBase;
    private SerializedProperty maxHealthPerStrength;
    private SerializedProperty healthRegenBase;
    private SerializedProperty healthRegenPerMagic;
    private SerializedProperty weaponDamagePerStrength;
    private SerializedProperty hitRecoverySpeedBase;
    private SerializedProperty hitRecoverySpeedPerStrength;
    private SerializedProperty attackSpeedPerDexterity;
    private SerializedProperty walkSpeedBase;
    private SerializedProperty spellDamagePerMagic;

    private SerializedProperty strength;
    private SerializedProperty dexterity;
    private SerializedProperty magic;

    private SerializedProperty maxHealth;
    private SerializedProperty regenHealth;
    private SerializedProperty armour;

    private SerializedProperty fireResist;
    private SerializedProperty coldResist;
    private SerializedProperty lightningResist;
    private SerializedProperty poisonResist;
    private SerializedProperty shadowResist;
    private SerializedProperty holyResist;

    private SerializedProperty hitRecoverySpeed;
    private SerializedProperty attackSpeed;

    private SerializedProperty fireDamage;
    private SerializedProperty coldDamage;
    private SerializedProperty lightningDamage;
    private SerializedProperty poisonDamage;
    private SerializedProperty shadowDamage;
    private SerializedProperty holyDamage;

    private SerializedProperty mainHandWeaponClass;
    private SerializedProperty mainHandDamage;
    private SerializedProperty mainHandAttacksPerSecond;
    private SerializedProperty mainHandRange;
    private SerializedProperty mainHandBlock;
    private SerializedProperty offHandWeaponClass;
    private SerializedProperty offHandDamage;
    private SerializedProperty offHandAttacksPerSecond;
    private SerializedProperty offHandRange;
    private SerializedProperty offHandBlock;

    [SerializeField]
    private int walkSpeed;

    public void OnEnable()
    {
        obj = serializedObject;
        stats = obj.targetObject as Stats;

        maxHealthBase                   = obj.FindProperty("maxHealthBase");
        maxHealthPerStrength            = obj.FindProperty("maxHealthPerStrength");
        healthRegenBase                 = obj.FindProperty("healthRegenBase");
        healthRegenPerMagic             = obj.FindProperty("healthRegenPerMagic");
        weaponDamagePerStrength         = obj.FindProperty("weaponDamagePerStrength");
        hitRecoverySpeedBase            = obj.FindProperty("hitRecoverySpeedBase");
        hitRecoverySpeedPerStrength     = obj.FindProperty("hitRecoverySpeedPerStrength");
        attackSpeedPerDexterity         = obj.FindProperty("attackSpeedPerDexterity");
        walkSpeedBase                   = obj.FindProperty("walkSpeedBase");
        spellDamagePerMagic             = obj.FindProperty("spellDamagePerMagic");

        strength                        = obj.FindProperty("strength");
        dexterity                       = obj.FindProperty("dexterity");
        magic                           = obj.FindProperty("magic");

        maxHealth                       = obj.FindProperty("maxHealth");
        regenHealth                     = obj.FindProperty("regenHealth");
        armour                          = obj.FindProperty("armour");

        fireResist                      = obj.FindProperty("fireResist");
        coldResist                      = obj.FindProperty("coldResist");
        lightningResist                 = obj.FindProperty("lightningResist");
        poisonResist                    = obj.FindProperty("poisonResist");
        shadowResist                    = obj.FindProperty("shadowResist");
        holyResist                      = obj.FindProperty("holyResist");

        hitRecoverySpeed                = obj.FindProperty("hitRecoverySpeed");
        attackSpeed                     = obj.FindProperty("attackSpeed");
        fireDamage                      = obj.FindProperty("fireDamage");
        coldDamage                      = obj.FindProperty("coldDamage");
        lightningDamage                 = obj.FindProperty("lightningDamage");
        poisonDamage                    = obj.FindProperty("poisonDamage");
        shadowDamage                    = obj.FindProperty("shadowDamage");
        holyDamage                      = obj.FindProperty("holyDamage");

        mainHandWeaponClass             = obj.FindProperty("mainHandWeaponClass");
        mainHandDamage                  = obj.FindProperty("mainHandDamage");
        mainHandAttacksPerSecond        = obj.FindProperty("mainHandAttacksPerSecond");
        mainHandRange                   = obj.FindProperty("mainHandRange");
        mainHandBlock                   = obj.FindProperty("mainHandBlock");

        offHandWeaponClass              = obj.FindProperty("offHandWeaponClass");
        offHandDamage                   = obj.FindProperty("offHandDamage");
        offHandAttacksPerSecond         = obj.FindProperty("offHandAttacksPerSecond");
        offHandRange                    = obj.FindProperty("offHandRange");
        offHandBlock                    = obj.FindProperty("offHandBlock");
    }

    public override void OnInspectorGUI()
    {
        header = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };

        EditorGUILayout.PropertyField(maxHealthBase);
        EditorGUILayout.PropertyField(maxHealthPerStrength);
        EditorGUILayout.PropertyField(healthRegenBase);
        EditorGUILayout.PropertyField(healthRegenPerMagic);
        EditorGUILayout.PropertyField(weaponDamagePerStrength);
        EditorGUILayout.PropertyField(hitRecoverySpeedBase);
        EditorGUILayout.PropertyField(hitRecoverySpeedPerStrength);
        EditorGUILayout.PropertyField(attackSpeedPerDexterity);
        EditorGUILayout.PropertyField(walkSpeedBase);
        EditorGUILayout.PropertyField(spellDamagePerMagic);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Core Stats");
        EditorGUILayout.PropertyField(strength);
        EditorGUILayout.PropertyField(dexterity);
        EditorGUILayout.PropertyField(magic);


        bool enableEditing = true;

        if (stats.GetComponent<EquipmentManager>())
        {
            enableEditing = false;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("(The following properties are driven by this unit's EquipmentManager component)");
            EditorGUILayout.Space();
        }

        Header("Defensive Stats");
        LayoutPair(maxHealth, stats.MaxHealth, "0.#", enableEditing);
        LayoutPair(regenHealth, stats.RegenHealth, "0.##/s", enableEditing);
        LayoutPair(armour, stats.Armour, "0.#", enableEditing);
        LayoutPair(hitRecoverySpeed, stats.HitRecoveryDuration, "0.##s", enableEditing);
        EditorGUILayout.Space();

        Header("Elemental Resistances");
        LayoutPair(fireResist, stats.FireResist / 100f, "0.#%", enableEditing);
        LayoutPair(coldResist, stats.ColdResist / 100f, "0.#%", enableEditing);
        LayoutPair(lightningResist, stats.LightningResist / 100f, "0.#%", enableEditing);
        LayoutPair(poisonResist, stats.PoisonResist / 100f, "0.#%", enableEditing);
        LayoutPair(shadowResist, stats.ShadowResist / 100f, "0.#%", enableEditing);
        LayoutPair(holyResist, stats.HolyResist / 100f, "0.#%", enableEditing);
        EditorGUILayout.Space();

        Header("Offensive Stats");
        LayoutPair(attackSpeed, stats.AttackSpeed, "0.##/s", enableEditing);
        EditorGUILayout.Space();

        Header(" ");
        LayoutSingle(mainHandWeaponClass, enableEditing);
        LayoutPair(mainHandDamage, stats.MainHandDamage, "", enableEditing);
        LayoutPair(mainHandAttacksPerSecond, stats.MainHandAttacksPerSecond, "0.##/s", enableEditing);
        LayoutPair(mainHandRange, stats.MainHandRange, "0.#", enableEditing);
        LayoutPair(mainHandBlock, stats.MainHandBlockChance, "0.#%", enableEditing);
        EditorGUILayout.Space();

        Header(" ");
        LayoutSingle(offHandWeaponClass, enableEditing);
        LayoutPair(offHandDamage, stats.OffHandDamage, "", enableEditing);
        LayoutPair(offHandAttacksPerSecond, stats.OffHandAttacksPerSecond, "0.##/s", enableEditing);
        LayoutPair(offHandRange, stats.OffHandRange, "0.#", enableEditing);
        LayoutPair(offHandBlock, stats.OffHandBlockChance, "0.#%", enableEditing);

        // DrawPropertiesExcluding(obj, "m_Script", "strength", "dexterity", "magic");
        obj.ApplyModifiedProperties();
    }

    private void Header(string str)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(str, "Gear");
        EditorGUILayout.LabelField("Effective", GUILayout.MaxWidth(100));
        EditorGUILayout.EndHorizontal();
    }

    private void LayoutSingle(SerializedProperty gearStat, bool enableEditing)
    {
        EditorGUILayout.BeginHorizontal();
        if (enableEditing)
            EditorGUILayout.PropertyField(gearStat);
        else
        {
            switch(gearStat.type)
            {
                case "Enum":
                    EditorGUILayout.LabelField(gearStat.displayName, gearStat.enumDisplayNames[gearStat.enumValueIndex]);
                    break;
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void LayoutPair(SerializedProperty gearStat, float displayStat, string format, bool enableEditing)
    {
        EditorGUILayout.BeginHorizontal();
        
        if (enableEditing)
            EditorGUILayout.PropertyField(gearStat);
        else
        {
            switch (gearStat.type)
            {
                case "int":
                    EditorGUILayout.LabelField(gearStat.displayName, gearStat.intValue.ToString());
                    break;
                case "float":
                    EditorGUILayout.LabelField(gearStat.displayName, gearStat.floatValue.ToString());
                    break;
            }
        }

        EditorGUILayout.LabelField(displayStat.ToString(format), GUILayout.MaxWidth(100));

        EditorGUILayout.EndHorizontal();
    }

    private void LayoutPair(SerializedProperty gearStat, Vector2 displayStat, string format, bool enableEditing)
    {
        EditorGUILayout.BeginHorizontal();
        if (enableEditing)
            EditorGUILayout.PropertyField(gearStat);
        else
            EditorGUILayout.LabelField(gearStat.displayName, gearStat.vector2Value.Min() + "-" + gearStat.vector2Value.Max());

        EditorGUILayout.LabelField(displayStat.Min() + "-" + displayStat.Max(), GUILayout.MaxWidth(100));

        EditorGUILayout.EndHorizontal();
    }
}
