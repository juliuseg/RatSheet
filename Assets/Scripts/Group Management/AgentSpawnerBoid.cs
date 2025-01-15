using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentSpawnerBoid : MonoBehaviour
{
    public GameObject agentPrefab;


    public LayerMask terrainLayer; 

    public int agentCount;

    public int team;


    private PathFindingController pfCont;

    void Start()
    {   
        pfCont = GameObject.Find("PathFindingController").GetComponent<PathFindingController>();

        List<AgentControllerBoid> agents = new List<AgentControllerBoid>();
        
        for (int i = 0; i < agentCount; i++)
        {
            agents.Add(SpawnAgent());
        }

        MovementManager mm = GetSpawningMovementManager(agents);

        pfCont.AddMM(mm);

        foreach (AgentControllerBoid pf in agents){
            
            pf.SetSelectable(team);

            pf.SetMovementManager(mm);

            pf.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
            
        
        }
        
    }

    MovementManager GetSpawningMovementManager(List<AgentControllerBoid> agents){
        FlowFieldManager flowFieldManager = pfCont.GetFlowFieldManager();
        flowFieldManager.CreateGridFromMousePos(transform.position);
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
