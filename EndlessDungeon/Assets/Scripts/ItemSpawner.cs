using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public ItemSpawn items;

    public int index;

    public bool useGravity = true;
    public bool isKinematic = false;

    public ItemObject Spawn()
    {
        ItemObject item = items.RandomItem();
        if (item == null) return null;
        item.transform.SetParent(transform, false);
        item.UseGravity = useGravity;
        item.IsKinematic = isKinematic;
        return item;
    }

    public ItemObject Spawn(int index)
    {
        ItemObject prefab = items[index];
        if (prefab == null) return null;
        ItemObject item =  Instantiate(prefab, transform);
        item.UseGravity = useGravity;
        item.IsKinematic = isKinematic;
        return item;
    }

    [ContextMenu("Spawn Random Item")]
    private void SpawnRandomItem()
    {
        DeleteSpawnedItem();
        Spawn();
    }

    [ContextMenu("Spawn Item")]
    private void SpawnItem()
    {
        DeleteSpawnedItem();
        Spawn(index);
    }

    [ContextMenu("Delete Children")]
    private void DeleteSpawnedItem()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
}
