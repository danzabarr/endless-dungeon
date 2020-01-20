using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Room : Location
{
    [SerializeField]
    private int xSize, zSize;
    [SerializeField]
    private bool rotatable;
    [SerializeField]
    private int[] openEdges;
    [SerializeField][HideInInspector]
    public List<MapNode> roomNodes = new List<MapNode>();
    [SerializeField][HideInInspector]
    public List<MapNode> entranceNodes = new List<MapNode>();
    [SerializeField][HideInInspector]
    public List<Room> connections = new List<Room>();
    [SerializeField]
    //[HideInInspector]
    public Door[] doors;

    [SerializeField]
    private Transform playerSpawnPoint;

    public void MoveUnitToSpawn(Unit unit)
    {
        unit.Teleport(playerSpawnPoint.position);
        unit.SetRotation(playerSpawnPoint.rotation);
    }

    public override int MaxX() { return x + XSize; }
    public override int MaxZ() { return z + ZSize; }
    public int XSize => (int)rotation % 2 == 0 ? xSize : zSize;
    public int ZSize => (int)rotation % 2 == 0 ? zSize : xSize;
    public bool Rotatable => rotatable;
    public Units.Rotation Rotation => rotation;
    public int IntegerRotation => (int)rotation;
    public int Circumference => XSize * 2 + ZSize * 2;

    public bool[] OpenEdges
    {
        get
        {
            bool[] edges = new bool[Circumference];

            foreach (int e in openEdges)
                if (e >= 0 && e < edges.Length)
                    edges[e] = true;

            return edges;
        }
    }

    public int OpenEdgesLength => openEdges.Length;

    public override LocationData.LocationType GetLocationType()
    {
        return LocationData.LocationType.Corridor;
    }

    public override void SetPosition(int x, int z, Units.Rotation rotation)
    {
        this.x = x;
        this.z = z;
        this.rotation = rotation;

        if (rotation == Units.Rotation.North) x += zSize;
        else if (rotation == Units.Rotation.West)
        {
            x += xSize;
            z += zSize;
        }
        else if (rotation == Units.Rotation.South) z += xSize;

        transform.localRotation = Quaternion.Euler(0, 90 * (int)rotation, 0);
        transform.localPosition = new Vector3(x, 0, z);

        SetWallRotations();
    }

    public int GetEdge(Vector2Int inside, Vector2Int outside)
    {
        bool[] openEdges = OpenEdges;

        for (int edge = 0; edge < Circumference; edge++)
        {
            if (!openEdges[edge])
                continue;

            GetEntrance(edge, out Vector2Int i, out Vector2Int o, out _);
            if (i == inside && o == outside)
                return edge;
        }

        return -1;
    }
    public Door GetDoor(Vector2Int inside, Vector2Int outside)
    {
        int edge = GetEdge(inside, outside);
        if (edge == -1)
            return null;

        for (int i = 0; i < openEdges.Length; i++)
            if (openEdges[i] == edge)
                return doors[i];
        return null;
    }

    public void GetEntrance(int edge, out Vector2Int inside, out Vector2Int outside, out int direction)
    {
        int circumference = Circumference;

        edge %= circumference;
        edge += circumference;
        edge %= circumference;

        int rotation = (int)this.rotation;

        if (rotation > 0) edge += xSize;
        if (rotation > 1) edge += zSize;
        if (rotation > 2) edge += xSize;

        edge %= circumference;

        if (edge < ZSize)
        {
            inside = new Vector2Int(x, z + edge);
            outside = new Vector2Int(x - 1, z + edge);
            direction = 4;
        }
        else if (edge < ZSize + XSize)
        {
            inside = new Vector2Int(x + edge - ZSize, z + ZSize - 1);
            outside = new Vector2Int(x + edge - ZSize, z + ZSize);
            direction = 6;
        }
        else if (edge < ZSize + XSize + ZSize)
        {
            inside = new Vector2Int(x + XSize - 1, z + ZSize - (edge - XSize - ZSize) - 1);
            outside = new Vector2Int(x + XSize, z + ZSize - (edge - XSize - ZSize) - 1);
            direction = 0;
        }
        else
        {
            inside = new Vector2Int(x + XSize - (edge - ZSize - XSize - ZSize) - 1, z);
            outside = new Vector2Int(x + XSize - (edge - ZSize - XSize - ZSize) - 1, z - 1);
            direction = 2;
        }
    }
}
