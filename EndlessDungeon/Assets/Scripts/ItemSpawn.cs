using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SpawnItem", menuName = "Spawn/Item")]
public class ItemSpawn : ScriptableObject, IReadOnlyList<ItemObject>
{
    [SerializeField]
    protected ItemObject[] items;

    public virtual ItemObject RandomItem()
    {
        ItemObject prefab = items[Random.Range(0, items.Length)];
        if (prefab == null) return null;
        ItemObject instance = Instantiate(prefab);
        if (instance is EquipmentObject)
            (instance as EquipmentObject).RollStats();
        return instance;
    }

    public ItemObject this[int index] => items[index];

    public int Count => items.Length;

    public IEnumerator<ItemObject> GetEnumerator()
    {
        return ((IEnumerable<ItemObject>)items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}