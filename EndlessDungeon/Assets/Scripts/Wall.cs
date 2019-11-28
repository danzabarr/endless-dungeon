using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField]
    private Units.Rotation rotation;
    [SerializeField]
    private Units.Rotation worldRotation;
    [SerializeField]
    private bool outside;
    public Units.Rotation WorldRotation =>worldRotation;
    public Units.Rotation Rotation { get => rotation; set => rotation = value; }
    public bool Outside => outside;
    public Renderer Renderer { get; private set; }
    public void Awake()
    {
        Renderer = GetComponent<Renderer>();
    }

    public void SetParentRotation(Units.Rotation rotation)
    {
        int integer = (int)rotation;

        integer += (int)this.rotation;
        integer %= 4;
        integer += 4;
        integer %= 4;

        worldRotation = (Units.Rotation)integer;
    }
}
