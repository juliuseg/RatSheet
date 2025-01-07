using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovementManager
{
    public FlowFieldManager flowFieldManager; 


    public List<AgentControllerBoid> agents;

    private int id;


    public MovementManager(FlowFieldManager _flowFieldManager, List<AgentControllerBoid> _agents, int _id = 0)
    {
        flowFieldManager = _flowFieldManager;
        agents = _agents;
        id = _id==0?Random.Range(0, 2000000):_id;
        Debug.Log("MovementManager created with id: " + id);

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
    }

}