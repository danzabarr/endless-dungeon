using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBar : MonoBehaviour
{
    [SerializeField]
    private BuffButton prefab;

    public void AddBuff(BuffInstance buff, Color borderColor)
    {
        Instantiate(prefab, transform).Init(buff, borderColor);
    }
}
