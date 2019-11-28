using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MobSpawner : MonoBehaviour
{
    public float range = 10.0f;

    public MobPack[] packs;

    public bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 50; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    [ContextMenu("Spawn")]
    public List<Mob> Spawn()
    {
        List<Mob> list = new List<Mob>();

        MobPack pack = packs[Random.Range(0, packs.Length)];

        foreach (Mob prefab in pack)
        {
            if (RandomPoint(transform.position, range, out Vector3 point))
            {
                Mob mob = Instantiate(prefab, transform);
                mob.Teleport(point);
                mob.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                list.Add(mob);
            }
            else
                Debug.Log("Failed to spawn mob");
        }
        return list;
    }
}
