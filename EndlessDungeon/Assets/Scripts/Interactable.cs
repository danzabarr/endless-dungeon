using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Interactable : MonoBehaviour, Interactive
{
    private Outline[] outline;
    private ObjectLabel label;

    private ObjectLabel Label
    {
        get
        {
            label = LabelManager.Add(this);
            label.ZSort = true;
            label.BarEnabled = false;
            label.worldTransform = transform;
            label.worldOffset = labelWorldOffset;
            label.screenOffset = labelScreenOffset;
            label.LabelSupportRichText = labelSupportRichText;
            label.LabelText = labelText;
            label.LabelTextColor = labelTextColor;
            return label;
        }
    }

    [SerializeField]
    private float interactDistance;

    [SerializeField]
    [TextArea]
    private string labelText;
    [SerializeField]
    private Color labelTextColor = Color.white;
    [SerializeField]
    private bool labelSupportRichText;
    [SerializeField]
    private Vector3 labelWorldOffset;
    [SerializeField]
    private Vector2 labelScreenOffset;

    public virtual void Start()
    {
        outline = GetComponentsInChildren<Outline>();
        ShowOutline(false);
    }

    public virtual void Interact()
    {

    }

    public void ShowOutline(bool show)
    {
        foreach (Outline r in outline)
            r.hideOutline = !show;
    }

    public void ShowObjectLabel(bool show)
    {
        if (show)
            Label.gameObject.SetActive(show);
        else
            LabelManager.Remove(this);
    }

    public void ShowLabelText(bool show)
    {
        Label.LabelEnabled = show;
    }

    public void ShowBar(bool show)
    {
        Label.BarEnabled = show;
    }

    public Transform GetWorldTransform()
    {
        return transform;
    }

    public Vector3 GetGroundPosition()
    {
        return GetWorldTransform().position;
    }
    public Vector3 GetCenterPosition()
    {
        return GetWorldTransform().position;
    }

    public float GetInteractDistance()
    {
        return interactDistance;
    }
}
