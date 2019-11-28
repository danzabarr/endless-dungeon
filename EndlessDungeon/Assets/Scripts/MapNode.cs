using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapNode
{
    public int x, z;
    public NodeType type;
    public int locationIndex;
    public byte neighbours;
    public enum NodeType
    {
        Empty,
        Temporary,
        Corridor,
        EntranceInside,
        EntranceOutside,
        Room
    }

    public MapNode(){ }

    public static MapNode Copy(MapNode original)
    {
        if (original == null) return null;
        return new MapNode
        {
            x = original.x,
            z = original.z,
            type = original.type,
            locationIndex = original.locationIndex,
            neighbours = original.neighbours
        };
    }

    public bool this[int index]
    {
        get => (neighbours & (1 << index)) != 0;
        set
        {
            if (value)
                neighbours = (byte)(neighbours | (1 << index));
            else
                neighbours = (byte)(neighbours & ~(1 << index));
        }
    }

    public static Vector2Int[] NeighbourPositions = new Vector2Int[]
    {
        new Vector2Int( 1, 0),
        new Vector2Int( 1,-1),
        new Vector2Int( 0,-1),
        new Vector2Int(-1,-1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int( 0, 1),
        new Vector2Int( 1, 1),
    };

    public Vector2Int GetNeighbourPosition(int direction)
    {
        return new Vector2Int(x, z) + NeighbourPositions[direction];
    }

    public bool GetNeighbour(int direction)
    {
        return (neighbours & (1 << direction)) != 0;
    }

    public void SetNeighbour(int direction)
    {
        neighbours = (byte) (neighbours | (1 << direction));
    }

    public void UnsetNeighbour(int direction)
    {
        neighbours = (byte) (neighbours & ~(1 << direction));
    }

    public int CountNeighbours()
    {
        int count = 0;
        int value = neighbours;
        while (value > 0)
        {                               // until all bits are zero
            if ((value & 1) == 1)       // check lower bit
                count++;
            value >>= 1;                // shift bits, removing lower bit
        }
        return count;
    }

    public int CountCardinalNeighbours()
    {
        int count = 0;
        int value = neighbours;
        while (value > 0)
        {                               // until all bits are zero
            if ((value & 1) == 1)       // check lower bit
                count++;
            value >>= 2;                // shift bits, removing 2 lower bits
        }
        return count;
    }

    #region PathFinding
    [System.NonSerialized]
    public float pathDistance;
    [System.NonSerialized]
    public float crowFliesDistance;
    [System.NonSerialized]
    public float additionalCost;
    [System.NonSerialized]
    public float lastDirection, diagonalness;
    [System.NonSerialized]
    public float xSteps, zSteps;
    [System.NonSerialized]
    public MapNode parent;
    public float Cost => pathDistance + crowFliesDistance + additionalCost + diagonalness + Mathf.Min(xSteps + zSteps);
    #endregion

    public void ClearPathfindindData()
    {
        pathDistance = 0;
        crowFliesDistance = 0;
        additionalCost = 0;
        lastDirection = 0;
        diagonalness = 0;
        xSteps = 0;
        zSteps = 0;
        parent = null;
    }

    

    
}
