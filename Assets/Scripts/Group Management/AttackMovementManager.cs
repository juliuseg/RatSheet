using UnityEngine;

public class AttackMovementManager : MovementManager
{
    private GameObject target;
    
    public AttackMovementManager(FlowFieldManager _flowFieldManager, System.Collections.Generic.List<AgentControllerBoid> _agents, int _id = 0, GameObject _target = null) : base(_flowFieldManager, _agents, _id)
    {
        target = _target;
    }

    public GameObject GetTarget(){
        return target;
    }
}