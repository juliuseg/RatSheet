using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class MovementManager
{
    public FlowFieldManager flowFieldManager; 
    public abstract MovementManagerType ManagerType { get; }

    public List<AgentControllerBoid> agents;

    private int id;

    public event System.Action OnAllAgentsRemoved;


    public MovementManager(FlowFieldManager _flowFieldManager, List<AgentControllerBoid> _agents, int _id = 0)
    {
        flowFieldManager = _flowFieldManager;
        agents = _agents;
        id = _id==0?Random.Range(0, 2000000):_id;
        //Debug.Log("MovementManager created with id: " + id);

    }

    public int GetAgentArrived(){
        return agents.Where(agent => agent.arrivedHandler.GetArrived()).Count(); //arrived).Count();
    }

    public int GetAgentCount(){
        return agents.Count;
    }

    public int GetID(){
        return id;
    }

    public void RemoveAgent(AgentControllerBoid agent){
        agents.Remove(agent);
        if (agents.Count == 0){
            OnAllAgentsRemoved?.Invoke();
        }
    }

}

public enum MovementManagerType
{
    Basic,
    Attack,
    DirrectAttack,
    Collect
}