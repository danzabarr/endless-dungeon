using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[System.Serializable]
public class ItemObject : MonoBehaviour, RegisterablePrefab, Interactive
{
    private int prefabID;
    public int GetPrefabID() => prefabID;
    public void SetPrefabID(int id) => prefabID = id;

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

    private ObjectLabel Label
    {
        get
        {
            if (label == null)
            {
                label = LabelManager.Add(this);
                label.SeparateRectangles = true;
                label.BarEnabled = false;
                label.worldTransform = transform;
                label.worldOffset = labelWorldOffset;
                label.screenOffset = labelScreenOffset;
                label.LabelText = displayName;
                label.LabelTextColor = QualityColor(quality);
            }
            return label;
        }
    }

    protected Outline[] outline;
    protected ObjectLabel label;

    public DynamicObject DynamicObject { get; private set; }
    public Rigidbody Rigidbody { get; private set; }

    [SerializeField]
    protected string baseName;

    [SerializeField]
    protected Quality quality;

    [SerializeField][MinMax(1, 999)]
    protected int maxQuantity = 1;

    [SerializeField]
    protected Texture2D icon;

    [SerializeField][TextArea]
    protected string description;

    [SerializeField]
    protected bool sellable;

    [SerializeField]
    protected bool displayQuantity;

    [DisplayOnly]
    protected string displayName;

    [DisplayOnly]
    protected int currentQuantity;

    [SerializeField]
    protected int sellValue;

    [SerializeField]
    protected Vector3 labelWorldOffset;

    [SerializeField]
    protected Vector2 labelScreenOffset;

    [SerializeField]
    protected Vector2Int inventorySize;

    [SerializeField]
    protected float luckDropChanceScalar;

    [SerializeField]
    [DistributionCurve]
    protected AnimationCurve characterLevelDropChanceCurve;

    [SerializeField]
    [DistributionCurve]
    protected AnimationCurve monsterLevelDropChanceCurve;

    public float EvaluateDropChance(float weight, float luck, int cLvl, int mLvl)
    {
        return Mathf.Max(0, (weight + luckDropChanceScalar * luck) * characterLevelDropChanceCurve.Evaluate(cLvl) * monsterLevelDropChanceCurve.Evaluate(mLvl));
    }

    public string DisplayName => displayName;
    public string Description => description;
    public Texture2D Icon => icon;
    
    public Quality ItemQuality => quality;
    public Color ItemQualityColor => QualityColor(quality);
    public string ItemQualityName => quality + "";
    public int Quantity
    {
        get => currentQuantity;
        set
        {
            currentQuantity = Mathf.Clamp(value, 0, QuantityMax);
            if (quality == Quality.Gold) UpdateDisplayName();
            if (currentQuantity <= 0) Destroy(gameObject);
        }
    }
    public int QuantityMax => maxQuantity;
    public float QuantityFraction
    {
        get => (float)Quantity / (float)QuantityMax;
        set => Quantity = (int)(QuantityMax * value);
    }
    public float QuantityPercent
    {
        get => (float)Quantity / (float)QuantityMax * 100f; 
        set => Quantity = (int)(QuantityMax * value / 100f);
    }
    public int QuantityDeficit => QuantityMax - Quantity;
    public bool UseGravity
    {
        get => GetComponent<Rigidbody>().useGravity;
        set => GetComponent<Rigidbody>().useGravity = value;
    }
    public bool IsKinematic
    {
        get => GetComponent<Rigidbody>().isKinematic;
        set => GetComponent<Rigidbody>().isKinematic = value;
    }
    public bool Sellable => sellable;
    public int SellValue => sellValue;
    public bool DisplayQuantity => displayQuantity;
    public int InventorySizeX => inventorySize.x;
    public int InventorySizeY => inventorySize.y;
    public Vector2Int InventorySize => inventorySize;
    public virtual void Awake()
    {
        DynamicObject = GetComponent<DynamicObject>();
        Rigidbody = GetComponent<Rigidbody>();
        outline = GetComponentsInChildren<Outline>();
    }
    public virtual void Start()
    {
        ShowOutline(false);
        UpdateDisplayName();
    }

    public virtual void UpdateDisplayName()
    {
        displayName = baseName;
        if (quality == Quality.Gold) displayName = currentQuantity + " " + baseName;
        if (label != null)
        {
            label.LabelText = displayName;
            label.LabelTextColor = QualityColor(quality);
        }
    }

    public void ShowObjectLabel(bool show)
    {
        if (show)
            Label.gameObject.SetActive(show);
        else
            LabelManager.Remove(this);
    }

    public void ShowOutline(bool show)
    {
        foreach (Outline o in outline)
            o.hideOutline = !show;
    }

    public void ShowLabelText(bool show)
    {
        Label.LabelEnabled = show;
    }

    public void ShowBar(bool show)
    {
        Label.BarEnabled = show;
    }

    public ItemData WriteData()
    {
        return new ItemData
        {
            prefabID = prefabID,

            posX = transform.position.x,
            posY = transform.position.y,
            posZ = transform.position.z,

            rotX = transform.rotation.x,
            rotY = transform.rotation.y,
            rotZ = transform.rotation.z,
            rotW = transform.rotation.w,

            currentQuantity = currentQuantity
        };
    }

    public virtual float GetInteractDistance()
    {
        return 5;
    }

    public void Interact()
    {
        //Debug.Log("Picking...");
    }

    public Vector3 GetGroundPosition()
    {
        return transform.position;
    }

    public Vector3 GetCenterPosition()
    {
        return transform.position;
    }
}
