using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class LineOfSight : MonoBehaviour
{

    public Vector2[] polygon;
    public Bounds bounds;

    public void OnDrawGizmos()
    {
        if (PolygonIntersectsBounds(polygon, bounds.min, bounds.max, out _))
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.white;

        Gizmos.DrawCube(bounds.center, bounds.size);
        
        if (polygon.Length > 1)
        for (int i = 0; i < polygon.Length; i++)
        {
            Vector2 p0 = polygon[i];
            Vector2 p1 = polygon[(i + 1) % polygon.Length];
            Gizmos.DrawLine(p0, p1);
        }
    }

    public static List<Edge> GenerateEdges(Level level, bool addBoundaryEdges)
    {
        List<Edge> edges = new List<Edge>();
        float xsize = level.XSize;
        float zsize = level.ZSize;
        if (addBoundaryEdges)
        {
            edges.Add(new Edge(new Vector2(-1, 0), new Vector2(xsize + 1, 0)));
            edges.Add(new Edge(new Vector2(xsize, -1), new Vector2(xsize, zsize + 1)));
            edges.Add(new Edge(new Vector2(xsize + 1, zsize), new Vector2(-1, zsize)));
            edges.Add(new Edge(new Vector2(0, zsize + 1), new Vector2(0, -1)));
        }

        bool Different(MapNode a, MapNode b)
        {
            if (a == null || a.type == MapNode.NodeType.Empty || a.type == MapNode.NodeType.Temporary)
            {
                return b != null && (b.type == MapNode.NodeType.Corridor || b.type == MapNode.NodeType.EntranceInside || b.type == MapNode.NodeType.EntranceOutside || b.type == MapNode.NodeType.Room);
            }
            else if (a.type == MapNode.NodeType.Corridor || a.type == MapNode.NodeType.EntranceOutside)
            {
                return b == null || b.type == MapNode.NodeType.Empty || b.type == MapNode.NodeType.Temporary || b.type == MapNode.NodeType.Room || b.type == MapNode.NodeType.EntranceInside;
            }
            else if (a.type == MapNode.NodeType.Room || a.type == MapNode.NodeType.EntranceInside)
            {
                return b == null || b.type == MapNode.NodeType.Empty || b.type == MapNode.NodeType.Temporary || b.type == MapNode.NodeType.Corridor || b.type == MapNode.NodeType.EntranceOutside;
            }
            else return false;
        }

        bool Door(MapNode a, MapNode b)
        {
            if (a == null || b == null)
                return false;

            return (a.type == MapNode.NodeType.EntranceInside && b.type == MapNode.NodeType.EntranceOutside)
                || (b.type == MapNode.NodeType.EntranceInside && a.type == MapNode.NodeType.EntranceOutside);

        }

        for (int z = -1; z < zsize; z++)
        {
            bool started = false;
            int edgeStart = 0;
            for (int x = 0; x < xsize; x++)
            {
                MapNode above = level[x, z];
                MapNode below = level[x, z + 1];
                if (Door(above, below))
                {
                    if (started)
                    {
                        started = false;
                        edges.Add(new Edge(new Vector2(edgeStart, z + 1), new Vector2(x, z + 1)));
                    }

                    Door door = level.GetDoor(above, below, true);
                    if (door != null)
                        edges.Add(new Edge(new Vector2(x, z + 1), new Vector2(x + 1, z + 1), door));
                }
                else if (Different(above, below))
                {
                    if (!started)
                    {
                        started = true;
                        edgeStart = x;
                    }
                }
                else
                {
                    if (started)
                    {
                        started = false;
                        edges.Add(new Edge(new Vector2(edgeStart, z + 1), new Vector2(x, z + 1)));
                    }
                }
            }
            if (started)
            {
                started = false;
                edges.Add(new Edge(new Vector2(edgeStart, z + 1), new Vector2(xsize, z + 1)));
            }
        }


        for (int x = -1; x < xsize; x++)
        {
            bool started = false;
            int edgeStart = 0;
            for (int z = 0; z < zsize; z++)
            {
                MapNode above = level[x, z];
                MapNode below = level[x + 1, z];
                if (Door(above, below))
                {
                    if (started)
                    {
                        started = false;
                        edges.Add(new Edge(new Vector2(x + 1, edgeStart), new Vector2(x + 1, z)));
                    }

                    Door door = level.GetDoor(above, below, true);
                    if (door != null)
                        edges.Add(new Edge(new Vector2(x + 1, z), new Vector2(x + 1, z + 1), door));
                }
                else if(Different(above, below))
                {
                    if (!started)
                    {
                        started = true;
                        edgeStart = z;
                    }
                }
                else
                {
                    if (started)
                    {
                        started = false;
                        edges.Add(new Edge(new Vector2(x + 1, edgeStart), new Vector2(x + 1, z)));
                    }
                }
            }
            if (started)
            {
                started = false;
                edges.Add(new Edge(new Vector2(x + 1, edgeStart), new Vector2(x + 1, zsize)));
            }
        }

        return edges;
    }


    public static Mesh GenerateMesh(Mesh mesh, Vector2 center, Vector2[] visibleArea)
    {
        if (visibleArea == null)
            return null;

        if (mesh == null)
            mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        vertices.Add(center);

        for (int i = 0; i < visibleArea.Length; i++)
        {
            vertices.Add(visibleArea[i]);
            triangles.Add(0);

            if (i >= visibleArea.Length - 1)
            {
                triangles.Add(i + 1);
                triangles.Add(1);
            }
            else
            {
                triangles.Add(i + 1);
                triangles.Add(i + 2);
            }
        }


        mesh.triangles = new int[] { };

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    public static bool CircleContainsPoint(Vector2 center, float radius, Vector2 testPoint)
    {
        return SqDist(center, testPoint) <= radius * radius;
    }

    public static bool PolygonContainsPoint(Vector2[] polygon, Vector2 testPoint)
    {
        bool result = false;
        int j = polygon.Length - 1;
        for (int i = 0; i < polygon.Length; i++)
        {
            if (polygon[i].y < testPoint.y && polygon[j].y >= testPoint.y || polygon[j].y < testPoint.y && polygon[i].y >= testPoint.y)
            {
                if (polygon[i].x + (testPoint.y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < testPoint.x)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }

    public static bool CircleIntersectsBounds(Vector2 center, float radius, Vector2 boundsMin, Vector2 boundsMax)
    {
        Rect rect1 = new Rect(boundsMin.x, boundsMin.y - radius, boundsMax.x - boundsMin.x, boundsMax.y - boundsMin.y + radius * 2);
        Rect rect2 = new Rect(boundsMin.x - radius, boundsMin.y, boundsMax.x - boundsMin.x + radius * 2, boundsMax.y - boundsMin.y);

        if (rect1.Contains(center))
            return true;

        if (rect2.Contains(center))
            return true;

        if (CircleContainsPoint(boundsMin, radius, center))
            return true;

        if (CircleContainsPoint(boundsMax, radius, center))
            return true;

        if (CircleContainsPoint(new Vector2(boundsMin.x, boundsMax.y), radius, center))
            return true;

        if (CircleContainsPoint(new Vector2(boundsMax.x, boundsMin.y), radius, center))
            return true;

        return false;
    }

    public enum PolygonBoundsIntersectionType
    {
        None,
        BoundsContainsPolygonPoint,
        PolygonContainsBoundsCorner,
        LineSegmentsIntersect
    }


    public static bool PolygonIntersectsBounds(Vector2[] polygon, Vector2 boundsMin, Vector2 boundsMax, out PolygonBoundsIntersectionType intersectionType)
    {
        Vector2 c00 = new Vector3(boundsMin.x, boundsMin.y);
        Vector2 c01 = new Vector3(boundsMin.x, boundsMax.y);
        Vector2 c11 = new Vector3(boundsMax.x, boundsMax.y);
        Vector2 c10 = new Vector3(boundsMax.x, boundsMin.y);

        intersectionType = PolygonBoundsIntersectionType.BoundsContainsPolygonPoint;

        foreach (Vector2 p in polygon)
        {
            if (p.x > boundsMin.x && p.x <= boundsMax.x &&
                p.y > boundsMin.y && p.y <= boundsMax.y)
            {
                return true;
            }
        }

        intersectionType = PolygonBoundsIntersectionType.PolygonContainsBoundsCorner;

        if (PolygonContainsPoint(polygon, c00))
            return true;

        if (PolygonContainsPoint(polygon, c01))
            return true;

        if (PolygonContainsPoint(polygon, c11))
            return true;

        if (PolygonContainsPoint(polygon, c10))
            return true;

        intersectionType = PolygonBoundsIntersectionType.LineSegmentsIntersect;

        if (polygon.Length > 1)
        for (int i = 0; i < polygon.Length; i++)
        {
            Vector2 p0 = polygon[i];
            Vector2 p1 = polygon[(i + 1) % polygon.Length];

            if (LineSegmentIntersection(p0, p1, c00, c01, out _))
                return true;

            if (LineSegmentIntersection(p0, p1, c01, c11, out _))
                return true;

            if (LineSegmentIntersection(p0, p1, c11, c10, out _))
                return true;

            if (LineSegmentIntersection(p0, p1, c10, c00, out _))
                return true;
        }

        intersectionType = PolygonBoundsIntersectionType.None;
        return false;
    }

    public static float SqDist(Vector2 v1, Vector2 v2)
    {
        return (v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y);
    }

    public static readonly float TAU = Mathf.PI * 2;

    private static List<Vector3> directions = new List<Vector3>();
    private static List<Edge> edgesCopy = new List<Edge>();
    private static int SortByAngle(Vector3 p0, Vector3 p1)
    {
        if (p0.z < p1.z) return 1;
        if (p0.z > p1.z) return -1;
        return 0;
    }
    public static Vector2[] CalculateVisibleArea(List<Edge> edges, bool addRadialEdges, Vector2 origin, float radius, int circumferenceResolution /* could include angle and arc */)
    {

        float radiusSq = radius * radius;

        directions.Clear();
        edgesCopy.Clear();

        foreach (Edge edge in edges)
            if (edge.Intersects(origin, radius + 1))
                if (edge.door == null || !edge.door.Open)
                    edgesCopy.Add(edge);

        edges = edgesCopy;

        float epsilon = 0.0001f;
        void AddEndPoint(Vector2 point, bool threePoints, bool checkRadius = true)
        {
            if (!addRadialEdges || !checkRadius || SqDist(origin, point) <= radiusSq)
            {
                float angle1 = Mathf.Atan2(point.y - origin.y, point.x - origin.x);
                angle1 = (angle1 % TAU + TAU) % TAU;
                directions.Add(new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), angle1));

                if (threePoints)
                {
                    float angle0 = angle1 - epsilon;
                    angle0 = (angle0 % TAU + TAU) % TAU;
                    directions.Add(new Vector3(Mathf.Cos(angle0), Mathf.Sin(angle0), angle0));

                    float angle2 = angle1 + epsilon;
                    angle2 = (angle2 % TAU + TAU) % TAU;
                    directions.Add(new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), angle2));
                }
            }
        }

        for (int i = 4; i < edges.Count; i++)
        {
            AddEndPoint(edges[i].p1, true);
            AddEndPoint(edges[i].p2, true);
        }

        if (addRadialEdges)
        {
            for (int i = 0; i < circumferenceResolution; i++)
            {
                float angle0 = TAU / circumferenceResolution * (i + 0) - TAU * 0.01f;
                float angle1 = TAU / circumferenceResolution * (i + 1) + TAU * 0.01f;
                Vector2 v0 = origin + new Vector2(Mathf.Cos(angle0), Mathf.Sin(angle0)) * radius;
                Vector2 v1 = origin + new Vector2(Mathf.Cos(angle1), Mathf.Sin(angle1)) * radius;

                AddEndPoint(v0, false, false);
                edges.Add(new Edge(v0, v1));
            }

            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = i + 1; j < edges.Count; j++)
                {
                    if (edges[i].Intersects(edges[j], out Vector2 intersection))
                    {
                        AddEndPoint(intersection, false, false);
                    }
                }
            }
        }

        

        directions.Sort(SortByAngle);

        List<Vector2> intersections = new List<Vector2>();

        bool RayCast(Vector2 direction, out Vector2 intersection)
        {
            float closest = float.MaxValue;
            intersection = Vector2.zero;
            bool hit = false;

            foreach (Edge edge in edges)
            {
                if (LineRayIntersection(origin, direction, edge.p1, edge.p2, out Vector2 i, out float d))
                {
                    if (d < closest)
                    {
                        closest = d;
                        intersection = i;
                        hit = true;
                    }
                }
            }
            if (!hit)
            {
               // Debug.Log("Unbelievable... failed to find an intersection. " + message);
            }
            return hit;
        }

        //foreach(Vector3 direction in directions)
        for (int i = 0; i < directions.Count; i++)
        {
            Vector3 direction = directions[i];
            //, "index: " + i + ", direction: " + direction
            if (RayCast(direction, out Vector2 intersection))
            {
                intersections.Add(intersection);
            }
            else intersections.Add(direction);
        }
        return intersections.ToArray();
    }

    public static bool LineRayIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 point1, Vector2 point2, out Vector2 intersection, out float distance)
    {
        Vector2 v1 = rayOrigin - point1;
        Vector2 v2 = point2 - point1;
        Vector2 v3 = new Vector2(-rayDirection.y, rayDirection.x);

        float dot = v2.Dot(v3);
        if (Mathf.Abs(dot) < 0.0000001f)
        {
            intersection = Vector2.zero;
            distance = -1;
            return false;
        }

        float t1 = v2.Cross(v1) / dot;
        float t2 = v1.Dot(v3) / dot;

        if (t1 >= 0.0 && (t2 >= 0.0 && t2 <= 1.0))
        {
            intersection = rayOrigin + rayDirection * t1;
            distance = t1;
            return true;
        }

        intersection = Vector2.zero;
        distance = -1;
        return false;
    }

    public static bool LineSegmentIntersection(Vector2 l1Start, Vector2 l1End, Vector2 l2Start, Vector2 l2End, out Vector2 intersection)
    {
        intersection = Vector3.zero;
        float deltaACy = l1Start.y - l2Start.y;
        float deltaDCx = l2End.x - l2Start.x;
        float deltaACx = l1Start.x - l2Start.x;
        float deltaDCy = l2End.y - l2Start.y;
        float deltaBAx = l1End.x - l1Start.x;
        float deltaBAy = l1End.y - l1Start.y;

        float denominator = deltaBAx * deltaDCy - deltaBAy * deltaDCx;
        float numerator = deltaACy * deltaDCx - deltaACx * deltaDCy;

        if (denominator == 0)
        {
            return false;
            if (numerator == 0)
            {
                // collinear. Potentially infinite intersection points.
                // Check and return one of them.
                if (l1Start.x >= l2Start.x && l1Start.x <= l2End.x)
                {
                    intersection = l1Start;
                    return true;
                }
                else if (l2Start.x >= l1Start.x && l2Start.x <= l1End.x)
                {
                    intersection = l2Start;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            { // parallel
                return false;
            }
        }

        float r = numerator / denominator;
        if (r < 0 || r > 1)
        {
            return false;
        }

        float s = (deltaACy * deltaBAx - deltaACx * deltaBAy) / denominator;
        if (s < 0 || s > 1)
        {
            return false;
        }

        intersection = new Vector2((float)(l1Start.x + r * deltaBAx), (float)(l1Start.y + r * deltaBAy));
        return true;
    }
}
