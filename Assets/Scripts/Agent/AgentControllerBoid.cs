using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class AgentControllerBoid : MonoBehaviour
{
    public AgentStats agentStats;               // Reference to BoidStats ScriptableObject
    public MovementManager movementManager;     // Reference to the MovementManager
    private Rigidbody2D rb;
    private CircleCollider2D col;
    private List<AgentControllerBoid> neighbors = new List<AgentControllerBoid>(); // List of nearby agents
    public ArrivedHandler arrivedHandler;

    public AgentAppearance agentAppearance{ get; private set;}
    private AgentVelocity agentVelocity;

    [SerializeField] private GameObject selectionCircle;

    public int team { get; private set; }


    public AgentHPController health;

    private AgentAttackController attackController;
    private AgentMovementController movementController;

    public void SetAgent(int  _team)
    {
        team = _team;

        rb = GetComponent<Rigidbody2D>();
        arrivedHandler = gameObject.AddComponent<ArrivedHandler>();

        agentAppearance = new AgentAppearance(selectionCircle, GetComponent<SpriteRenderer>(), team);
        SetupNeighborDetection();
        agentVelocity = new AgentVelocity(rb, transform, agentStats, neighbors);
        
        health = GetComponent<AgentHPController>();
        health.SetHealthInit(agentStats);
        health.OnDeath += AgentDead;

        attackController = gameObject.AddComponent<AgentAttackController>();
        attackController.Setup(transform, agentVelocity, agentStats);
        movementController = new AgentMovementController(this, agentVelocity);

        neighbors = gameObject.AddComponent<AgentNeighborCollisionHandler>().GetNeighbors();

    }

    private void SetupNeighborDetection()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = agentStats.neighborRadius;

    }
    

    public void SetMovementManager(MovementManager _movementManager)
    {
        if (movementManager != null)
        {
            movementManager.RemoveAgent(this);
        }
        movementManager = _movementManager;
        

        arrivedHandler.Setup(movementManager, rb, neighbors);


        arrivedHandler.TriggerNewFlowfield();

        agentVelocity.SetMovementManager(movementManager);


    }

    public void AgentDead(){
        if (movementManager != null)
        {
            movementManager.RemoveAgent(this);
        }
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {

        if (movementManager.flowFieldManager == null || agentStats == null) return;

        if (rb.isKinematic) rb.isKinematic = false;

        // Handle Attack
        AttackState attackState = attackController.HandleAttack(neighbors, team);

        // Handle Movement if not actively attacking
        if (attackState == AttackState.idle || attackState == AttackState.moving)
        {
            attackState = movementController.HandleMovement(arrivedHandler, movementManager, ref attackState);
        }

        
        // Adjust Appearance
        print(attackState);
        HandleAppearance(attackState);
    }


    private void HandleAppearance(AttackState attackState)
    {
        agentAppearance.AdjustAgentAppearance(
            arrivedHandler.GetArrived(),
            arrivedHandler.GetArrivedCorrection(),
            attackState,
            movementManager,
            AgentUtils.GetNeighborsInGroup(neighbors, movementManager).Count
        );
    }



    
}


public enum AttackState {idle, moving, movingToAttack, reloading}; // Attack is instant, so no need for attacking state


