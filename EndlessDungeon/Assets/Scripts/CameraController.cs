using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player, pointLight;

    [Header("Camera Follow Settings")]
    public float followDistance;
    public Vector3 multiply, offset;


    [Header("Point Light Settings")]
    public float minDistance;
    public float maxDistance;
    public float radius;
    [Range(1, 10)]
    public float followAmount;

    public int positionCount = 10;

    private List<Vector3> positions = new List<Vector3>();
    private List<float> times = new List<float>();

    [ContextMenu("Follow Player")]
    public void FollowPlayer()
    {
        Vector3 playerPosition = new Vector3(player.position.x * multiply.x, player.position.y * multiply.y, player.position.z * multiply.z) + offset;


        Vector3 targetPosition = transform.position;

        targetPosition = playerPosition;
        targetPosition -= transform.forward * followDistance;

        /*
        positions.Add(targetPosition);
        times.Add(Time.deltaTime);

        while (positions.Count > positionCount)
            positions.RemoveAt(0);
        while (times.Count > positionCount)
            times.RemoveAt(0);

        Vector3 averagePosition = Vector3.zero;
        float sum = 0;
        for (int i = 0; i < positions.Count; i++)
        {
            averagePosition += positions[i] * times[i];
            sum += times[i];
        }
        
        averagePosition /= sum;
        */
        transform.position = targetPosition;


//        transform.position += (targetPosition - transform.position) / followAmount;

        Vector3 origin = playerPosition;
        Vector3 direction = transform.position - playerPosition;

        LayerMask layerMask = LayerMask.GetMask("Default", "Walls", "Ignore Raycast");

        float distance = maxDistance;

        if (Physics.SphereCast(origin, radius, direction, out RaycastHit hitInfo, maxDistance, layerMask))
            distance = hitInfo.distance;

        distance = Mathf.Max(distance, minDistance);

        pointLight.transform.position = origin + direction.normalized * distance;
    }

    void LateUpdate()
    {
        FollowPlayer();
    }

}
