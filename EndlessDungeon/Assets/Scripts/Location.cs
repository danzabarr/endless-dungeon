using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Location : MonoBehaviour, RegisterablePrefab
{
    public enum Visibility
    {
        Hidden,
        ContentsHidden,
        Shown
    }

    private int prefabID;
    public int GetPrefabID() => prefabID;
    public void SetPrefabID(int id) => prefabID = id;

    [SerializeField] [HideInInspector]
    protected int x, z;
    [SerializeField] [HideInInspector]
    protected Units.Rotation rotation;

    [SerializeField]
    protected GameObject contentsContainer;
    [SerializeField]
    protected GameObject wallsContainer;
    [SerializeField]
    protected GameObject floorsContainer;
    public GameObject ContentsContainer => contentsContainer;
    public GameObject WallsContainer => wallsContainer;
    public GameObject FloorsContainer => floorsContainer;


    private List<DynamicObject> dynamicObjects = new List<DynamicObject>();

    public Renderer[] WallRenderers { get; private set; }
    public Renderer[] ContentsRenderers { get; private set; }
    public Renderer[] FloorRenderers { get; private set; }
    public List<DynamicObject> DynamicObjects { get; private set; }
    public Wall[] Walls { get; private set; }

    public int X => x;
    public int Z => z;

    public virtual int MaxX() { return x; }
    public virtual int MaxZ() { return z; }

    //[HideInInspector]
    public Visibility visibility;

    public abstract LocationData.LocationType GetLocationType();

    public void Awake()
    {
        WallRenderers = wallsContainer.GetComponentsInChildren<Renderer>();
        ContentsRenderers = contentsContainer.GetComponentsInChildren<Renderer>();
        FloorRenderers = floorsContainer.GetComponentsInChildren<Renderer>();
        Walls = wallsContainer.GetComponentsInChildren<Wall>();
        DynamicObjects = new List<DynamicObject>();
    }

    public LocationData WriteData()
    {
        LocationData data = new LocationData
        {
            prefabID = prefabID,
            x = x,
            z = z,
            rotation = rotation,
            type = GetLocationType()
        };
        return data;
    }

    public virtual void SetPosition(int x, int z, Units.Rotation rotation)
    {
        this.x = x;
        this.z = z;
        this.rotation = rotation;
    }

    public void SetWallRotations()
    {
        foreach (Wall wall in GetComponentsInChildren<Wall>())
            if (wall != null) wall.SetParentRotation(rotation);
    }
}
