using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EquipmentObject)), CanEditMultipleObjects]
public class EquipmentEditor : Editor
{
    private EquipmentObject item;

    private SerializedObject obj;

    #region Item
    private SerializedProperty baseName;
    private SerializedProperty quality;
    private SerializedProperty maxQuantity;
    private SerializedProperty displayQuantity;
    private SerializedProperty icon;
    private SerializedProperty inventorySize;
    private SerializedProperty description;
    private SerializedProperty sellable;
    private SerializedProperty luckDropChanceScalar;
    private SerializedProperty characterLevelDropChanceCurve;
    private SerializedProperty monsterLevelDropChanceCurve;
    #endregion
    #region Equipment
    private SerializedProperty requiresLevel;
    private SerializedProperty requiresStrength;
    private SerializedProperty requiresDexterity;
    private SerializedProperty requiresMagic;

    private SerializedProperty itemClass;
    private SerializedProperty wornItemName;
    private SerializedProperty heldItemMesh;
    private SerializedProperty heldItemPosition;
    private SerializedProperty heldItemEulerRotation;

    private SerializedProperty statsTemplate;
    private SerializedProperty stats;
    private SerializedProperty statPrefixSuffix;

    private SerializedProperty baseArmour;
    private SerializedProperty baseDamage;
    private SerializedProperty baseAttacksPer100Seconds;
    private SerializedProperty baseBlock;
    private SerializedProperty meleeRange;
    #endregion

    private void OnEnable()
    {
        obj = serializedObject;
        item = obj.targetObject as EquipmentObject;
        baseName = obj.FindProperty("baseName");
        quality = obj.FindProperty("quality");
        maxQuantity = obj.FindProperty("maxQuantity");
        displayQuantity = obj.FindProperty("displayQuantity");
        icon = obj.FindProperty("icon");
        inventorySize = obj.FindProperty("inventorySize");
        description = obj.FindProperty("description");
        sellable = obj.FindProperty("sellable");
        luckDropChanceScalar = obj.FindProperty("luckDropChanceScalar");
        characterLevelDropChanceCurve = obj.FindProperty("characterLevelDropChanceCurve");
        monsterLevelDropChanceCurve = obj.FindProperty("monsterLevelDropChanceCurve");

        requiresLevel = obj.FindProperty("requiresLevel");
        requiresStrength = obj.FindProperty("requiresStrength");
        requiresDexterity= obj.FindProperty("requiresDexterity");
        requiresMagic= obj.FindProperty("requiresMagic");

        itemClass = obj.FindProperty("itemClass");
        wornItemName = obj.FindProperty("wornItemName");
        heldItemMesh = obj.FindProperty("heldItemMesh");
        heldItemPosition = obj.FindProperty("heldItemPosition");
        heldItemEulerRotation = obj.FindProperty("heldItemEulerRotation");

        statsTemplate = obj.FindProperty("statsTemplate");
        statPrefixSuffix = obj.FindProperty("statPrefixSuffix");
        stats = obj.FindProperty("stats");

        baseArmour = obj.FindProperty("baseArmour");
        baseDamage = obj.FindProperty("baseDamage");
        baseAttacksPer100Seconds = obj.FindProperty("baseAttacksPer100Seconds");
        baseBlock = obj.FindProperty("baseBlock");
        meleeRange = obj.FindProperty("meleeRange");
    }

