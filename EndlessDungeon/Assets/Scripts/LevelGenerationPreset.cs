using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LevelGenerationPreset", menuName = "Level Generation Preset")]
public class LevelGenerationPreset : ScriptableObject
{
    public Vector2Int size;

    [Header("Rooms")]
    public RequiredRoom startRoom;
    public RequiredRoom endRoom;
    public RequiredRoom[] uniqueRooms;

    [System.Serializable]
    public class RequiredRoom
    {
        public Room prefab;
        public Vector2Int[] locations;
    }

    public Room[] randomRooms;
    public int maxRandomRooms;

    [Header("Corridors")]
    public int corridorMaxTries;
    public int corridorMaxDistance;
    public float corridorNewPathCost;
    public float corridorDiagonalness1Cost;
    public float corridorDiagonalness2Cost;

    public Corridor[] corridorFloors;
    public Corridor[] corridorEdges;
    public Corridor[] corridorCorners;
    public Corridor[] corridorPassages;
    public Corridor[] corridorDeadends;

    [Header("Misc Prefabs")]
    public GameObject pillarPrefab;


    [Header("Mob Spawning")]
    public int minCorridorMobSpawns;
    public int maxCorridorMobSpawns;
    public float minCorridorMobDistance;
}
