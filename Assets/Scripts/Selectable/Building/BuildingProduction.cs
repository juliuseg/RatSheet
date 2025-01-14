using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingProduction : MonoBehaviour {
    private List<SpawnObject> productionQueue = new List<SpawnObject>();
    private float productionTime = 0;

    private BuildingStats buildingStats;

    private int team;

    public LayerMask terrainLayer; 

    public void SetupBuildingProduction(BuildingStats _buildingStats, int _team){
        buildingStats = _buildingStats;
        team = _team;
    }

    public void AddToProduction(int i){
        productionQueue.Add(buildingStats.spawnObjects[i]);
    }
    public void UpdateQueue(){
        if (productionQueue.Count > 0){
            productionTime += Time.deltaTime;

            if (productionTime >= productionQueue[0].spawnTime){//productionQueue[0].spawnTime){
                Produce(productionQueue[0].objectToSpawn);
                productionQueue.RemoveAt(0);
                productionTime = 0;
            }
        }
    }
    private void Produce(GameObject objectToSpawn){
        float angle = Random.Range(0, Mathf.PI*2);
        Vector3 spawnOffset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * buildingStats.SpawnRadius;

        Vector3 spawnPos = transform.position + spawnOffset;
        AgentControllerBoid agent = SpawnAgent(spawnPos, objectToSpawn);

        MovementManager mm = GetSpawningMovementManager(agent,spawnPos);

        agent.SetSelectable(team);

        agent.SetMovementManager(mm);

        agent.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
            
        
        
    }

    public float GetFinishPercentage(){
        if (productionQueue.Count == 0){
            return -1f;
        }
        return productionTime / productionQueue[0].spawnTime;
    }


    MovementManager GetSpawningMovementManager(AgentControllerBoid agent, Vector3 spawnPos){
        FlowFieldManager flowFieldManager = new FlowFieldManager(64, 64, 0.5f, terrainLayer);
        flowFieldManager.CreateGridFromMousePos(spawnPos);
        MovementManager movementManager = new BasicMovementManager(flowFieldManager, new List<AgentControllerBoid>{agent});

        return movementManager;
    }

    AgentControllerBoid SpawnAgent(Vector3 spawnPos, GameObject objectToSpawn)
    {
        GameObject pf = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
        pf.name = "Agent"+"_"+Random.Range(1000, 10000);
        return pf.GetComponent<AgentControllerBoid>();
        
    }


}