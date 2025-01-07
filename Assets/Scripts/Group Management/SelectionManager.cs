using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;


public class SelectionManager : MonoBehaviour
{
    public RectTransform selectionBox;

    private List<AgentControllerBoid> selectedAgents;
    //private List<AgentControllerBoid> selectedAgentsPrevios;

    private List<AgentControllerBoid> highlightedAgents;
    //private List<AgentControllerBoid> highlightedAgentsPrevios;

    public LayerMask terrainLayer; 
    public Transform targetPoint;

    bool mouseDown;

    public GridRenderer gridRenderer;

    private int selecterMode = 0;
    int UILayer;

    [SerializeField] private GameObject UI;

    Vector2 mouseStart;
    // Start is called before the first frame update
    void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");
        selectedAgents = new List<AgentControllerBoid>();
        highlightedAgents = new List<AgentControllerBoid>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleUI();

        // Selection box
        SelectionBoxLogic();

        // Move selected agents
        ActionSelectedAgents();
        
    }

    void HandleUI(){
        if (selectedAgents.Count > 0){
            UI.SetActive(true);
        } else {
            selecterMode = 0;
            UI.SetActive(false);
        }
    }

    public void SetSelecterMode(int mode){
        if (mode == selecterMode) selecterMode = 0;
        else
        selecterMode = mode;

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
            if (selecterMode == 0){
                MoveSelectedAgents();

            } else if (selecterMode == 1){
                print("Attack");
                MoveSelectedAgents();
                // Move to the target of the attack. Either a unit/building or a point on the map.
                // If a point on the map, move to that point and attack any units in its path.
                // If a unit/building, move to that: In pathfinding add all the tiles the building occupies as start.

            }
        }
        
    }

    void MoveSelectedAgents(){
        FlowFieldManager flowFieldManager = new FlowFieldManager(40, 40, 0.5f, terrainLayer, targetPoint.position);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!flowFieldManager.CreateGridFromMousePos(mousePos)) return;

        gridRenderer.flowFieldManager = flowFieldManager;

        targetPoint.position = new Vector3(mousePos.x, mousePos.y, 0);


        MovementManager movementManager = new MovementManager(flowFieldManager, selectedAgents.ToList());

        foreach (AgentControllerBoid agent in selectedAgents)
        {
            agent.SetMovementManager(movementManager);
        }
    }

    void SelectionBoxLogic(){
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
        {
            mouseStart = Input.mousePosition;
            mouseDown = true;
        }

        if (Input.GetMouseButtonUp(0) && mouseDown)
        {
            selecterMode = 0;

            // Deselect all agents
            foreach (AgentControllerBoid agent in selectedAgents)
            {
                agent.SetSelectionCircleActive(0);
            }
            selectedAgents.Clear();

            mouseDown = false;
            if (selectionBox.gameObject.activeSelf) {
                selectionBox.gameObject.SetActive(false);

                
                
                foreach (AgentControllerBoid agent in highlightedAgents)
                {
                    agent.SetSelectionCircleActive(2);
                    selectedAgents.Add(agent);
                }

            } else {
                selectedAgents = SelectUnits(mouseStart, mouseStart, padding: 0.1f);
                print("Selecting single agent: " + selectedAgents.Count);
                if (selectedAgents.Count > 0)
                {
                    selectedAgents = new List<AgentControllerBoid> { selectedAgents[0] };
                    selectedAgents[0].SetSelectionCircleActive(2);

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

            
            List<AgentControllerBoid> foundInBox = SelectUnits(mouseStart, (Vector2)Input.mousePosition);

            List<AgentControllerBoid> removeFromList = ListComparison.FindInBNotInA(foundInBox, highlightedAgents);
            List<AgentControllerBoid> addToList = ListComparison.FindInBNotInA(highlightedAgents, foundInBox);

            foreach (AgentControllerBoid agent in removeFromList)
            {
                // Deselect this unit
                agent.SetSelectionCircleActive(0);

                // Remove from selected list
                highlightedAgents.Remove(agent);
            }

            foreach (AgentControllerBoid agent in addToList)
            {
                // Select this unit
                agent.SetSelectionCircleActive(1);

                // Add to selected list
                highlightedAgents.Add(agent);
            }
        }
    }

    List<AgentControllerBoid> SelectUnits(Vector2 startPos, Vector2 endPos, float padding = 0.07f){
        List<AgentControllerBoid> selectedA = new List<AgentControllerBoid>();

        Vector2 min = Camera.main.ScreenToWorldPoint(new Vector2(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y)));
        Vector2 max = Camera.main.ScreenToWorldPoint(new Vector2(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.y, endPos.y)));

        Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);
        foreach (Collider2D collider in colliders)
        {
            AgentControllerBoid agent = collider.GetComponent<AgentControllerBoid>();
            if (agent != null)
            {
                if (IsPointWithinBounds(collider.transform.position, min, max, padding)) {
                    selectedA.Add(agent);
                }
                
            }
        }
        //print(selectedA.Count + " agents selected, colliders: " + colliders.Length);
        return selectedA;
        
    }

    public static bool IsPointWithinBounds(Vector2 point, Vector2 bound1, Vector2 bound2, float padding = 0)
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
