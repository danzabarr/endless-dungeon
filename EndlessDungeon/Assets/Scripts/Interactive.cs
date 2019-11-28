using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactive 
{
    void Interact();
    Vector3 GetGroundPosition();
    Vector3 GetCenterPosition();
    float GetInteractDistance();
    void ShowObjectLabel(bool show);
    void ShowOutline(bool show);
    void ShowLabelText(bool show);
    void ShowBar(bool show);

}
