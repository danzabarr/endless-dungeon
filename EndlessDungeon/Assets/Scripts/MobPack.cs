using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "MobPack", menuName = "Mob Pack")]
public class MobPack : ScriptableObject, IReadOnlyList<Mob>
{
    [SerializeField]
    private Mob[] mobs;

    public Mob this[int index] => mobs[index];

    public int Count => mobs.Length;

    public IEnumerator<Mob> GetEnumerator()
    {
        return ((IEnumerable<Mob>) mobs).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return mobs.GetEnumerator();
    }
}
