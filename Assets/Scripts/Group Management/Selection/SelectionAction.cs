using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class SelectionAction
{
    private SelectionManager sm;
    private SelectionMovement smov;

    public SelectionAction(SelectionManager sm, SelectionMovement smov){
        this.sm = sm;
        this.smov = smov;
    }

    public void ActionSelectedAgents(){
        if (Input .GetMouseButtonDown(1) && sm.selectables.Count > 0 && !SelectionUtils.IsPointerOverUIElement())
        {
            if (sm.selectables[0] is AgentControllerBoid)
            {
                List<AgentControllerBoid> agents = sm.selectables.Cast<AgentControllerBoid>().ToList();
                if (sm.selecterMode == 0){
                    smov.MoveSelectedAgents(false, agents);

                } else if (sm.selecterMode == 1){
                    smov.MoveSelectedAgents(true, agents);
                    sm.SetSelecterMode(0);         
                } 

            } 
        } else if (sm.selecterMode == 2){
            if (sm.selectables[0] is AgentControllerBoid)
            {
                List<AgentControllerBoid> agents = sm.selectables.Cast<AgentControllerBoid>().ToList();

                // Stop selected agents
                foreach (AgentControllerBoid agent in agents)
                {
                    agent.SetMovementManager(null);
                }

                sm.SetSelecterMode(0);
            }
            
        }
    }
    public void ActionSelectedBuildings(int mode){
        List<BuildingController> buildings = sm.selectables.Cast<BuildingController>().ToList();

        foreach (BuildingController building in buildings)
        {
            building.AddToProduction(mode);
        }
    }
}