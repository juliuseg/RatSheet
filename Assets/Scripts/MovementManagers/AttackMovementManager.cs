using UnityEngine;

public class AttackMovementManager : MovementManager {
    public override MovementManagerType ManagerType => MovementManagerType.Attack;
    public AttackMovementManager(FlowFieldManager _flowFieldManager, System.Collections.Generic.List<AgentMoveable> _agents, int _id = 0) : base(_flowFieldManager, _agents, _id)
    {

    }
}