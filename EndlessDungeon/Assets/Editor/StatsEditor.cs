
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
    private SerializedProperty healthRegenBase;
    private SerializedProperty blockSpeedBase;
    private SerializedProperty hitRecoverySpeedBase;
    private SerializedProperty walkSpeedBase;

    private SerializedProperty maxHealthPerStrength;
    private SerializedProperty weaponDamagePerStrength;
    private SerializedProperty hitRecoverySpeedPerStrength;

    private SerializedProperty blockSpeedPerDexterity;
    private SerializedProperty castSpeedPerDexterity;
    private SerializedProperty attackSpeedPerDexterity;

    private SerializedProperty healthRegenPerMagic;
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
    private SerializedProperty blockSpeed;
    private SerializedProperty castSpeed;
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
    private SerializedProperty mainHandBlockSpeed;
    private SerializedProperty offHandWeaponClass;
    private SerializedProperty offHandDamage;
    private SerializedProperty offHandAttacksPerSecond;
    private SerializedProperty offHandRange;
    private SerializedProperty offHandBlock;
    private SerializedProperty offHandBlockSpeed;

    [SerializeField]
    private int walkSpeed;

    public void OnEnable()
    {
        obj = serializedObject;
        stats = obj.targetObject as Stats;

        maxHealthBase                   = obj.FindProperty("maxHealthBase");
        healthRegenBase                 = obj.FindProperty("healthRegenBase");
        hitRecoverySpeedBase            = obj.FindProperty("hitRecoverySpeedBase");
        blockSpeedBase                  = obj.FindProperty("blockSpeedBase");
        walkSpeedBase                   = obj.FindProperty("walkSpeedBase");

        maxHealthPerStrength            = obj.FindProperty("maxHealthPerStrength");
        weaponDamagePerStrength         = obj.FindProperty("weaponDamagePerStrength");
        hitRecoverySpeedPerStrength     = obj.FindProperty("hitRecoverySpeedPerStrength");

        blockSpeedPerDexterity          = obj.FindProperty("blockSpeedPerDexterity");
        attackSpeedPerDexterity         = obj.FindProperty("attackSpeedPerDexterity");
        castSpeedPerDexterity           = obj.FindProperty("castSpeedPerDexterity");

        healthRegenPerMagic             = obj.FindProperty("healthRegenPerMagic");
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
        castSpeed                       = obj.FindProperty("castSpeed");
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
        EditorGUILayout.PropertyField(healthRegenBase);
        EditorGUILayout.PropertyField(hitRecoverySpeedBase);
        EditorGUILayout.PropertyField(walkSpeedBase);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(maxHealthPerStrength);
        EditorGUILayout.PropertyField(weaponDamagePerStrength);
        EditorGUILayout.PropertyField(hitRecoverySpeedPerStrength);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(attackSpeedPerDexterity);
        EditorGUILayout.PropertyField(castSpeedPerDexterity);
        EditorGUILayout.PropertyField(blockSpeedPerDexterity);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(healthRegenPerMagic);
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
        LayoutPair(castSpeed, stats.CastSpeed, "0.##/s", enableEditing);
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
