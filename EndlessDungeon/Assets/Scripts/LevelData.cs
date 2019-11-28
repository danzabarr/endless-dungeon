using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int level;
    public int xSize, zSize;
    public MapNode[,] nodes;
    public LocationData[] roomData, corridorData;
    public ItemData[] itemData;
    public MobData[] mobData;
    
}

