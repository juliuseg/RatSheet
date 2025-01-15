using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingController : MonoBehaviour
{
    private List<MovementManager> movementManagers;

    public int cols = 64;
    public int rows = 64;
    public float cellSize = 0.5f;
    public LayerMask terrainLayer;


    private void Start() {
        movementManagers = new List<MovementManager>();
    }

    public void AddMM(MovementManager mm){
        mm.OnAllAgentsRemoved += () => movementManagers.Remove(mm);
        movementManagers.Add(mm);
        
    }

    private void Update() {
        //print("mm count: " + movementManagers.Count);
    }

    public FlowFieldManager GetFlowFieldManager(){
        return new FlowFieldManager(cols, rows, cellSize, terrainLayer);
    }


    public void UpdateGrids(){
        foreach (MovementManager mm in movementManagers){
            mm.flowFieldManager.MakeWeightGrid();
            mm.flowFieldManager.CreateGridFromMousePos(mm.flowFieldManager.targetPoint[0]);
        }
    }
    
}

/*
This should gather all movement mannagers
If something is placed in the world, it should tell them all to update their weight field and then recalculate the path.

It should also be responseble for making the same grid for all movement managers.

*/
