using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class MovementManager
{
    public FlowFieldManager flowFieldManager; 
    public abstract MovementManagerType ManagerType { get; }

    public List<AgentMoveable> agents;

    private int id;

    public event System.Action OnAllAgentsRemoved;

    public event System.Action InitialArrival;

    public bool oneAgentArrived;


    public MovementManager(FlowFieldManager _flowFieldManager, List<AgentMoveable> _agents, int _id = 0)
    {
        flowFieldManager = _flowFieldManager;
        agents = _agents;
        id = _id==0?Random.Range(0, 2000000):_id;

        oneAgentArrived = false;

        foreach (AgentMoveable agent in agents)
        {
            agent.arrivedHandler.InitialArrival += () => SetInitalArrivedAgent();
        }
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

    public void RemoveAgent(AgentMoveable agent){
        agents.Remove(agent);
        if (agents.Count == 0){
            OnAllAgentsRemoved?.Invoke();
        }
    }

    public void SetInitalArrivedAgent(){
        if (!oneAgentArrived){
            InitialArrival?.Invoke();
            oneAgentArrived = true;
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