    public override void OnInspectorGUI()
    {

        
        obj.Update();

        GUIStyle header = new GUIStyle()
        {
            fontStyle = FontStyle.Bold
        };
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Template", header);
        EditorGUILayout.PropertyField(baseName);
        EditorGUILayout.PropertyField(icon);
        EditorGUILayout.PropertyField(inventorySize);
        EditorGUILayout.PropertyField(description);
        EditorGUILayout.PropertyField(quality);
        EditorGUILayout.PropertyField(itemClass);
        EditorGUILayout.Space();

        if (item.ItemClass.IsHeld())
        {
            EditorGUILayout.PropertyField(heldItemMesh);
            EditorGUILayout.PropertyField(heldItemPosition);
            EditorGUILayout.PropertyField(heldItemEulerRotation);
        }

        if (item.ItemClass.IsVisibleWornObject())
        {
            EditorGUILayout.PropertyField(wornItemName);
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(maxQuantity);
        EditorGUILayout.PropertyField(displayQuantity);

        EditorGUILayout.Space();

        if (item.ItemClass.HasDamage())
        {
            EditorGUILayout.PropertyField(baseDamage);
        }

        if (item.ItemClass.IsMeleeWeapon())
        {
            EditorGUILayout.PropertyField(meleeRange);
        }

        if (item.ItemClass.HasArmour())
        {
            EditorGUILayout.PropertyField(baseArmour);
        }

        if (item.ItemClass.HasBlock())
        {
            EditorGUILayout.PropertyField(baseBlock);
        }

        if (item.ItemClass.HasSpeed())
        {
            EditorGUILayout.PropertyField(baseAttacksPer100Seconds);
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(statsTemplate, true);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(requiresLevel);
        EditorGUILayout.PropertyField(requiresStrength);
        EditorGUILayout.PropertyField(requiresDexterity);
        EditorGUILayout.PropertyField(requiresMagic);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(sellable);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(luckDropChanceScalar);
        EditorGUILayout.PropertyField(characterLevelDropChanceCurve);
        EditorGUILayout.PropertyField(monsterLevelDropChanceCurve);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        Line();

        Rect last = GUILayoutUtility.GetLastRect();

        int width = item.InventorySizeX * 64;
        int height = item.InventorySizeY * 64;

        int maxInventoryWidth = 2 * 64;
        int maxInventoryHeight = 4 * 64;

        EditorGUILayout.BeginHorizontal();

        int insetX = 15;
        int insetY = 16;

        GUILayoutUtility.GetRect(width + insetX, height + insetY);
        //EditorGUI.DrawRect(new Rect(insetX, last.y + insetY, maxInventoryWidth, maxInventoryHeight), Color.gray);
        //GUI.Box(new Rect(insetX, last.y + insetY, maxInventoryWidth, maxInventoryHeight), "");

        if (item.Icon)
        {
            GUI.Box(new Rect(insetX, last.y + insetY, width, height), item.Icon, new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter });
            //GUI.Box(new Rect(insetX + (maxInventoryWidth - width) / 2, last.y + insetY + (maxInventoryHeight - height) / 2, width, height), item.Icon, new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter });
            //GUI.DrawTexture(new Rect(insetX + (maxInventoryWidth - width) / 2, last.y + insetY + (maxInventoryHeight - height) / 2, width, height), item.Icon);
        }


        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField(item.DisplayName, new GUIStyle() { fontSize = 18 });
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(item.DescriptiveName + " (" + item.ItemType.ToString().SplitCamelCase() + ")");
        EditorGUILayout.Space();

        if (item.ItemClass.HasArmour())
        {
            EditorGUILayout.LabelField(item.Armour + " Armour", new GUIStyle() { fontSize = 16 });
            EditorGUILayout.Space();
        }

        if (item.ItemClass.HasDamage())
        {
            EditorGUILayout.LabelField(item.DamagePerSecond.ToString("0.0") + " DPS", new GUIStyle() { fontSize = 16 });
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(item.Damage.Min().ToString("0") + "-" + item.Damage.Max().ToString("0") + " Damage");
        }

        if (item.ItemClass.HasBlock())
        {
            EditorGUILayout.LabelField(item.Block.ToString("0.#%") + " Chance to Block");
        }

        if (item.ItemClass.HasSpeed())
        {
            if (item.ItemClass.HasDamage() && item.ItemClass.HasBlock())
                EditorGUILayout.LabelField(item.AttacksPerSecond.ToString("0.00") + " Attacks/Blocks per Second");
            else if (item.ItemClass.HasDamage())
                EditorGUILayout.LabelField(item.AttacksPerSecond.ToString("0.00") + " Attacks per Second");
            else if (item.ItemClass.HasBlock())
                EditorGUILayout.LabelField(item.AttacksPerSecond.ToString("0.00") + " Blocks per Second");
        }

        if (item.DisplayQuantity)
        {
            if (item.Indestructible)
                EditorGUILayout.LabelField("Indestructible");
            else
                EditorGUILayout.LabelField(item.Quantity + "/" + item.QuantityMax + " Durability");
        }

        EditorGUILayout.Space();

        if (item.StatsLength > 0)
        {
            EditorGUILayout.PropertyField(stats);
            EditorGUILayout.Space();
        }

        if (item.RequiresLevel > 1 || item.RequiresStrength + item.RequiresDexterity + item.RequiresMagic > 0)
        {
            if (item.RequiresLevel > 1)
                EditorGUILayout.LabelField("Requires Level: " + item.RequiresLevel);
            if (item.RequiresStrength > 0)
                EditorGUILayout.LabelField("Requires Strength: " + item.RequiresStrength);
            if (item.RequiresDexterity > 0)
                EditorGUILayout.LabelField("Requires Dexterity: " + item.RequiresDexterity);
            if (item.RequiresMagic > 0)
                EditorGUILayout.LabelField("Requires Magic: " + item.RequiresMagic);

            EditorGUILayout.Space();
        }


        if (item.Description.Length > 0)
        {
            EditorGUILayout.LabelField(item.Description, new GUIStyle() { richText = true, wordWrap = true, });
            EditorGUILayout.Space();
        }

        if (item.Sellable)
        {
            EditorGUILayout.LabelField("Sell Value: " + item.SellValue);
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        obj.ApplyModifiedProperties();
    }

    public static void Line()
    {
        Rect rect = EditorGUILayout.GetControlRect(false);
        rect.height = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}
