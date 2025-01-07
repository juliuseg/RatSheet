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

        MovementManager mm = GetSpawningMovementManager(agents);

        foreach (AgentControllerBoid pf in agents){
            pf.GetComponent<AgentControllerBoid>().SetMovementManager(mm);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Spawn on space key press
        if (Input.GetKeyDown(KeyCode.Space))
        {

            SpawnAgent();
            
        }

        
    }

    MovementManager GetSpawningMovementManager(List<AgentControllerBoid> agents){
        FlowFieldManager flowFieldManager = new FlowFieldManager(40, 40, 0.5f, terrainLayer, targetPoint.position);
        flowFieldManager.CreateGridFromMousePos(targetPoint.position);
        MovementManager movementManager = new MovementManager(flowFieldManager, agents);

        return movementManager;
    }

    AgentControllerBoid SpawnAgent()
    {
        GameObject pf = Instantiate(agentPrefab, transform.position, Quaternion.identity);
        return pf.GetComponent<AgentControllerBoid>();
        
    }

}
