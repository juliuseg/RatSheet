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
        if (Input .GetMouseButtonDown(1) && sm.selectedAgents.Count > 0 && !SelectionUtils.IsPointerOverUIElement())
        {
            if (sm.selectedAgents[0] is AgentControllerBoid)
            {
                List<AgentControllerBoid> agents = sm.selectedAgents.Cast<AgentControllerBoid>().ToList();
                if (sm.selecterMode == -1){
                    smov.MoveSelectedAgents(false, agents);

                } else if (sm.selecterMode == 0){
                    smov.MoveSelectedAgents(true, agents);
                    sm.SetSelecterMode(0);         
                }
            } 
        }
    }
}