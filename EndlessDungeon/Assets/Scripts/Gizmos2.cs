using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Gizmos2 
{

    public static void DrawWireCircle(Vector3 center, float radius, float angleStart, float angleDegrees, float edgeSize)
    {
        float circumference = 2 * Mathf.PI * radius * (angleDegrees / 360f);
        int edges = (int) (circumference / edgeSize);
        float edgeLength = angleDegrees / edges;

        float angle0, angle1;
        Vector3 p0 = new Vector3(0, center.y, 0);
        Vector3 p1 = new Vector3(0, center.y, 0);

        for (int i = 0; i < edges; i++)
        {
            angle0 = Mathf.Deg2Rad * (angleStart + (i + 0) * edgeLength);
            angle1 = Mathf.Deg2Rad * (angleStart + (i + 1) * edgeLength);

            p0.x = center.x + Mathf.Cos(angle0) * radius;
            p0.z = center.z + Mathf.Sin(angle0) * radius;

            p1.x = center.x + Mathf.Cos(angle1) * radius;
            p1.z = center.z + Mathf.Sin(angle1) * radius;
            Gizmos.DrawLine(p0, p1);
        }
    }

    public static void DrawWirePolygon(Vector3 center, float radius, float angleStart, float angleDegrees, int edges)
    {
        float edgeLength = angleDegrees / edges;
        float angle0, angle1;
        Vector3 p0 = new Vector3(0, center.y, 0);
        Vector3 p1 = new Vector3(0, center.y, 0);

        for (int i = 0; i < edges; i++)
        {
            angle0 = Mathf.Deg2Rad * (angleStart + (i + 0) * edgeLength);
            angle1 = Mathf.Deg2Rad * (angleStart + (i + 1) * edgeLength);

            p0.x = center.x + Mathf.Cos(angle0) * radius;
            p0.z = center.z + Mathf.Sin(angle0) * radius;

            p1.x = center.x + Mathf.Cos(angle1) * radius;
            p1.z = center.z + Mathf.Sin(angle1) * radius;
            Gizmos.DrawLine(p0, p1);
        }
    }
}
