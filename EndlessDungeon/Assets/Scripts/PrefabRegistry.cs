using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabRegistry : MonoBehaviour
{
    private static PrefabRegistry instance;
    public static PrefabRegistry Instance => instance;
    public void Awake()
    {
        instance = this;
        ApplyPrefabIDs();
    }
    public void OnEnable()
    {
        instance = this;
    }

    [ContextMenu("Apply Prefab IDs")]
    public void ApplyPrefabIDs()
    {
        rooms.Load();
        corridors.Load();
        mobs.Load();
        items.Load();
    }

    [System.Serializable]
    public abstract class Register<T> where T : Object, RegisterablePrefab
    {
        public string resourcesPath;
        public T[] prefabs;

        public void Load()
        {
            prefabs = Resources.LoadAll<T>(resourcesPath);
            for (int i = 0; i < prefabs.Length; i++)
            {
                prefabs[i].SetPrefabID(i);
#if UNITY_EDITOR
                EditorUtility.SetDirty(prefabs[i]);
#endif
            }
        }
    }

    #region Rooms
    [System.Serializable]
    public class RoomRegister : Register<Room> { }

    [SerializeField]
    private RoomRegister rooms;

    public Room Room(int prefabID)
    {
        return rooms.prefabs[prefabID];
    }
    #endregion

    #region Corridors
    [System.Serializable]
    public class CorridorRegister : Register<Corridor> { }

    [SerializeField]
    private CorridorRegister corridors;

    public Corridor Corridor(int prefabID)
    {
        return corridors.prefabs[prefabID];
    }
    #endregion

    #region Mobs
    [System.Serializable]
    public class MobRegister : Register<Mob> { }

    [SerializeField]
    private MobRegister mobs;

    public Mob Mob(int prefabID)
    {
        return mobs.prefabs[prefabID];
    }
    #endregion

    #region Items
    [System.Serializable]
    public class ItemRegister : Register<ItemObject> { }

    [SerializeField]
    private ItemRegister items;

    public ItemObject Item(int prefabID)
    {
        return items.prefabs[prefabID];
    }
    #endregion
}
