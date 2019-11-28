using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Room))]
public class RoomWallsGenerator : MonoBehaviour
{

    [SerializeField]
    private GameObject[] wallPrefabs;

    [SerializeField]
    private GameObject[] doorPrefabs;

    [ContextMenu("GenerateWalls")]
    private void GenerateWalls()
    {
        Room room = GetComponent<Room>();


#if UNITY_EDITOR
        bool[] edges = room.OpenEdges;

        int index = 0;
        int openEdgeIndex = 0;
        room.doors = new Door[room.OpenEdgesLength];

        void Add(Vector3 position, Units.Rotation rotation)
        {
            if (edges[index])
            {
                GameObject door = PrefabUtility.InstantiatePrefab(doorPrefabs[Random.Range(0, doorPrefabs.Length)], room.WallsContainer.transform) as GameObject;
                door.transform.localPosition = position;
                door.transform.localRotation = Quaternion.Euler(0, ((int)rotation + 1) * 90, 0);
                foreach (Wall ssmr in door.GetComponentsInChildren<Wall>())
                    ssmr.Rotation = (Units.Rotation)(((int)ssmr.Rotation + (int)rotation) % 4);
                room.doors[openEdgeIndex] = door.GetComponentInChildren<Door>();
                openEdgeIndex++;
            }
            else
            {
                GameObject wall = PrefabUtility.InstantiatePrefab(wallPrefabs[Random.Range(0, wallPrefabs.Length)], room.WallsContainer.transform) as GameObject;
                wall.transform.localPosition = position;
                wall.transform.localRotation = Quaternion.Euler(0, ((int)rotation + 1) * 90, 0);
                foreach (Wall ssmr in wall.GetComponentsInChildren<Wall>())
                    ssmr.Rotation = (Units.Rotation)(((int)ssmr.Rotation + (int)rotation) % 4);
            }
        }


        for (int i = 0; i < room.ZSize; i++)
        {
            Add(new Vector3(0, 0, 0.5f + i), Units.Rotation.West);//270);  
            index++;
        }

        for (int i = 0; i < room.XSize; i++)
        {
            Add(new Vector3(0.5f + i, 0, room.ZSize), Units.Rotation.North);// 0);
            index++;
        }

        for (int i = 0; i < room.ZSize; i++)
        {
            Add(new Vector3(room.XSize, 0, room.ZSize - 0.5f - i), Units.Rotation.East);// 90);
            index++;
        }

        for (int i = 0; i < room.XSize; i++)
        {
            Add(new Vector3(room.XSize - 0.5f - i, 0, 0), Units.Rotation.South);// 180);
            index++;
        }
#endif
    }
}
