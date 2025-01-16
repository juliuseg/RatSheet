using UnityEngine;

public class BasicMovementManager : MovementManager
{
    public override MovementManagerType ManagerType => MovementManagerType.Basic;
    
    public BasicMovementManager(FlowFieldManager _flowFieldManager, System.Collections.Generic.List<AgentMoveable> _agents, int _id = 0) : base(_flowFieldManager, _agents, _id)
    {

    }
}