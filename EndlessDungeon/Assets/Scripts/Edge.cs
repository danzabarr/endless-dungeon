using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Edge 
{
    public Vector2 p1, p2;

    public Door door;

    public Edge(Vector2 p1, Vector2 p2, Door door = null)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.door = door;
    }

    public static bool operator ==(Edge e1, Edge e2)
    {
        return (e1.p1 == e2.p1 && e1.p2 == e2.p2)
            || (e1.p1 == e2.p2 && e1.p2 == e2.p1);
    }

    public static bool operator !=(Edge e1, Edge e2)
    {
        return !(e1 == e2);
    }

    public bool Intersects(Edge e2, out Vector2 intersection)
    {
        return LineSegmentIntersection(p1, p2, e2.p1, e2.p2, out intersection);
    }

    public bool Intersects(Vector2 circleCenter, float circleRadius)
    {
        return LineSegmentCircleIntersection(p1, p2, circleCenter, circleRadius);
    }

    private static bool LineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
    {
        // Get the segments' parameters.
        float dx12 = p2.x - p1.x;
        float dy12 = p2.y - p1.y;
        float dx34 = p4.x - p3.x;
        float dy34 = p4.y - p3.y;

        // Solve for t1 and t2
        float denominator = (dy12 * dx34 - dx12 * dy34);

        float t1 =
            ((p1.x - p3.x) * dy34 + (p3.y - p1.y) * dx34)
                / denominator;
        if (float.IsInfinity(t1))
        {
            // The lines are parallel (or close enough to it).
            intersection = Vector2.zero;
            return false;
        }

        float t2 =
            ((p3.x - p1.x) * dy12 + (p1.y - p3.y) * dx12)
                / -denominator;

        // Find the point of intersection.
        intersection = new Vector2(p1.x + dx12 * t1, p1.y + dy12 * t1);

        // The segments intersect if t1 and t2 are between 0 and 1.
        return
            ((t1 >= 0) && (t1 <= 1) &&
             (t2 >= 0) && (t2 <= 1));
    }
    public static float SqDist(Vector2 v1, Vector2 v2)
    {
        return (v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y);
    }
    public static bool PointInsideCircle(Vector2 point, Vector2 circleCenter, float circleRadius)
    {
        return SqDist(point, circleCenter) <= circleRadius * circleRadius;
    }

    public static bool LineSegmentCircleIntersection(Vector2 line1, Vector2 line2, Vector2 circleCenter, float circleRadius)
    {
        return PointInsideCircle(line1, circleCenter, circleRadius)
            || PointInsideCircle(line2, circleCenter, circleRadius)
            || ShortestSquareDistance(circleCenter, line1, line2) <= circleRadius * circleRadius;
    }

    public static float ShortestSquareDistance(Vector2 point, Vector2 line1, Vector2 line2)
    {
        float A = point.x - line1.x;
        float B = point.y - line1.y;
        float C = line2.x - line1.x;
        float D = line2.y - line1.y;

        float dot = A * C + B * D;
        float len_sq = C * C + D * D;
        float param = -1;
        if (len_sq != 0) //in case of 0 length line
            param = dot / len_sq;

        float xx, yy;

        if (param < 0)
        {
            xx = line1.x;
            yy = line1.y;
        }
        else if (param > 1)
        {
            xx = line2.x;
            yy = line2.y;
        }
        else
        {
            xx = line1.x + param * C;
            yy = line1.y + param * D;
        }

        float dx = point.x - xx;
        float dy = point.y - yy;
        return dx * dx + dy * dy;
    }
}
