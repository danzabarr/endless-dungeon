using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropper : MonoBehaviour
{
    public float height;
    public int amount;

    public GameObject prefab;

    [ContextMenu("Drop")]
    public void Drop()
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject drop = 
            Instantiate(prefab, transform);
            drop.transform.localPosition = new Vector3(0, height, 0);
        }
    }
}
