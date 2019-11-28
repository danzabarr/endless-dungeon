using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DynamicObject : MonoBehaviour
{
    [DebugOnly]
    public int nodeX = -1, nodeZ = -1;
    [DebugOnly]
    public int locationIndex = -1;
    [DebugOnly]
    public MapNode.NodeType nodeType;
    [DebugOnly]
    public bool dirty = true;

    protected Rigidbody rb;
    public Renderer[] Renderers { get; private set; }
    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Renderers = GetComponentsInChildren<Renderer>(true);
    }
    public virtual void Update()
    {
        if (rb && !rb.IsSleeping())
            dirty = true;

        if (dirty || rb == null)
            Level.Instance.AssignObjectLocation(this);
    }
}