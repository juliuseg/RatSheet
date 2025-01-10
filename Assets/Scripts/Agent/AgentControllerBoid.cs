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

    private AgentAppearance agentAppearance;
    private AgentVelocity agentVelocity;

    [SerializeField] private GameObject selectionCircle;

    public int team { get; private set; }


    public AgentHPController health;

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


        
        // Attack
        AttackState attackState = AttackState.idle;

        Vector2 enemyVelocity = Vector2.zero;
        AgentControllerBoid other = null;

        if (rb.isKinematic) rb.isKinematic = false;
        if (movementManager.GetType() == typeof(AttackMovementManager)
        || (movementManager.GetType() == typeof(BasicMovementManager) && arrivedHandler.GetInitialArrived()))
        {
            other = AgentUtils.GetClosestNeigborOnOtherTeam(neighbors, transform.position, team);
            if (other != null){
                print ("other: " + other);
                bool canSee = AgentUtils.CanSeeOther(this, other);
                if (canSee){
                    enemyVelocity = agentVelocity.GetVelocityFromEnemy(other);
                    print ("enemyVelocity: " + enemyVelocity);
                }
            }
        }

        if (enemyVelocity != Vector2.zero){
            if (other != null){
                if  (!agentVelocity.IsInAttackRange(other)){
                    agentVelocity.SetVelocity(enemyVelocity);
                    attackState = AttackState.movingToAttack;
                } else {
                    agentVelocity.SetVelocity(Vector2.zero);
                    attackState = AttackState.attacking;
                    rb.isKinematic = true;
                    other.health.TakeDamage(agentStats.dps*Time.deltaTime);

                }
            }
        } else {
            // Movement
            bool checkN = arrivedHandler.UpdateArrivalStatus();

            Vector2 flowFieldVelocity = agentVelocity.GetVelocityFromFlowField(arrivedHandler.GetArrived(), arrivedHandler.GetArrivedCorrection());
            
            if (checkN) arrivedHandler.CheckNeighborsArrival();
            
            if (flowFieldVelocity != Vector2.zero){
                agentVelocity.SetVelocity(flowFieldVelocity);
                attackState = AttackState.moving;
            } else {
                agentVelocity.SetVelocity(Vector2.zero);
                attackState = AttackState.idle;
            }
        }


        // Appearance
        agentAppearance.AdjustAgentAppearance(arrivedHandler.GetArrived(), arrivedHandler.GetArrivedCorrection(), attackState, movementManager, AgentUtils.GetNeighborsInGroup(neighbors, movementManager).Count);


    }

    public void SetSelectionCircleActive(int mode) // mode 0 = off, mode 1 = highlighted in red, mode 2 = selected in green
    {
        agentAppearance.SetSelectionCircleActive(mode);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Add neighbor if it's another AgentController and not this agent
        if (other.TryGetComponent(out AgentControllerBoid neighbor) && neighbor != this && !neighbors.Contains(neighbor))
        {
            print ("neighbor added: " + other.gameObject.name + " to " + gameObject.name);
            neighbors.Add(neighbor);
            neighbor.health.OnDeath += () => neighbors.Remove(neighbor);
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Remove the agent from neighbors when exiting the detection radius
        if (other.TryGetComponent(out AgentControllerBoid neighbor)  && other.isTrigger)
        {
            neighbors.Remove(neighbor);
            neighbor.health.OnDeath -= () => neighbors.Remove(neighbor);
            //print ("neighbor removed: " + other.gameObject.GetInstanceID() + " from " + gameObject.GetInstanceID());
        }
    }
}
