using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentSpawnerBoid : MonoBehaviour
{
    public GameObject agentPrefab;

    //public int agentsArrived;

    //public List<AgentControllerBoid> agents;


    public LayerMask terrainLayer; 
    public Transform targetPoint;

    public int agentCount;


    void Start()
    {

        List<AgentControllerBoid> agents = new List<AgentControllerBoid>();
        
        for (int i = 0; i < agentCount; i++)
        {
            agents.Add(SpawnAgent());
        }

        MovementManager mm0 = GetSpawningMovementManager(agents, new Vector3(-0.5f,0,0));
        MovementManager mm1 = GetSpawningMovementManager(agents, new Vector3(3,0,0));

        //print("movement mannagers: " + mm0 + " " + mm1);

        // Check these two? are they handled the right way?

        int t = 0;
        foreach (AgentControllerBoid pf in agents){
            int team = t % 2;
            t ++;

            pf.SetAgent(team);

            if (team == 0) {
                pf.SetMovementManager(mm0);
                pf.transform.position += new Vector3(-0.5f, 0, 0);
            } else {
                pf.SetMovementManager(mm1);
                pf.transform.position += new Vector3(3, 0, 0);
            }

            
        
        }
    }

    MovementManager GetSpawningMovementManager(List<AgentControllerBoid> agents, Vector3 targetOffset){
        FlowFieldManager flowFieldManager = new FlowFieldManager(40, 40, 0.5f, terrainLayer);
        flowFieldManager.CreateGridFromMousePos(targetPoint.position+ targetOffset);
        MovementManager movementManager = new BasicMovementManager(flowFieldManager, agents);

        return movementManager;
    }

    AgentControllerBoid SpawnAgent()
    {
        GameObject pf = Instantiate(agentPrefab, transform.position, Quaternion.identity);
        pf.name = "Agent"+"_"+Random.Range(1000, 10000);
        return pf.GetComponent<AgentControllerBoid>();
        
    }

}
