using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;


public class SelectionManager : MonoBehaviour
{
    public RectTransform selectionBox;

    private List<Selectable> selectedAgents;
    //private List<AgentControllerBoid> selectedAgentsPrevios;

    private List<Selectable> highlightedAgents;
    //private List<AgentControllerBoid> highlightedAgentsPrevios;

    public LayerMask terrainLayer; 
    public Transform targetPoint;

    private bool mouseDown;

    public GridRenderer gridRenderer;

    private int selecterMode = -1;
    private int UILayer;

    private Vector2 mouseStart;

    private int team = -1;

    public UIFacade uiFacade;


    // Start is called before the first frame update
    void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");
        selectedAgents = new List<Selectable>();
        highlightedAgents = new List<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleUI();

        // Selection box
        SelectionBoxLogic();

        // Move selected agents
        ActionSelectedAgents();

        // Check for input for UI
        CheckUIInput();
    }

    void CheckUIInput(){
        if (Input.GetKeyDown(KeyCode.T) && selectedAgents.Count > 0 && selectedAgents[0] is AgentControllerBoid)
        {
            SetSelecterMode(0);
        }
    }

    void HandleUI(){
        if (selectedAgents.Count > 0){
            uiFacade.UI.SetActive(true);
            if (selectedAgents[0] is AgentControllerBoid)
            {
                // Change UI button colors based on selecterMode
                uiFacade.UIButtons[0].image.color = selecterMode == 0 ? Color.green : Color.white;
            } 

            Abilities abilities = selectedAgents[0].GetAbilities();

            for (int i = 0; i < uiFacade.UIButtons.Length; i++)
            {
                if (i >= abilities.abilityNames.Count){
                    uiFacade.UIButtons[i].gameObject.SetActive(false);
                    continue;
                } else {
                    //print("settingActive: " + i);
                    uiFacade.UIButtons[i].gameObject.SetActive(true);
                }
                uiFacade.UIButtons[i].tmpText.text = abilities.abilityNames[i];
            }

            if (selectedAgents[0] is BuildingController)
            {
                BuildingController b = selectedAgents[0] as BuildingController;
                float p = b.buildingProduction.GetFinishPercentage();

                if (p == -1){
                    uiFacade.UIProgressBarBackground.gameObject.SetActive(false);
                } else {
                    uiFacade.UIProgressBarBackground.gameObject.SetActive(true);
                    uiFacade.UIProgressBar.localScale = new Vector3(p, 1, 1);
                }
            } else {
                uiFacade.UIProgressBarBackground.gameObject.SetActive(false);
            }

        } else {
            selecterMode = -1;
            uiFacade.UI.SetActive(false);
        }
    }

    public void SetSelecterMode(int mode){
        if (selectedAgents.Count == 0) return;

        if (selectedAgents[0] is AgentControllerBoid) {
            if (mode == selecterMode) {
                selecterMode = -1;
            } else {
                selecterMode = mode;
            }
        } else {
            List<BuildingController> buildings = selectedAgents.Cast<BuildingController>().ToList();

            foreach (BuildingController building in buildings)
            {
                building.AddToProduction(mode);
            }
        }

    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    void ActionSelectedAgents(){
        if (Input.GetMouseButtonDown(1) && selectedAgents.Count > 0 && !IsPointerOverUIElement())
        {
            if (selectedAgents[0] is AgentControllerBoid)
            {
                List<AgentControllerBoid> agents = selectedAgents.Cast<AgentControllerBoid>().ToList();
                if (selecterMode == -1){
                    MoveSelectedAgents(false, agents);

                } else if (selecterMode == 0){
                    MoveSelectedAgents(true, agents);
                    SetSelecterMode(0);         
                }
            } 
        }
    }

    void MoveSelectedAgents(bool AttackMove, List<AgentControllerBoid> agents){
        FlowFieldManager flowFieldManager = new FlowFieldManager(64, 64, 0.5f, terrainLayer);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!flowFieldManager.CreateGridFromMousePos(mousePos)) return;

        gridRenderer.flowFieldManager = flowFieldManager;

        targetPoint.position = new Vector3(mousePos.x, mousePos.y, 0);

        MovementManager movementManager;
        if (AttackMove){
            movementManager = new AttackMovementManager(flowFieldManager, agents.ToList());

        } else {
            movementManager = new BasicMovementManager(flowFieldManager, agents.ToList());

        }


        foreach (AgentControllerBoid agent in selectedAgents)
        {
            agent.SetMovementManager(movementManager);
        }
    }


    void SelectionBoxLogic(){
        
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
        {
            team = Input.GetKey(KeyCode.RightAlt)? 1 : 0;
            mouseStart = Input.mousePosition;
            mouseDown = true;
        }

        if (Input.GetMouseButtonUp(0) && mouseDown)
        {
            selecterMode = -1;

            // Deselect all agents
            foreach (Selectable agent in selectedAgents)
            {
                if (agent != null)
                    agent.SetSelectionCircleActive(0);
            }
            selectedAgents.Clear();

            mouseDown = false;
            if (selectionBox.gameObject.activeSelf) {
                selectionBox.gameObject.SetActive(false);

                foreach (Selectable agent in highlightedAgents)
                {
                    if (agent != null){
                        agent.SetSelectionCircleActive(2);
                        selectedAgents.Add(agent);
                    }
                }

            } else {
                selectedAgents = SelectUnits(mouseStart, mouseStart, padding: 0.1f);
                print("Selecting single agent: " + selectedAgents.Count);
                if (selectedAgents.Count > 0)
                {
                    if (selectedAgents[0] != null){
                        
                        selectedAgents = new List<Selectable> { selectedAgents[0] };
                        selectedAgents[0].SetSelectionCircleActive(2);
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

            float boxWidth = Input.mousePosition.x - mouseStart.x;
            float boxHeight = Input.mousePosition.y - mouseStart.y;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(boxWidth), Mathf.Abs(boxHeight));
            selectionBox.anchoredPosition = mouseStart + new Vector2(boxWidth/2, boxHeight/2);

            
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
                
                if (agent.team == team) {
                    // Select this unit
                    agent.SetSelectionCircleActive(1);

                    // Add to selected list
                    highlightedAgents.Add(agent);
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
                if (IsPointWithinBounds(collider.transform.position, min, max, padding)) {
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


    private static bool IsPointWithinBounds(Vector2 point, Vector2 bound1, Vector2 bound2, float padding = 0)
    {
        // Calculate min and max bounds
        float minX = Mathf.Min(bound1.x, bound2.x) - padding;
        float maxX = Mathf.Max(bound1.x, bound2.x) + padding;
        float minY = Mathf.Min(bound1.y, bound2.y) - padding;
        float maxY = Mathf.Max(bound1.y, bound2.y) + padding;

        // Check if the point is within the bounds
        return point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY;
    }

}
