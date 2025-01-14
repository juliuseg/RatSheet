using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class AgentControllerBoid : Selectable
{
    [SerializeField] private AgentStats agentStats;

    public override SelectableStats stats => agentStats;

    public MovementManager movementManager;     
    private CircleCollider2D col; 
    [HideInInspector] public ArrivedHandler arrivedHandler;
    public AgentAppearance agentAppearance{ get; private set;}
    private AgentVelocity agentVelocity;


    private AgentAbilities agentAbilities = new AgentAbilities();
    public override Abilities abilities => agentAbilities;
    
    private AgentAttackController attackController;
    private AgentMovementController movementController;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private AgentAnimation agentAnimation;

    public override Abilities GetAbilities(){
        return agentAbilities;
    }

    public override void SetSelectable(int  _team)
    {
        base.SetSelectable(_team);

        arrivedHandler = gameObject.AddComponent<ArrivedHandler>();

        agentAppearance = new AgentAppearance(selectionCircle, spriteRenderer, team);
        SetupNeighborDetection();
        agentVelocity = new AgentVelocity(rb, transform, agentStats, AgentUtils.GetNeighborsAgents(neighbors));
        

        attackController = gameObject.AddComponent<AgentAttackController>();
        attackController.Setup(transform, agentVelocity, agentStats, rb);
        movementController = new AgentMovementController(this, agentVelocity);

        agentAbilities = new AgentAbilities();
        agentAbilities.SetAbilities(agentStats.abilities);

        agentAnimation = GetComponent<AgentAnimation>();

    }

    public override void SetSelectionCircleActive(int active)
    {
        agentAppearance.SetSelectionCircleActive(active);
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
        

        arrivedHandler.Setup(movementManager, rb, AgentUtils.GetNeighborsAgents(neighbors));


        arrivedHandler.TriggerNewFlowfield();

        agentVelocity.SetMovementManager(movementManager);

    }

    public override void SelectableDead(){
        if (movementManager != null)
        {
            movementManager.RemoveAgent(this);
        }
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        //print("FixedUpdate");
        if (movementManager != null && movementManager.flowFieldManager == null || agentStats == null) {
            print("MovementManager is " + movementManager != null + "flowFieldManager is " + (movementManager.flowFieldManager==null) + " and agentStats is " + (agentStats==null));
            return;
        }

        if (rb.isKinematic) rb.isKinematic = false;

        AttackState attackState = AttackState.idle;
        Vector2 velocity = Vector2.zero;

        // Handle Attack only if attackmovement manager. 
        if (movementManager.GetType() == typeof(AttackMovementManager) || arrivedHandler.GetInitialArrived()){
            attackState = attackController.HandleAttack(neighbors, team, out velocity);
        }
        // Handle Movement if not actively attacking
        if (attackState == AttackState.idle)
        {
            attackState = movementController.HandleMovement(arrivedHandler, movementManager, ref attackState, out velocity);
        }


        
        // Adjust Appearance
        //print("Attack state: " + attackState);
        HandleAppearance(attackState);

        agentAnimation.SetState(attackState, velocity, spriteRenderer);
    }


    private void HandleAppearance(AttackState attackState)
    {
        agentAppearance.AdjustAgentAppearance(
            arrivedHandler.GetArrived(),
            arrivedHandler.GetArrivedCorrection(),
            attackState,
            movementManager,
            AgentUtils.GetNeighborsInGroup(AgentUtils.GetNeighborsAgents(neighbors), movementManager).Count
        );
    }



    
}


public enum AttackState {idle, moving, movingToAttack, reloading, attacking}; // Attack is instant, so no need for attacking state


