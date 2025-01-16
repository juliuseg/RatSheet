using UnityEngine;

public class SpearMan : AgentMoveable
{
    public override void SetSelectable(int  _team)
    {
        base.SetSelectable(_team);
        attackController = gameObject.AddComponent<AgentAttackController>();
        attackController.Setup(transform, agentVelocity, agentStats, rb);
    }

    protected override void FixedUpdateBeforeMovement(AttackState attackState)
    {
        // Handle Attack only if attackmovement manager. 
        if ((movementManager != null && movementManager.GetType() == typeof(AttackMovementManager)) 
        || arrivedHandler.GetInitialArrived()){
            attackState = attackController.HandleAttack(neighbors, team, out velocity);
        }
    }
    
}