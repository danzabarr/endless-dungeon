using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{

    [SerializeField]
    private Transform lid;

    [SerializeField]
    private float openAngle, closedAngle, duration;

    private bool open;

    public bool Open
    {
        get => open;
        set
        {
            if (open == value) return;
            open = value;
            StopAllCoroutines();
            StartCoroutine(AnimateTo());
        }
    }
    private float CurrentAngle => lid.localRotation.eulerAngles.x;
    private float TargetAngle => open ? openAngle : closedAngle;
    private float CurrentAngleAbsDifference => Mathf.Abs(Mathf.DeltaAngle(CurrentAngle, TargetAngle));
    private float AngleAbsDifference => Mathf.Abs(Mathf.DeltaAngle(openAngle, closedAngle));
    private float Speed => AngleAbsDifference / duration;

    public override void Interact()
    {
        if (CurrentAngleAbsDifference > 0.0001f)
            return;

        Open = !open;
    }

    IEnumerator AnimateTo()
    {
        while (CurrentAngleAbsDifference > 0.0001f)
        {
            //float signedDifference = Mathf.DeltaAngle(TargetAngle, CurrentAngle);
            //signedDifference = Mathf.Clamp(signedDifference, -Speed * Time.deltaTime, Speed * Time.deltaTime);

//            lid.localRotation = Quaternion.Euler(CurrentAngle + signedDifference, 0, 0);

           // lid.Rotate(signedDifference, 0, 0, Space.Self);


//            Debug.Log(Mathf.MoveTowardsAngle(CurrentAngle, TargetAngle, Speed * Time.deltaTime));
            //lid.localRotation = Quaternion.Euler(Mathf.MoveTowardsAngle(CurrentAngle, TargetAngle, Speed * Time.deltaTime), 0, 0);
            lid.localRotation = Quaternion.Lerp(lid.localRotation, Quaternion.Euler(TargetAngle, 0, 0), Speed * Time.deltaTime / CurrentAngleAbsDifference);
            yield return null;
        }
    }
}
