using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableChild : MonoBehaviour, Interactive
{
    [SerializeField]
    public Interactable parent;

    public virtual void Interact()
    {
        parent.Interact();
    }

    public void ShowObjectLabel(bool show)
    {
        parent.ShowObjectLabel(show);
    }

    public void ShowOutline(bool show)
    {
        parent.ShowOutline(show);
    }

    public void ShowLabelText(bool show)
    {
        parent.ShowLabelText(show);
    }

    public void ShowBar(bool show)
    {
        parent.ShowBar(show);
    }

    public Transform GetWorldTransform()
    {
        return parent.GetWorldTransform();
    }

    public Vector3 GetGroundPosition()
    {
        return parent.GetGroundPosition();
    }
    public Vector3 GetCenterPosition()
    {
        return parent.GetCenterPosition();
    }

    public float GetInteractDistance()
    {
        return parent.GetInteractDistance();
    }
}
