using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent), typeof(DynamicObject))]
[DisallowMultipleComponent]
public class Mob : Unit, RegisterablePrefab, Interactive
{
    
    private NavMeshObstacle obstacle;
    public DynamicObject DynamicObject { get; private set; }

    private Outline[] outline;
    private ObjectLabel label;

    [SerializeField]
    private Vector3 labelWorldOffset;
    [SerializeField]
    private Vector2 labelScreenOffset;

    private int prefabID;
    public int GetPrefabID() => prefabID;
    public void SetPrefabID(int id) => prefabID = id;

    

    public override void Awake()
    {
        base.Awake();
        obstacle = GetComponent<NavMeshObstacle>();
        DynamicObject = GetComponent<DynamicObject>();
        outline = GetComponentsInChildren<Outline>();
    }

    public override void Start()
    {
        base.Start();
        ShowOutline(false);
        navigationTarget = Player.Instance.transform;
        UpdateDestination();
    }

    protected override void Update()
    {
        base.Update();
        Agent.speed = Stats.WalkSpeed;
        if (Agent.isOnNavMesh)
        {
            if (Stunned)
                Agent.ResetPath();
            else
                UpdateDestination();
        }
    }

    

   

    

    private Transform navigationTarget;

    public Transform NavigationTarget
    {
        get => navigationTarget;
        set
        {
            if (navigationTarget == value)
                return;
            navigationTarget = value;
            UpdateDestination();
        }
    }

    public void UpdateDestination()
    {
        if (!Agent.isOnNavMesh)
            return;

        if (navigationTarget == null)
        {
            Agent.ResetPath();
            return;
        }


        NavMeshPath path = Agent.path;

        bool foundPath = Agent.CalculatePath(navigationTarget.position, path);


        //      agent.SetDestination(new Vector3(target.position.x, transform.position.y, target.position.z));

        if (foundPath && path.status == NavMeshPathStatus.PathComplete)
        {
            Agent.SetPath(path);
        }
        else if (Agent.isOnNavMesh)
        {
            Agent.ResetPath();
        }
    }

    private ObjectLabel Label
    {
        get
        {
            if (label == null)
            {
                label = LabelManager.Add(this);
                label.ZSort = true;
                label.LabelTextFontSize = 12;
                label.worldTransform = transform;
                label.worldOffset = labelWorldOffset;
                label.screenOffset = labelScreenOffset;
            }
            return label;
        }
    }

    public void OnMobDeath(GameEventSystem.EventArgs e)
    {
        Debug.Log("Something died, I'm sure of it...");
    }
    

    public override void OnDeath()
    {
        base.OnDeath();
        
        LabelManager.Remove(this);
        Level.Instance.RemoveMob(this);
    }

    public override void Teleport(Vector3 position)
    {
        Agent.enabled = false;
        transform.position = position;
        Agent.enabled = true;
    }

    public MobData WriteData()
    {
        return new MobData
        {
            prefabID = prefabID,

            posX = transform.position.x,
            posY = transform.position.y,
            posZ = transform.position.z,

            rotX = transform.rotation.x,
            rotY = transform.rotation.y,
            rotZ = transform.rotation.z,
            rotW = transform.rotation.w,
        };
    }

    public override void OnTakeDamage(Unit caster, Ability.DamageType type, float amount)
    {
        Label.LabelText = IntegerHealth + "/" + IntegerMaxHealth;
        Label.BarFillAmount = FractionHealth;
        ShowObjectLabel(true);
    }

    public void ShowObjectLabel(bool show)
    {
        if (show)
            Label.gameObject.SetActive(show);
        else
            LabelManager.Remove(this);
    }

    public void ShowOutline(bool show)
    {
        foreach (Outline o in outline)
            o.hideOutline = !show;
    }

    public void ShowLabelText(bool show)
    {
        Label.LabelEnabled = show;
    }

    public void ShowBar(bool show)
    {
        Label.BarEnabled = show;
    }

    public Transform GetWorldTransform()
    {
        return transform;
    }

    public float GetInteractDistance()
    {
        return 1;
    }

    public void Interact()
    {
        
    }


}
