using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class SelectionSelection// : MonoBehaviour
{
    private SelectionManager sm;
    private bool mouseDown;
    private Vector2 mouseStart;

    private RectTransform selectionBox;
    private List<Selectable> highlightedAgents;


    public SelectionSelection(SelectionManager _sm, RectTransform _selectionBox){
        sm = _sm;
        selectionBox = _selectionBox;

        highlightedAgents = new List<Selectable>();
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
            sm.selecterMode = 0;

            // Deselect all agents
            foreach (Selectable agent in sm.selectables)
            {
                if (agent != null)
                    agent.SetSelectionCircleActive(0);
            }
            sm.selectables.Clear();

            mouseDown = false;
            if (selectionBox.gameObject.activeSelf) {
                selectionBox.gameObject.SetActive(false);

                foreach (Selectable agent in highlightedAgents)
                {
                    if (agent != null){
                        AddAgentToSelection(agent); 
                    }
                }

            } else {
                List<Selectable> selected = SelectUnits(mouseStart, mouseStart, padding: 0.1f);

                if (selected.Count > 0)
                {
                    
                    if (selected[0] != null){
                        AddAgentToSelection(selected[0]);
                    }
                } 
            }

            sm.NotifySelectionChanged();
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

            List<Selectable> removeFromList = ListComparison.FindInBNotInA(foundInBox, highlightedAgents);
            List<Selectable> addToList = ListComparison.FindInBNotInA(highlightedAgents, foundInBox);

            foreach (Selectable agent in removeFromList)
            {
                if (agent != null){
                    // Deselect this unit
                    agent.SetSelectionCircleActive(0);

                    // Remove from selected list
                    highlightedAgents.Remove(agent);
                }
            }

            foreach (Selectable agent in addToList)
            {
                
                if (agent.team == sm.team) {
                    // Select this unit
                    agent.SetSelectionCircleActive(1);

                    // Add to selected list
                    highlightedAgents.Add(agent);
                }
            }
        }
    }

    void AddAgentToSelection(Selectable agent){
        Debug.Log("adding to selection count: " + sm.selectables.Count);

        agent.SetSelectionCircleActive(2);
        sm.selectables.Add(agent);

        Debug.Log("selection count after add: " + sm.selectables.Count);

        
        // Here were are sure we have a valid agent and have selected it
        agent.health.OnDeath += () => RemoveDeadAgent(agent);
        agent.health.OnHealthChanged += () => sm.NotifySelectionChanged();
    }

    void RemoveDeadAgent(Selectable agent){
        sm.selectables.Remove(agent);
        
        sm.NotifySelectionChanged();
        
    }



    List<Selectable> SelectUnits(Vector2 startPos, Vector2 endPos, float padding = 0.07f){
        List<Selectable> selectedA = new List<Selectable>();

        Vector2 min = Camera.main.ScreenToWorldPoint(new Vector2(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y)));
        Vector2 max = Camera.main.ScreenToWorldPoint(new Vector2(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.y, endPos.y)));
        
        Collider2D[] colliders = new Collider2D[0];
        if (min != max){
            colliders = Physics2D.OverlapAreaAll(min, max);
        } else {
            colliders = Physics2D.OverlapPointAll(min);
        }

        foreach (Collider2D collider in colliders)
        {
            Selectable agent = collider.GetComponent<Selectable>();
            if (agent != null)
            {
                Debug.Log("col: " + collider.gameObject.name);

                if ((min == max ) || SelectionUtils.IsPointWithinBounds(collider.transform.position, min, max, padding)) {
                    selectedA.Add(agent);
                }
                
            }
        }

        //print(selectedA.Count + " agents selected, colliders: " + colliders.Length);

        // if selection contains 1 or more units, remove all buildings. 
        bool containsAgent = false;
        foreach (Selectable agent in selectedA)
        {
            if (agent is AgentMoveable)
            {
                containsAgent = true;
            }
        }

        if (containsAgent)
        {
            selectedA = selectedA.Where(x => x is AgentMoveable).ToList();
        }

        return selectedA;
        
    }


}