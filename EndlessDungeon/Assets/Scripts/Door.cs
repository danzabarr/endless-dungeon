using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField]
    private float openAngle, closedAngle, duration;

    private bool open;

    private int delay;

    [SerializeField]
    private Transform hingedTransform;

    private Coroutine animation;

    public bool Open
    {
        get => open;
        set
        {
            if (open == value) return;
            open = value;

            if (animation != null) StopCoroutine(animation);
            animation = StartCoroutine(AnimateTo());
        }
    }

    private float CurrentAngle => hingedTransform.localRotation.eulerAngles.y;
    private float TargetAngle => open ? openAngle : closedAngle;
    private float CurrentAngleAbsDifference => Mathf.Abs(Mathf.DeltaAngle(CurrentAngle, TargetAngle));
    private float AngleAbsDifference => Mathf.Abs(Mathf.DeltaAngle(openAngle, closedAngle));
    private float Speed => AngleAbsDifference / duration;

    IEnumerator AnimateTo()
    {
        while(CurrentAngleAbsDifference > 0.0001f)
        {
            hingedTransform.localRotation = Quaternion.Euler(0, Mathf.MoveTowardsAngle(CurrentAngle, TargetAngle, Speed * Time.deltaTime), 0);
            delay--;
            yield return null;
        }
    }

    public override void Interact()
    {
        if (CurrentAngleAbsDifference > 0.0001f)
            return;
        delay = 10;
        Open = !open;
    }
}
