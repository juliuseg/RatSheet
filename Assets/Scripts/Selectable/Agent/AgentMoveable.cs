using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public abstract class AgentMoveable : Selectable
{
    [SerializeField] protected AgentStats agentStats;

    public override SelectableStats stats => agentStats;

    public MovementManager movementManager;     
    private CircleCollider2D col; 
    [HideInInspector] public ArrivedHandler arrivedHandler;
    public AgentAppearance agentAppearance{ get; private set;}
    protected AgentVelocity agentVelocity;
    private AgentAbilities agentAbilities = new AgentAbilities();
    public override Abilities abilities => agentAbilities;
    
    protected AgentAttackController attackController;
    private AgentMovementController movementController;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private AgentAnimation agentAnimation;

    protected Vector2 velocity;

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

        movementController = new AgentMovementController(this, agentVelocity);

        agentAbilities = new AgentAbilities();
        agentAbilities.SetAbilities(agentStats.abilities);

        agentAnimation = GetComponent<AgentAnimation>();

        velocity = Vector2.zero;
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
        if (_movementManager == null) {
            return;
        }

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

    protected virtual void FixedUpdate()
    {
        AttackState attackState = AttackState.idle;
        if (agentStats ==null) return;

        if (rb.isKinematic) rb.isKinematic = false;

        if (movementManager == null)
        {
            agentVelocity.SetVelocity(Vector2.zero);
        }

        FixedUpdateBeforeMovement(attackState);


        // Handle Movement if not actively attacking
        if (attackState == AttackState.idle && movementManager != null)
        {
            attackState = movementController.HandleMovement(arrivedHandler, movementManager, ref attackState, out velocity);
        }
        

        HandleAppearance(attackState);

        agentAnimation.SetState(attackState, velocity, spriteRenderer);
    }

    protected virtual void FixedUpdateBeforeMovement(AttackState attackState)
    {
        
    }


    private void HandleAppearance(AttackState attackState)
    {
        agentAppearance.AdjustAgentAppearance(
            arrivedHandler.GetArrived(),
            arrivedHandler.GetArrivedCorrection(),
            attackState,
            movementManager,
            movementManager != null?AgentUtils.GetNeighborsInGroup(AgentUtils.GetNeighborsAgents(neighbors), movementManager).Count:0
        );
    }



    
}


public enum AttackState {idle, moving, movingToAttack, reloading, attacking}; // Attack is instant, so no need for attacking state


