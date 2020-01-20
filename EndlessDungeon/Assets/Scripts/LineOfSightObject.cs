using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSightObject : MonoBehaviour
{
    public Color boundsColor = new Color(1, .1f, 1, .2f);

    public Renderer[] renderers;
    public Bounds Bounds {
        get
        {
            Bounds b = default;


            if (renderers.Length > 0 && renderers[0] != null)
            {
                b = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                    b.Encapsulate(renderers[i].bounds);
            }
            return b;
        }

    }

    public void OnEnable()
    {
        if (renderers.Length < 1)
        {
            Renderer get = GetComponent<Renderer>();
            if (get)
            {
                renderers = new Renderer[1];
                renderers[0] = get;
            }
        }
    }

    private bool shown;

    public void Show()
    {
        int interactiveLayer = LayerMask.NameToLayer("Interactive");
        int mobsLayer = LayerMask.NameToLayer("Mobs");
        int defaultLayer = LayerMask.NameToLayer("Default");
        int ssaoLayer = LayerMask.NameToLayer("SSAO");

        shown = true;
        foreach(Renderer renderer in renderers)
        {
            renderer.enabled = true;

            if (renderer.CompareTag("Interactive"))
                renderer.gameObject.layer = interactiveLayer;
            else if (renderer.CompareTag("Mob"))
                renderer.gameObject.layer = mobsLayer;
            else if (renderer.CompareTag("SSAO"))
                renderer.gameObject.layer = ssaoLayer;
            else
                renderer.gameObject.layer = defaultLayer;
        }
    }

    public void Hide()
    {
        shown = false;
        int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;

            renderer.gameObject.layer = ignoreRaycastLayer;
        }
    }

    public void OnDrawGizmos()
    {
        if (shown)
            Gizmos.color = boundsColor;
        else
            Gizmos.color = new Color(1, 1, 1, .2f);

        Bounds bounds = Bounds;
        float lineOfSightObjectInset = Level.Instance.lineOfSightObjectInset;
        bounds.size = new Vector3(Mathf.Max(bounds.size.x + lineOfSightObjectInset, 0), bounds.size.y, Mathf.Max(bounds.size.z + lineOfSightObjectInset, 0));

        Gizmos.DrawCube(Bounds.center, bounds.size);
    }
}
