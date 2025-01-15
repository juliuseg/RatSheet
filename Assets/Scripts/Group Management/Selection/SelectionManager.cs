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

    [HideInInspector] public List<Selectable> selectedAgents;
    //private List<AgentControllerBoid> selectedAgentsPrevios;

    [HideInInspector] public List<Selectable> highlightedAgents;
    //private List<AgentControllerBoid> highlightedAgentsPrevios;

    public Transform targetPoint;


    public GridRenderer gridRenderer;

    [SerializeField] public int selecterMode = -1;


    [HideInInspector] public int team;

    public UIFacade uiFacade;

    [SerializeField] private PathFindingController pfCont;


    private SelectionSelection selection;

    private SelectionUI selectionUI;

    private SelectionAction selectionAction;
    private SelectionMovement selectionMovement;

    // Start is called before the first frame update
    void Start()
    {
        selectedAgents = new List<Selectable>();
        highlightedAgents = new List<Selectable>();
        team = -1;

        selection = new SelectionSelection(this, selectionBox);
        selectionUI = new SelectionUI(this, uiFacade);

        selectionMovement = new SelectionMovement(this, targetPoint, gridRenderer, pfCont);
        selectionAction = new SelectionAction(this, selectionMovement);

    }

    // Update is called once per frame
    void Update()
    {
        selectionUI.HandleUI();

        // Selection box
        selection.SelectionBoxLogic();

        // Move selected agents
        selectionAction.ActionSelectedAgents();

    }

    public void SetSelecterMode(int mode){
        print("Setting selecter mode: " + mode);
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
}