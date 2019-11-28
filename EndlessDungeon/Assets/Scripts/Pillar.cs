using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour, RegisterablePrefab
{
    private int prefabID;
    public int GetPrefabID() => prefabID;
    public void SetPrefabID(int id) => prefabID = id;
}
