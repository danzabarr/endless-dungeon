using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class FollowWorldTransform : MonoBehaviour
{
    private Camera main;
    private Canvas canvas;
    private RectTransform rect;

    public Transform worldTransform;
    public Vector3 worldOffset;
    public Vector2 screenOffset;
    public bool clamp;

    public void Awake()
    {
        main = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        rect = transform as RectTransform;
    }

    public void LateUpdate()
    {
        //Vector3 world = worldOffset;
        //if (worldTransform) world += worldTransform.position;

        //SetPosition(rect, world, screenOffset, clamp, main, canvas);
    }

    public static void SetPosition(RectTransform transform, Vector3 world, Vector2 screenOffset, bool clamp, Camera main, Canvas canvas)
    {
        Vector2 screen = main.WorldToScreenPoint(world);

        screen.x -= Screen.width / 2;
        screen.y -= Screen.height / 2;

        screen /= canvas.scaleFactor;

        screen += screenOffset;

        transform.anchoredPosition = screen;

        if (clamp)
        {
            Vector3 pos = transform.localPosition;

            Vector3 minPosition = new Vector2(-Screen.width / 2 / canvas.scaleFactor, -Screen.height / 2 / canvas.scaleFactor) - transform.rect.min;
            Vector3 maxPosition = new Vector2(+Screen.width / 2 / canvas.scaleFactor, +Screen.height / 2 / canvas.scaleFactor) - transform.rect.max;

            pos.x = Mathf.Clamp(transform.localPosition.x, minPosition.x, maxPosition.x);
            pos.y = Mathf.Clamp(transform.localPosition.y, minPosition.y, maxPosition.y);

            transform.localPosition = pos;
        }
    }
}
