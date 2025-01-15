using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SelectionSelection// : MonoBehaviour
{
    private SelectionManager sm;
    private bool mouseDown;
    private Vector2 mouseStart;

    public RectTransform selectionBox;

    public SelectionSelection(SelectionManager _sm, RectTransform _selectionBox){
        sm = _sm;
        selectionBox = _selectionBox;
    }

    public void SelectionBoxLogic(){
        
        if (Input.GetMouseButtonDown(0) && !SelectionUtils.IsPointerOverUIElement())
        {
            sm.team = Input.GetKey(KeyCode.RightAlt)? 1 : 0;
            mouseStart = Input.mousePosition;
            mouseDown = true;
        }

        if (Input.GetMouseButtonUp(0) && mouseDown)
        {
            sm.selecterMode = -1;

            // Deselect all agents
            foreach (Selectable agent in sm.selectedAgents)
            {
                if (agent != null)
                    agent.SetSelectionCircleActive(0);
            }
            sm.selectedAgents.Clear();

            mouseDown = false;
            if (selectionBox.gameObject.activeSelf) {
                selectionBox.gameObject.SetActive(false);

                foreach (Selectable agent in sm.highlightedAgents)
                {
                    if (agent != null){
                        agent.SetSelectionCircleActive(2);
                        sm.selectedAgents.Add(agent);
                    }
                }

            } else {
                sm.selectedAgents = SelectUnits(mouseStart, mouseStart, padding: 0.1f);
                //print("Selecting single agent: " + selectedAgents.Count);
                if (sm.selectedAgents.Count > 0)
                {
                    if (sm.selectedAgents[0] != null){
                        
                        sm.selectedAgents = new List<Selectable> { sm.selectedAgents[0] };
                        sm.selectedAgents[0].SetSelectionCircleActive(2);
                    }

                } 
                
            }
            

            

        }

        if (mouseDown){
            if (Vector2.Distance(mouseStart, Input.mousePosition) < 30 && !selectionBox.gameObject.activeSelf){
                return;
            } else if (!selectionBox.gameObject.activeSelf){
                selectionBox.gameObject.SetActive(true);
            }

            float boxWidth = SelectionUtils.GetMousePos(Input.mousePosition).x - SelectionUtils.GetMousePos(mouseStart).x;
            float boxHeight = SelectionUtils.GetMousePos(Input.mousePosition).y - SelectionUtils.GetMousePos(mouseStart).y;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(boxWidth), Mathf.Abs(boxHeight));
            selectionBox.anchoredPosition = (Vector2)SelectionUtils.GetMousePos(mouseStart) + new Vector2(boxWidth/2, boxHeight/2);

            // Try to change this to Input.mousePosition!
            List<Selectable> foundInBox = SelectUnits(mouseStart, (Vector2)Input.mousePosition);

            List<Selectable> removeFromList = ListComparison.FindInBNotInA(foundInBox, sm.highlightedAgents);
            List<Selectable> addToList = ListComparison.FindInBNotInA(sm.highlightedAgents, foundInBox);

            foreach (Selectable agent in removeFromList)
            {
                if (agent != null){
                    // Deselect this unit
                    agent.SetSelectionCircleActive(0);

                    // Remove from selected list
                    sm.highlightedAgents.Remove(agent);
                }
            }

            foreach (Selectable agent in addToList)
            {
                
                if (agent.team == sm.team) {
                    // Select this unit
                    agent.SetSelectionCircleActive(1);

                    // Add to selected list
                    sm.highlightedAgents.Add(agent);
                }
            }
        }
    }

    List<Selectable> SelectUnits(Vector2 startPos, Vector2 endPos, float padding = 0.07f){
        List<Selectable> selectedA = new List<Selectable>();

        Vector2 min = Camera.main.ScreenToWorldPoint(new Vector2(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y)));
        Vector2 max = Camera.main.ScreenToWorldPoint(new Vector2(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.y, endPos.y)));

        Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);
        foreach (Collider2D collider in colliders)
        {
            Selectable agent = collider.GetComponent<Selectable>();
            if (agent != null)
            {
                if (SelectionUtils.IsPointWithinBounds(collider.transform.position, min, max, padding)) {
                    selectedA.Add(agent);
                }
                
            }
        }
        //print(selectedA.Count + " agents selected, colliders: " + colliders.Length);

        // if selection contains 1 or more units, remove all buildings. 
        bool containsAgent = false;
        foreach (Selectable agent in selectedA)
        {
            if (agent is AgentControllerBoid)
            {
                containsAgent = true;
            }
        }

        if (containsAgent)
        {
            selectedA = selectedA.Where(x => x is AgentControllerBoid).ToList();
        }

        return selectedA;
        
    }


}