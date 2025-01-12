using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SelectionManager : MonoBehaviour
{
    public RectTransform selectionBox;

    private List<AgentControllerBoid> selectedAgents;
    //private List<AgentControllerBoid> selectedAgentsPrevios;

    private List<AgentControllerBoid> highlightedAgents;
    //private List<AgentControllerBoid> highlightedAgentsPrevios;

    public LayerMask terrainLayer; 
    public Transform targetPoint;

    private bool mouseDown;

    public GridRenderer gridRenderer;

    private int selecterMode = 0;
    private int UILayer;

    [SerializeField] private GameObject UI;

    private Vector2 mouseStart;

    private int team = -1;

    public Image[] UIButtons;

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

        // Check for input for UI
        CheckUIInput();
        
    }

    void CheckUIInput(){
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetSelecterMode(1);
        }
    }

    void HandleUI(){
        if (selectedAgents.Count > 0){
            UI.SetActive(true);

            // Change UI button colors based on selecterMode
            UIButtons[0].color = selecterMode == 1 ? Color.green : Color.white;

        } else {
            selecterMode = 0;
            UI.SetActive(false);

            
        }

        
    }

    public void SetSelecterMode(int mode){
        if (mode == selecterMode) 
        selecterMode = 0;
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
                MoveSelectedAgents(false);

            } else if (selecterMode == 1){
                print("Attack");
                MoveSelectedAgents(true);
                SetSelecterMode(0);
                
            }
        }
        
    }

    void MoveSelectedAgents(bool AttackMove){
        FlowFieldManager flowFieldManager = new FlowFieldManager(40, 40, 0.5f, terrainLayer);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!flowFieldManager.CreateGridFromMousePos(mousePos)) return;

        gridRenderer.flowFieldManager = flowFieldManager;

        targetPoint.position = new Vector3(mousePos.x, mousePos.y, 0);

        MovementManager movementManager;
        if (AttackMove){
            movementManager = new AttackMovementManager(flowFieldManager, selectedAgents.ToList());

        } else {
            movementManager = new BasicMovementManager(flowFieldManager, selectedAgents.ToList());

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
            selecterMode = 0;

            // Deselect all agents
            foreach (AgentControllerBoid agent in selectedAgents)
            {
                if (agent != null)
                    agent.agentAppearance.SetSelectionCircleActive(0);
            }
            selectedAgents.Clear();

            mouseDown = false;
            if (selectionBox.gameObject.activeSelf) {
                selectionBox.gameObject.SetActive(false);

                foreach (AgentControllerBoid agent in highlightedAgents)
                {
                    if (agent != null){
                        agent.agentAppearance.SetSelectionCircleActive(2);
                        selectedAgents.Add(agent);
                    }
                }

            } else {
                selectedAgents = SelectUnits(mouseStart, mouseStart, padding: 0.1f);
                print("Selecting single agent: " + selectedAgents.Count);
                if (selectedAgents.Count > 0)
                {
                    if (selectedAgents[0] != null){
                        selectedAgents = new List<AgentControllerBoid> { selectedAgents[0] };
                        selectedAgents[0].agentAppearance.SetSelectionCircleActive(2);
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

            
            List<AgentControllerBoid> foundInBox = SelectUnits(mouseStart, (Vector2)Input.mousePosition);

            List<AgentControllerBoid> removeFromList = ListComparison.FindInBNotInA(foundInBox, highlightedAgents);
            List<AgentControllerBoid> addToList = ListComparison.FindInBNotInA(highlightedAgents, foundInBox);

            foreach (AgentControllerBoid agent in removeFromList)
            {
                if (agent != null){
                    // Deselect this unit
                    agent.agentAppearance.SetSelectionCircleActive(0);

                    // Remove from selected list
                    highlightedAgents.Remove(agent);
                }
            }

            foreach (AgentControllerBoid agent in addToList)
            {
                
                if (agent.team == team) {
                    // Select this unit
                    agent.agentAppearance.SetSelectionCircleActive(1);

                    // Add to selected list
                    highlightedAgents.Add(agent);
                }
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
