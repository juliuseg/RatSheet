using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SelectionManager : MonoBehaviour
{
    // World class assignment:
    public RectTransform selectionBox;  
    public Transform targetPoint;
    public GridRenderer gridRenderer;
    public UIFacade uiFacade;

    [HideInInspector] public List<Selectable> selectables;
    [HideInInspector] public int selecterMode = 0;
    [HideInInspector] public int team;

    private SelectionUI selectionUI;
    private SelectionSelection selection;
    private SelectionAction selectionAction;

    public event Action OnSelectionChange;

    // Start is called before the first frame update
    void Start()
    {
        selectables = new List<Selectable>();
        team = -1;

        selection = new SelectionSelection(this, selectionBox);
        selectionUI = new SelectionUI(this, uiFacade);

        PathFindingController pfCont = GameObject.Find("PathFindingController").GetComponent<PathFindingController>();
        SelectionMovement selectionMovement = new SelectionMovement(this, targetPoint, gridRenderer, pfCont);
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
        if (selectables.Count == 0) return;

        if (selectables[0] is AgentMoveable) {
            if (mode == selecterMode) {
                selecterMode = 0;
            } else {
                selecterMode = mode;
            }
        } else if (selectables[0] is BuildingController) {
            selectionAction.ActionSelectedBuildings(mode);
        }

    }

    public void NotifySelectionChanged()
    {
        OnSelectionChange?.Invoke();
    }
}