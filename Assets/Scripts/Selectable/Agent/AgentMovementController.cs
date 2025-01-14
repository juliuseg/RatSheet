using UnityEngine;


public class AgentMovementController
{
    private readonly AgentVelocity agentVelocity;

    public AgentMovementController(AgentControllerBoid agent, AgentVelocity agentVelocity)
    {
        this.agentVelocity = agentVelocity;
    }

    public AttackState HandleMovement(ArrivedHandler arrivedHandler, MovementManager movementManager, ref AttackState attackState)
    {
        bool hasArrived = arrivedHandler.UpdateArrivalStatus();
        if (hasArrived) arrivedHandler.CheckNeighborsArrival();

        Vector2 flowFieldVelocity = agentVelocity.GetVelocityFromFlowField(
            arrivedHandler.GetArrived(),
            arrivedHandler.GetArrivedCorrection()
        );

        if (flowFieldVelocity != Vector2.zero)
        {
            agentVelocity.SetVelocity(flowFieldVelocity);
            return AttackState.moving;
        }
        else
        {
            agentVelocity.SetVelocity(Vector2.zero);
            return AttackState.idle;
        }
    }
}
