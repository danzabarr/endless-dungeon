using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLayers : MonoBehaviour
{

    public Material shadowMaterial;
    public Material ssaoMaterial;

    public bool addShadow;
    public bool addSSAO;

    [ContextMenu("Add Multi Layers")]
    public void AddMultiLayers()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        Renderer child;
        if (addShadow)
        {
            child = Instantiate(renderer, transform);
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;
            child.sharedMaterial = shadowMaterial;
            child.receiveShadows = false;
            child.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            child.gameObject.layer = LayerMask.NameToLayer("Shadows");
            child.gameObject.name = "Shadow";
            DestroyImmediate(child.GetComponent<MultiLayers>());

            while (child.transform.childCount > 0)
                DestroyImmediate(child.transform.GetChild(0).gameObject);
        }
   
        if (addSSAO)
        {
            child = Instantiate(renderer, transform);
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;
            child.sharedMaterial = ssaoMaterial;
            child.receiveShadows = false;
            child.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            child.gameObject.layer = LayerMask.NameToLayer("SSAO");
            child.gameObject.name = "SSAO";
            DestroyImmediate(child.GetComponent<MultiLayers>());

            while (child.transform.childCount > 0)
                DestroyImmediate(child.transform.GetChild(0).gameObject);
        }

        DestroyImmediate(this);
    }
}
