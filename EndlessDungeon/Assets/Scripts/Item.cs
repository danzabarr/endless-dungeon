using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    public enum Quality
    {
        Junk = 0,
        Normal = 1,
        Rare = 2,
        Legendary = 3,
        Quest = 4,
        Gold = 5
    }

    private static readonly Color[] qualityColors =
    {
        new Color(.40f, .40f, .40f),//Junk
        new Color(.90f, .90f, .90f),//Normal
        new Color(.20f, .50f, .90f),//Rare
        new Color(.80f, .70f, .40f),//Legendary
        new Color(.20f, .80f, .20f),//Quest
        new Color(.90f, .90f, .90f),//Gold
    };

    public static Color QualityColor(Quality quality)
    {
        return qualityColors[(int)quality];
    }

    [SerializeField]
    private ItemObject objectPrefab;

    [SerializeField]
    protected string baseName;

    [SerializeField]
    protected Quality quality;

    [SerializeField]
    [MinMax(1, 999)]
    protected int maxQuantity = 1;

    [SerializeField]
    protected Texture2D icon;

    [SerializeField]
    [TextArea]
    protected string description;

    [SerializeField]
    protected bool sellable;

    [SerializeField]
    protected bool displayQuantity;

    [SerializeField]
    protected Vector3 labelWorldOffset;

    [SerializeField]
    protected Vector2 labelScreenOffset;

    [SerializeField]
    protected Vector2Int inventorySize;

}
