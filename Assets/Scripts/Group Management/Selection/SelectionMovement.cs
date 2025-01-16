using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class SelectionMovement
{
    private SelectionManager sm;
    private Transform targetPoint;
    private GridRenderer gridRenderer;

    private PathFindingController pfCont;


    public SelectionMovement(SelectionManager sm, Transform targetPoint, GridRenderer gridRenderer, PathFindingController pfCont){
        this.sm = sm;
        this.targetPoint = targetPoint;
        this.gridRenderer = gridRenderer;
        this.pfCont = pfCont;
    }

    public void MoveSelectedAgents(bool AttackMove, List<AgentMoveable> agents){
        FlowFieldManager flowFieldManager = pfCont.GetFlowFieldManager();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!flowFieldManager.CreateGridFromMousePos(mousePos)) return;

        gridRenderer.flowFieldManager = flowFieldManager;

        

        MovementManager movementManager;
        if (AttackMove){
            movementManager = new AttackMovementManager(flowFieldManager, agents.ToList());

        } else {
            movementManager = new BasicMovementManager(flowFieldManager, agents.ToList());

        }

        targetPoint.position = new Vector3(mousePos.x, mousePos.y, 0);
        targetPoint.GetComponent<MarkerAnimation>().PlaceMarker(AttackMove);

        // add mm to targetpoint for event. 
        movementManager.InitialArrival += () => targetPoint.GetComponent<MarkerAnimation>().RemoveMarker();


        pfCont.AddMM(movementManager);


        foreach (AgentMoveable agent in sm.selectables)
        {
            // MovementManager movementManager;
            // if (AttackMove){
            //     movementManager = new AttackMovementManager(flowFieldManager, agents.ToList());

            // } else {
            //     movementManager = new BasicMovementManager(flowFieldManager, agents.ToList());

            // }

            // pfCont.AddMM(movementManager);

            agent.SetMovementManager(movementManager);
        }
    }
}