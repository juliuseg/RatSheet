using UnityEngine;

public class DirrectAttackMovementManager : MovementManager
{
    private GameObject target;
    public override MovementManagerType ManagerType => MovementManagerType.DirrectAttack;
    public DirrectAttackMovementManager(FlowFieldManager _flowFieldManager, System.Collections.Generic.List<AgentControllerBoid> _agents, int _id = 0, GameObject _target = null) : base(_flowFieldManager, _agents, _id)
    {
        target = _target;
    }

    public GameObject GetTarget(){
        return target;
    }
}
