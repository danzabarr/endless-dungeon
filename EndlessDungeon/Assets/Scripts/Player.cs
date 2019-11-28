using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Player : Unit
{
    private static Player instance;
    public static Player Instance => instance;

    private Camera cam;
    private EventSystem eventSystem;
    public EquipmentManager Equipment { get; private set; }

    public Collider Collider { get; private set; }

    [SerializeField]
    private AbilityBarInterfaceManager abilityBar;

    [SerializeField]
    private BuffBar buffBar, debuffBar;

    [SerializeField]
    private ParticleSystem destinationIndicator;

    private float slowestWalkSpeed = .25f;

    private int cLvl = 1;

    public int CharacterLevel => cLvl;

    private float WalkSpeed => Stats.WalkSpeed;

    [SerializeField]
    private float rotateSpeed;

    private float targetRotation;
    private float currentRotation;
    private bool moving;
    private Vector3 mouseOnPlane;
    private Vector3 mouseDirection;
    private RaycastHit hitInfo;
    private Interactive hover;
    private Interactive queuedTarget;
    private Vector3 castDirection;
    private Vector3 castTarget;
    private Vector3 throwTarget;
    private Vector3 floorTarget;
    private Vector3 queuedCastTarget;
    private Vector3 queuedThrowTarget;
    private Vector3 queuedFloorTarget;
    private Vector3 floorTargetNavMeshRaycast;
    private Vector3 floorTargetNavMeshSample;
    private bool hasFloorTargetNavMeshSample;
    private Vector3 lastPosition;
    private Vector3 walkDestination;
    private bool hasWalkDestination;
    public int currentAbilityIndex = -1;
    public int queuedAbilityIndex = -1;
    private bool queuedFloorTargetValid;
    private bool lClickDown;
    
    public Buff testBuffPrefab;

    public BagDisplay bagSlots;


    [ContextMenu("Apply Buff")]
    public void TestApplyBuff()
    {
        ApplyBuff(this, testBuffPrefab, out _, out _);
    }

    // Start is called before the first frame update
    public override void Awake()
    {
        instance = this;
        base.Awake();
        cam = Camera.main;
        eventSystem = EventSystem.current;
       
        Equipment = GetComponent<EquipmentManager>();
        Collider = GetComponent<Collider>();

        //TODO: Load this from save data
        bagSlots.Inventory = new Inventory(12, 8);
        //Agent.updatePosition = false;

    }

    private void StopMoving()
    {
        moving = false;
        walkDestination = transform.position;
        hasWalkDestination = false;
    }

    private void SetHover(Interactive hover)
    {
        if (this.hover == hover)
            return;

        if (this.hover != null)
        {
            this.hover.ShowOutline(false);
            if (!(this.hover is Mob))
                this.hover.ShowObjectLabel(false);
        }

        this.hover = hover;

        if (hover != null)
        {
            hover.ShowOutline(true);
            if (!(this.hover is Mob))
                hover.ShowObjectLabel(true);
        }
    }

    public float SquareDistance(Vector3 a, Vector3 b)
    {
        return (b.x - a.x) * (b.x - a.x)
             + (b.y - a.y) * (b.y - a.y)
             + (b.z - a.z) * (b.z - a.z);
    }

    

    protected override void Update()
    {
        base.Update();
        //Vector3 position = transform.position;
        //Shader.SetGlobalColor("_PlayerPosition", new Color(position.x, position.y, position.z));

        Vector3 origin = GetCastPosition();
        mouseOnPlane = ScreenPointToRayPlaneIntersection(Input.mousePosition, origin.y, Camera.main);
        mouseDirection = (mouseOnPlane - origin).normalized;

        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        float maxDistance = 150;

        castTarget = mouseOnPlane;
        castDirection = mouseDirection;
        throwTarget = ScreenPointToRayPlaneIntersection(Input.mousePosition, 0, Camera.main);

        if (Physics.Raycast(mouseRay, out hitInfo, maxDistance, LayerMask.GetMask("Cast")))
        {
            castTarget = hitInfo.point;
            castDirection = castTarget - origin;
            castDirection.Normalize();
        }

        if (Physics.Raycast(mouseRay, out hitInfo, maxDistance, LayerMask.GetMask("Default", "Walls", "Interactive"), QueryTriggerInteraction.Ignore))
        {
            castTarget = hitInfo.point;
            castDirection = castTarget - origin;
            castDirection.Normalize();
        }

        if (Physics.Raycast(mouseRay, out hitInfo, maxDistance, LayerMask.GetMask("Default", "Walls", "Interactive", "Floor"), QueryTriggerInteraction.Ignore))
        {
            throwTarget = hitInfo.point;
        }

        NavMeshHit navMeshHit;
        hasFloorTargetNavMeshSample = false;
        if (Physics.Raycast(mouseRay, out hitInfo, maxDistance, LayerMask.GetMask("Floor")))
        {
            floorTarget = hitInfo.point;
            floorTargetNavMeshRaycast = hitInfo.point;
            floorTargetNavMeshSample = hitInfo.point;
            

            if (NavMesh.Raycast(transform.position, hitInfo.point, out navMeshHit, NavMesh.AllAreas))
            {
                floorTargetNavMeshRaycast = navMeshHit.position;
            }

            if (NavMesh.SamplePosition(hitInfo.point, out navMeshHit, 1.0f, NavMesh.AllAreas))
            {
                floorTargetNavMeshSample = navMeshHit.position;
                if (!Physics.Raycast(new Ray(transform.position, floorTargetNavMeshSample - transform.position), (floorTargetNavMeshSample - transform.position).magnitude, LayerMask.GetMask("Walls", "Doors")))
                {
                    hasFloorTargetNavMeshSample = true;
                }
            }
        }
        else
        {
            Vector3 planePoint = ScreenPointToRayPlaneIntersection(Input.mousePosition, transform.position.y, Camera.main);

            floorTarget = planePoint;
            floorTargetNavMeshRaycast = planePoint;
            floorTargetNavMeshSample = planePoint;

            if (NavMesh.Raycast(transform.position, planePoint, out navMeshHit, NavMesh.AllAreas))
            {
                floorTargetNavMeshRaycast = navMeshHit.position;
            }
            if (NavMesh.SamplePosition(planePoint, out navMeshHit, 1.0f, NavMesh.AllAreas))
            {
                floorTargetNavMeshSample = navMeshHit.position;
                if (!Physics.Raycast(new Ray(transform.position, floorTargetNavMeshSample - transform.position), (floorTargetNavMeshSample - transform.position).magnitude, LayerMask.GetMask("Walls", "Doors")))
                {
                    hasFloorTargetNavMeshSample = true;
                }
            }
        }

        if (!Input.GetMouseButton(0))
        {
            if (!eventSystem.IsPointerOverGameObject() && Physics.SphereCast(mouseRay, .5f, out hitInfo, maxDistance, LayerMask.GetMask("Interactive", "Mobs", "Walls")))
            {
                SetHover(hitInfo.collider.GetComponent<Interactive>());
                if (hover == null)
                {
                    InteractableChild child = hitInfo.collider.GetComponent<InteractableChild>();
                    if (child) hover = child.parent;
                }

                if (hover != null)
                {
                    castTarget = hover.GetCenterPosition();
                    castDirection = castTarget - origin;
                    castDirection.Normalize();
                    throwTarget = castTarget;
                    floorTargetNavMeshSample = hover.GetGroundPosition();
                }
            }
            else SetHover(null);
        }

        float epsilon = 0.1f;
        float rotationDelta = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, targetRotation);
        float rotationDeltaAbs = Mathf.Abs(rotationDelta);
        float directionSign = Mathf.Sign(rotationDelta);

        currentRotation = transform.rotation.eulerAngles.y;

        if (rotationDeltaAbs > epsilon)
            currentRotation += Mathf.Min(Time.deltaTime * rotateSpeed * WalkSpeed, rotationDeltaAbs) * directionSign;
        
        transform.rotation = Quaternion.Euler(0, currentRotation, 0);

        Vector3 movementDirection = Vector3.zero;
        moving = false;

        if (Input.GetMouseButtonDown(0) && InventoryManager.Instance.HeldItem == null && !eventSystem.IsPointerOverGameObject() && !Abilities.Channelling)
        {
            queuedTarget = hover;
            queuedAbilityIndex = 4;
            queuedCastTarget = castTarget;
            queuedFloorTarget = floorTargetNavMeshSample;
            queuedFloorTargetValid = hasFloorTargetNavMeshSample;
            queuedThrowTarget = throwTarget;
            hasWalkDestination = true;
            lClickDown = true;
            if (queuedTarget == null)
                Instantiate(destinationIndicator, queuedFloorTarget, Quaternion.identity);
        }

        if (!Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            lClickDown = false;
        }

        if (Input.GetMouseButton(0) && lClickDown && queuedTarget == null && !Abilities.Channelling)// && !ExtendedStandaloneInputModule.GetPointerEventData().pointerCurrentRaycast.isValid
        {
            queuedAbilityIndex = 4;
            queuedCastTarget = castTarget;
            queuedFloorTarget = floorTargetNavMeshSample;
            queuedFloorTargetValid = hasFloorTargetNavMeshSample;
            queuedThrowTarget = throwTarget;
            hasWalkDestination = true;
        }

        if (Abilities.Channelling)
        {
            queuedTarget = hover;
            queuedCastTarget = castTarget;
            queuedFloorTarget = floorTargetNavMeshSample;
            queuedFloorTargetValid = hasFloorTargetNavMeshSample;
            queuedThrowTarget = throwTarget;

            Vector3 t = queuedCastTarget;
            Vector3 d = GetCastPosition();
            if (Abilities.Type(queuedAbilityIndex) == Ability.AbilityType.Thrown)
            {
                t = queuedThrowTarget;
                d = GetCastPosition();
            }
            else if (Abilities.Type(queuedAbilityIndex) == Ability.AbilityType.Place)
            {
                t = queuedFloorTarget;
                d = transform.position;
            }

            LookInDirection(t - d);

            Abilities.Channel(queuedTarget is Unit ? queuedTarget as Unit : null, queuedCastTarget, queuedThrowTarget, queuedFloorTarget);
        }

        for (int i = 0; i < Abilities.Length; i++)
        {
            if (!Input.GetKey(KeyCode.LeftShift) && i == 4)
                continue;

            KeyCode binding = abilityBar.GetBinding(i);

            if (binding == KeyCode.None)
                continue;

            if (Abilities[i] == null)
                continue;

            if (Abilities.OnCooldown(i))
                continue;

            if (Abilities[i].Channelled && Abilities.Channelling && Abilities.Active == Abilities[i])
                continue;

            if (Input.GetKeyDown(binding))
            {
                queuedAbilityIndex = i;
                queuedTarget = hover;
                if (!(queuedTarget is Unit))
                    queuedTarget = null;
                queuedCastTarget = castTarget;
                queuedFloorTarget = floorTargetNavMeshSample;
                queuedFloorTargetValid = hasFloorTargetNavMeshSample;
                queuedThrowTarget = throwTarget;
            }
        }
        
        if (queuedTarget != null && queuedTarget is Unit && (queuedTarget as Unit).GetCurrentHealth() <= 0)
        {
            queuedTarget = null;
        }
        
        if (hover != null && hover is Unit && (hover as Unit).GetCurrentHealth() <= 0)
        {
            hover = null;
        }

        if (queuedTarget == null)
        {

            if (queuedAbilityIndex == -1 || queuedAbilityIndex == 4 || Abilities[queuedAbilityIndex] == null)
            {
                walkDestination = queuedFloorTarget;

                if (hasWalkDestination && SquareDistance(GetGroundPosition(), walkDestination) > 1)
                {
                    movementDirection = (walkDestination - GetGroundPosition()).normalized;
                    moving = true;
                }
               
            }
            else
            {
                Vector3 t = queuedCastTarget;
                Vector3 d = GetCastPosition();
                if (Abilities.Type(queuedAbilityIndex) == Ability.AbilityType.Thrown)
                {
                    t = queuedThrowTarget;
                    d = GetCastPosition();
                }
                else if (Abilities.Type(queuedAbilityIndex) == Ability.AbilityType.Place)
                {
                    t = queuedFloorTarget;
                    d = transform.position;
                }

                float range = Abilities.Range(queuedAbilityIndex);


                if (SquareDistance(d, t) < range * range)
                {
                    LookInDirection(t - d);
                    if (Abilities.CastTimeRemaining <= 0.1f)
                    {
                        Abilities.Cast(queuedAbilityIndex, null, queuedCastTarget, queuedThrowTarget, queuedFloorTarget, queuedFloorTargetValid);
                        currentAbilityIndex = queuedAbilityIndex;
                    }

                    StopMoving();
                   
                    if (!Input.GetKey(abilityBar.GetBinding(queuedAbilityIndex)))
                    {
                        queuedTarget = null;
                        queuedAbilityIndex = -1;
                    }
                    else if (queuedTarget == null)
                    {
                        queuedCastTarget = castTarget;
                        queuedThrowTarget = throwTarget;
                        queuedFloorTarget = floorTarget;
                        queuedFloorTargetValid = hasFloorTargetNavMeshSample;
                    }
                }
                else
                {
                    walkDestination = queuedFloorTarget;
                    hasWalkDestination = true;
                    movementDirection = (queuedFloorTarget - GetGroundPosition()).normalized;
                    moving = true;
                }
            }
        }
        else if (queuedTarget is Unit)
        {
            float lClickRange = Abilities.Range(queuedAbilityIndex);
            if (SquareDistance(GetCastPosition(), queuedTarget.GetCenterPosition()) < lClickRange * lClickRange)
            {
                LookInDirection(queuedTarget.GetCenterPosition() - GetCastPosition());
                if (Abilities.CastTimeRemaining <= 0.1f)
                {
                    Abilities.Cast(queuedAbilityIndex, queuedTarget as Unit, queuedCastTarget, queuedThrowTarget, queuedFloorTarget, queuedFloorTargetValid);
                    currentAbilityIndex = queuedAbilityIndex;
                }
                StopMoving();

                if (!Input.GetKey(abilityBar.GetBinding(queuedAbilityIndex)))
                {
                    queuedTarget = null;
                    queuedAbilityIndex = -1;
                }
                else if (queuedTarget == null)
                {
                    queuedCastTarget = castTarget;
                    queuedThrowTarget = throwTarget;
                    queuedFloorTarget = floorTargetNavMeshSample;
                    queuedFloorTargetValid = hasFloorTargetNavMeshSample;
                }

            }
            else
            {
                walkDestination = queuedTarget.GetGroundPosition();
                hasWalkDestination = true;
                movementDirection = (queuedTarget.GetGroundPosition() - GetGroundPosition()).normalized;
                moving = true;
            }
        }
        else if (queuedAbilityIndex == -1 || queuedAbilityIndex == 4)
        {
            if (SquareDistance(GetCastPosition(), queuedTarget.GetGroundPosition()) < queuedTarget.GetInteractDistance() * queuedTarget.GetInteractDistance())
            {
                LookInDirection(queuedTarget.GetGroundPosition() - GetGroundPosition());
                queuedTarget.Interact();
                SetHover(null);
                if (queuedTarget is ItemObject)
                {
                    ItemObject targetItem = queuedTarget as ItemObject;

                    if (bagSlots.Inventory.Add(targetItem))
                    {
                        bagSlots.Refresh(targetItem);   
                        Level.Instance.RemoveItem(targetItem);
                        targetItem.transform.SetParent(transform);
                    }
                }
                StopMoving();
                queuedTarget = null;
                queuedAbilityIndex = -1;
            }
            else
            {
                walkDestination = queuedTarget.GetGroundPosition();
                hasWalkDestination = true;
                movementDirection = (queuedTarget.GetGroundPosition() - GetGroundPosition()).normalized;
                moving = true;
            }
        }

        if (!Input.GetKey(abilityBar.GetBinding(queuedAbilityIndex)) && Abilities.Channelling)
        {
            Abilities.EndChannelling();
            queuedTarget = null;
            queuedAbilityIndex = -1;
            StopMoving();
        }

        if (Abilities.Casting)
        {
            StopMoving();
        }

        Animator.SetBool("moving", moving);
        Animator.SetFloat("walkspeed", WalkSpeed);

        if (moving)
        {
            movementDirection.Normalize();

            targetRotation = 90f - Mathf.Rad2Deg * Mathf.Atan2(movementDirection.z, movementDirection.x);// Quaternion.LookRotation(movementDirection, Vector3.up);

            Vector3 move = movementDirection * WalkSpeed;


            Agent.Move(move * Time.deltaTime);
            //rb.AddForce(move, ForceMode.VelocityChange);
        }

        Vector3 walkDelta = (GetGroundPosition() - lastPosition) / Time.deltaTime;
        lastPosition = GetGroundPosition();

        if (hasWalkDestination)
        {
            float minWalkDistance = slowestWalkSpeed * WalkSpeed;

            if (walkDelta.sqrMagnitude < minWalkDistance * minWalkDistance)
            {
                StopMoving();
                queuedTarget = null;
                queuedAbilityIndex = -1;
            }
        }
    }

    public override void Teleport(Vector3 position)
    {
        Agent.enabled = false;
        transform.position = position;
        Agent.enabled = true;
        walkDestination = position;
    }

    public override void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
        float y = rotation.eulerAngles.y;
        targetRotation = y;
        currentRotation = y;
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
        Vector3 origin = GetCastPosition();
        //Gizmos.color = Color.grey;
       // Gizmos2.DrawWireCircle(origin, radius, angleStart, angleDegrees, resolution);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, castTarget);
        Gizmos.DrawSphere(castTarget, 0.1f);
        //Gizmos.DrawLine(castTarget, new Vector3(castTarget.x, origin.y, castTarget.z));
        //Gizmos.DrawLine(new Vector3(castTarget.x, 0, castTarget.z), new Vector3(castTarget.x, origin.y, castTarget.z));
        //Gizmos.DrawSphere(new Vector3(castTarget.x, origin.y, castTarget.z), 0.1f);
        //Gizmos.DrawSphere(new Vector3(castTarget.x, 0, castTarget.z), 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(GetGroundPosition(), floorTargetNavMeshRaycast);
        Gizmos.DrawSphere(floorTargetNavMeshRaycast, .3f);

        if (hasFloorTargetNavMeshSample)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawSphere(floorTargetNavMeshSample, .2f);

        Gizmos.color = Color.magenta;
        //Gizmos.DrawLine(transform.position, floorTargetNavMeshSample);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(GetGroundPosition(), walkDestination);
        Gizmos.DrawSphere(walkDestination, .2f);
    }

    public override void LookInDirection(Vector3 direction)
    {
        targetRotation = 90f - Mathf.Rad2Deg * Mathf.Atan2(direction.z, direction.x);
    }

    /*
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision);
    }
    */

    public override void WalkTo(Vector3 position)
    {
        walkDestination = position;
    }

    public override void Interact(Interactive interactive)
    {
        queuedTarget = interactive;
    }

    public override bool ApplyBuff(Unit caster, Buff buff, out BuffInstance instance, out bool newInstance, int stacks = 1, int maxStacks = 0)
    {
        if (base.ApplyBuff(caster, buff, out instance, out newInstance, stacks, maxStacks))
        {
            if (newInstance)
                buffBar.AddBuff(instance, Color.clear);
            return true;
        }
        return false;
    }

    public override bool ApplyDebuff(Unit caster, Buff debuff, out BuffInstance instance, out bool newInstance, bool useResistances, int stacks = 1, int maxStacks = 0)
    {
        if (base.ApplyDebuff(caster, debuff, out instance, out newInstance, useResistances, stacks, maxStacks))
        {
            if (newInstance)
                debuffBar.AddBuff(instance, Color.red);
            return true;
        }
        return false;
    }

    public override void Hit(float duration)
    {
        
    }

    public override void OnDeath()
    {
        
    }
}
