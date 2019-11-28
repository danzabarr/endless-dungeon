using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteract : MonoBehaviour
{
    public float radius;
    public Vector3 center;
    public float angleStart, angleDegrees;
    public float resolution;

    public Vector3 mouseOnPlane;
    public Vector3 mouseDirection;

    public RaycastHit hitInfo;
    public Mob target;

    public Vector3 castTarget;
    public Vector3 castDirection;

    public void Awake()
    {
        
    }

    public Vector3 Center => transform.TransformPoint(center);


    public void Update()
    {
        Vector3 origin = Center;
        mouseOnPlane = ScreenPointToRayPlaneIntersection(Input.mousePosition, origin.y, Camera.main);
        Vector3 direction = mouseOnPlane - origin;
        direction.Normalize();
        mouseDirection = direction;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask layerMask = LayerMask.GetMask("Default", "Walls", "Interactive");
        float maxDistance = 150;

        castTarget = mouseOnPlane;
        castDirection = mouseDirection;

        if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
        {
            target = hitInfo.transform.GetComponent<Mob>();
            if (target)
            {
                castTarget = target.GetCenterPosition();
                castDirection = castTarget - origin;
                castDirection.Normalize();
            }
        }
    }

    public static Vector3 ScreenPointToRayPlaneIntersection(Vector3 screenPos, float y, Camera camera)
    {
        Vector3 hit = Vector3.zero;
        Ray ray = camera.ScreenPointToRay(screenPos);
        if (new Plane(Vector3.up, new Vector3(0, y, 0)).Raycast(ray, out float distance))
            hit = ray.GetPoint(distance);
        return hit;
    }

    public void OnDrawGizmos()
    {
        Vector3 origin = Center;
        Gizmos.color = Color.grey;
        Gizmos2.DrawWireCircle(origin, radius, angleStart, angleDegrees, resolution);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, castTarget);
    }
}
