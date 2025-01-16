using UnityEngine;


public class AgentMovementController
{
    private readonly AgentVelocity agentVelocity;

    public AgentMovementController(AgentMoveable agent, AgentVelocity agentVelocity)
    {
        this.agentVelocity = agentVelocity;
    }

    public AttackState HandleMovement(ArrivedHandler arrivedHandler, MovementManager movementManager, ref AttackState attackState, out Vector2 velocity)
    {
        velocity = Vector2.zero;
        bool hasArrived = arrivedHandler.UpdateArrivalStatus();
        if (hasArrived) arrivedHandler.CheckNeighborsArrival();

        Vector2 flowFieldVelocity = agentVelocity.GetVelocityFromFlowField(
            arrivedHandler.GetArrived(),
            arrivedHandler.GetArrivedCorrection()
        );

        if (flowFieldVelocity != Vector2.zero)
        {
            velocity = agentVelocity.SetVelocity(flowFieldVelocity);
            return AttackState.moving;
        }
        else
        {
            velocity = agentVelocity.SetVelocity(Vector2.zero);
            return AttackState.idle;
        }
    }
}
