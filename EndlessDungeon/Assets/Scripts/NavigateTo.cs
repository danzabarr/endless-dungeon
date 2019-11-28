using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavigateTo : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private NavMeshAgent agent;

    public Transform Target
    {
        get => target;
        set
        {
            if (target == value)
                return;
            target = value;
            UpdateDestination();
        }
    }

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Start()
    {
        target = Player.Instance.transform;
        UpdateDestination();
    }

    public void Update()
    {
        UpdateDestination();
    }

    public void UpdateDestination()
    {
        if (target == null)
        {
            agent.ResetPath();
            return;
        }

        NavMeshPath path = agent.path;

        bool foundPath = agent.CalculatePath(target.position, path);


        //      agent.SetDestination(new Vector3(target.position.x, transform.position.y, target.position.z));

        if (foundPath && path.status == NavMeshPathStatus.PathComplete)
            agent.SetPath(path);
        else agent.ResetPath();
    }
}
