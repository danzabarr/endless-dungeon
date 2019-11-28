using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SpawnGold", menuName = "Spawn/Gold")]
public class GoldSpawn : ItemSpawn
{
    public int min, max;

    public override ItemObject RandomItem()
    {
        return SpawnGold(Random.Range(min, max));
    }

    public ItemObject SpawnGold(int amount)
    {
        ItemObject prefab;

        if (amount <= 0)
            return null;

        else if (amount < 8)
            prefab = items[0];

        else if (amount < 16)
            prefab = items[1];

        else if (amount < 32)
            prefab = items[2];

        else if (amount < 64)
            prefab = items[3];

        else if (amount < 128)
            prefab = items[4];

        else if (amount < 256)
            prefab = items[5];

        else
            prefab = items[6];

        if (prefab == null)
            return null;

        ItemObject item = Instantiate(prefab);
        item.Quantity = amount;
        return item;
    }
}
