using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLightning : MonoBehaviour
{
    private LineRenderer line;

    public Transform target;
    
    public float duration;

    [SerializeField]
    [Range(0, .25f)]
    private float randomness;
    
    [SerializeField]
    private float targetSegmentLength;

    private float timer;

    public int seed;

    public void Awake()
    {
        line = GetComponent<LineRenderer>();
        seed = Random.Range(-100000, 100000);
    }

    public void Update()
    {
        
        if (duration > 0) timer += Time.deltaTime;
        if (duration > 0 && timer >= duration)
        {
            Destroy(gameObject);
        }
        else
        {
            Generate(Random.Range(-100000, 100000));
        }
    }

    public void Generate(int seed)
    {
        this.seed = seed;
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (!line)
            line = GetComponent<LineRenderer>();

        if (!target)
        {
            line.positionCount = 1;
            line.SetPosition(0, transform.position);
            return;
        }

        

        float totalLength = Vector3.Distance(transform.position, target.position);

        float targetSegmentNumber = totalLength / targetSegmentLength;
        int divisions = Mathf.Clamp(Mathf.CeilToInt(Mathf.Log(targetSegmentNumber, 2)), 0, 9);

//        points = new Vector3[] { Vector3.zero, Vector3.one };
        Vector3[] points = new Vector3[] { transform.position, target.position };
        Vector3[] temp, inter;


        Random.State oldState = Random.state;
        Random.InitState(seed);

        for (int d = 0; d < divisions; d++)
        {
            inter = new Vector3[points.Length - 1];

            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector3 p0 = points[i];
                Vector3 p1 = points[i + 1];
                Vector3 A = (p1 - p0);

                float magnitude = A.magnitude;
                A /= magnitude;

                float du = Vector3.Dot(A, Vector3.up);
                float df = Vector3.Dot(A, Vector3.forward);
                Vector3 v1 = Mathf.Abs(du) < Mathf.Abs(df) ? Vector3.up : Vector3.forward;

                // cross v1 with A. the new vector is perpendicular to both v1 and A.
                Vector3 v2 = Vector3.Cross(v1, A);

                Vector3 m = (p0 + p1) / 2;

                m += Quaternion.AngleAxis(Random.Range(0, 360), A) * v2 * randomness * magnitude;

                inter[i] = m;
            }
            
            temp = new Vector3[points.Length * 2 - 1];

            for (int i = 0; i < points.Length; i++)
            {
                temp[i * 2 + 0] = points[i];
                if (i < points.Length - 1)
                    temp[i * 2 + 1] = inter[i];
            }

            points = temp;
        }

        line.positionCount = points.Length;
        line.SetPositions(points);
        Random.state = oldState;

    }
}
