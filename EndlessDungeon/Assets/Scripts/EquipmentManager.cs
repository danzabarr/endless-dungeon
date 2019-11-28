using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private Stats stats;
    private AbilityManager abilities;

    private GameObject mainHandObject, offHandObject;
    private GameObject[] headObjects, bodyObjects, handsObjects, feetObjects;

    public void Awake()
    {
        stats = GetComponent<Stats>();
        abilities = GetComponent<AbilityManager>();
    }

    public void Start()
    {
        GatherObjects();
        UpdateAll();
        RecalculateStats();
    }

    

#if UNITY_EDITOR
    public void Update()
    {
        UpdateMainHand();
        UpdateOffHand();
    }
    public void OnValidate()
    {
        stats = GetComponent<Stats>();
        abilities = GetComponent<AbilityManager>();
        GatherObjects();
        UpdateAll();
        RecalculateStats();
    }
#endif

   
    [ContextMenu("Gather Equipment Objects")]
    private void GatherObjects()
    {
        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>(true))
        {
            if (mesh.name == "MH_Weapon") mainHandObject = mesh.gameObject;
            if (mesh.name == "OH_Weapon") offHandObject = mesh.gameObject;
        }
        List<GameObject> head = new List<GameObject>();
        List<GameObject> body = new List<GameObject>();
        List<GameObject> hands = new List<GameObject>();
        List<GameObject> feet = new List<GameObject>();

        foreach (SkinnedMeshRenderer smr in GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (smr.name.StartsWith("Head.")) head.Add(smr.gameObject);
            else if (smr.name.StartsWith("Body.")) body.Add(smr.gameObject);
            else if (smr.name.StartsWith("Hands.")) hands.Add(smr.gameObject);
            else if (smr.name.StartsWith("Feet.")) feet.Add(smr.gameObject);
        }

        headObjects = head.ToArray();
        bodyObjects = body.ToArray();
        handsObjects = hands.ToArray();
        feetObjects = feet.ToArray();
    }

    [SerializeField]
    [Icon(128, 256)]
    private EquipmentObject mainHand;

    [SerializeField]
    [Icon(128, 256)]
    private EquipmentObject offHand;

    [SerializeField]
    [Icon(128, 128)]
    private EquipmentObject head;

    [SerializeField]
    [Icon(128, 192)]
    private EquipmentObject body;

    [SerializeField]
    [Icon(128, 128)]
    private EquipmentObject hands;

    [SerializeField]
    [Icon(128, 128)]
    private EquipmentObject feet;

    [SerializeField]
    [Icon(64, 64)]
    private EquipmentObject neck;

    [SerializeField]
    [Icon(64, 64)]
    private EquipmentObject finger;

    public EquipmentObject MainHand
    {
        get => mainHand;
        set
        {
            mainHand = value;
            UpdateMainHand();
            RecalculateStats();
        }
    }
    public EquipmentObject OffHand
    {
        get => offHand;
        set
        {
            offHand = value;
            UpdateOffHand();
            RecalculateStats();
        }
    }

    public EquipmentObject Head
    {
        get => head;
        set
        {
            head = value;
            UpdateHead();
            RecalculateStats();
        }
    }

    public EquipmentObject Body
    {
        get => body;
        set
        {
            body = value;
            UpdateBody();
            RecalculateStats();
        }
    }
    public EquipmentObject Hands
    {
        get => hands;
        set
        {
            hands = value;
            UpdateHands();
            RecalculateStats();
        }
    }
    public EquipmentObject Feet
    {
        get => feet;
        set
        {
            feet = value;
            UpdateFeet();
            RecalculateStats();
        }
    }
    public EquipmentObject Neck
    {
        get => neck;
        set
        {
            neck = value;
            RecalculateStats();
        }
    }
    public EquipmentObject Finger
    {
        get => finger;
        set
        {
            finger = value;
            RecalculateStats();
        }
    }
    private void RecalculateStats()
    {
        mainHand?.Recalculate();
        offHand?.Recalculate();
        head?.Recalculate();
        body?.Recalculate();
        hands?.Recalculate();
        feet?.Recalculate();
        neck?.Recalculate();
        finger?.Recalculate();

        if (stats == null)
            return;

        stats.RecalculateGearStats
        (
            mainHand,
            offHand,
            head,
            body,
            hands,
            feet,
            finger,
            neck
        );

        //stats.RecalculateBonusStats();

        abilities?.Equip(stats);
    }

    private void UpdateAll()
    {
        UpdateMainHand();
        UpdateOffHand();
        UpdateHead();
        UpdateBody();
        UpdateHands();
        UpdateFeet();
    }

    private void UpdateMainHand()
    {
        if (mainHandObject == null) return;

        Mesh mesh = mainHand == null ? null : mainHand.HeldItemMesh;
        Vector3 position = mainHand == null ? Vector3.zero : mainHand.HeldItemPosition;
        Vector3 eulerRotation = mainHand == null ? Vector3.zero : mainHand.HeldItemEulerRotation;

        mainHandObject.transform.localPosition = position;
        mainHandObject.transform.localRotation = Quaternion.Euler(eulerRotation);

        foreach (MeshFilter filter in mainHandObject.GetComponentsInChildren<MeshFilter>())
            filter.sharedMesh = mesh;
    }
    private void UpdateOffHand()
    {
        if (offHandObject == null) return;

        Mesh mesh = offHand == null ? null : offHand.HeldItemMesh;
        Vector3 position = offHand == null ? Vector3.zero : offHand.HeldItemPosition;
        Vector3 eulerRotation = offHand == null ? Vector3.zero : offHand.HeldItemEulerRotation;

        offHandObject.transform.localPosition = new Vector3(position.x, position.y, -position.z);
        offHandObject.transform.localRotation = Quaternion.Euler(-eulerRotation.x, -eulerRotation.y, eulerRotation.z);

        foreach (MeshFilter filter in offHandObject.GetComponentsInChildren<MeshFilter>())
            filter.sharedMesh = mesh;
    }

    private void UpdateHead()
    {
        string name = head == null ? "empty" : head.WornItemName;
        foreach (GameObject obj in headObjects)
            obj.SetActive(obj.name == name);
    }
    private void UpdateBody()
    {
        string name = body == null ? "empty" : body.WornItemName;
        foreach (GameObject obj in bodyObjects)
            obj.SetActive(obj.name == name);
    }

    private void UpdateHands()
    {
        string name = hands == null ? "empty" : hands.WornItemName;
        foreach (GameObject obj in handsObjects)
            obj.SetActive(obj.name == name);
    }

    private void UpdateFeet()
    {
        string name = feet == null ? "empty" : feet.WornItemName;
        foreach (GameObject obj in feetObjects)
            obj.SetActive(obj.name == name);
    }
}
