using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class SelectionUI
{
    public SelectionManager sm;
    public UIFacade uiFacade;


    public SelectionUI(SelectionManager _sm, UIFacade _uiFacade){
        sm = _sm;
        uiFacade = _uiFacade;

        sm.OnSelectionChange += SetUIInfoForSelected;

        uiFacade.SetUIActive(false);
    }

    public void HandleUI(){
        if (sm.selectables.Count > 0){
            uiFacade.UI.SetActive(true);

            // Set color of actions buttons.
            SetColorOfSelectedAction();

            // Set UI info
            // SetUIInfoForSelected();

            // For buildings set also the progress bar:
            SetProgressBar();

            // Check for input for UI
            CheckUIInput();

            

        } else {
            // If no selected, hide UI
            sm.selecterMode = 0;
            uiFacade.UI.SetActive(false);
        }
    }

    void SetUIInfoForSelected() {
        if (sm.selectables.Count > 0){
            uiFacade.SetUIActive(true);
            if (sm.selectables[0] is AgentMoveable)
            {
                // Mini portraits
                List<Sprite> miniPortraits = new List<Sprite>();
                List<float> healths = new List<float>();
                foreach (AgentMoveable agent in sm.selectables)
                {
                    SelectableUIInfo info = agent.stats.uiInfo;
                    miniPortraits.Add(info.miniPortrait);
                    healths.Add(agent.health.GetHPPerc());

                    
                }
                uiFacade.SetMiniPortrait(miniPortraits, healths);
            } else if (sm.selectables[0] is BuildingController)
            {
                uiFacade.SetMiniPortrait(new List<Sprite>(), new List<float>());
            }
            
            // Set name, description and portrait
            SelectableUIInfo selectedInfo = sm.selectables[0].stats.uiInfo;
            
            uiFacade.SetSelectableInfo(selectedInfo.nameText, selectedInfo.descriptionText, selectedInfo.portrait);

            List<string> actionNames = new List<string>();
            List<string> actionDescriptions = new List<string>();
            List<Sprite> actionIcons = new List<Sprite>();
            foreach(ActionUIInfo actionInfo in selectedInfo.actionsInfos){
                actionNames.Add(actionInfo.nameText);
                actionDescriptions.Add(actionInfo.descriptionText);
                actionIcons.Add(actionInfo.icon);
                
            }

            uiFacade.SetActionText(actionNames, actionDescriptions);
            uiFacade.SetActionIcons(actionIcons);
        } else {
            uiFacade.SetUIActive(false);
        }
    }

    private void SetColorOfSelectedAction(){
        Color selectedColor = SelectionUtils.GetUIColor();
        bool isAgent = sm.selectables[0] is AgentMoveable;
        for (int i = 0; i < uiFacade.UIButtons.Length; i++)
        {
            uiFacade.UIButtons[i].image.color = isAgent && sm.selecterMode == i ? selectedColor : Color.white;
        }
    }

    private void SetProgressBar(){
        if (sm.selectables[0] is BuildingController)
            {
                BuildingController b = sm.selectables[0] as BuildingController;
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

    }

    void CheckUIInput(){
        if (Input.GetKeyDown(KeyCode.T) && sm.selectables.Count > 0 && sm.selectables[0] is AgentMoveable)
        {
            sm.SetSelecterMode(1);
        }
        if (Input.GetKeyDown(KeyCode.S) && sm.selectables.Count > 0 && sm.selectables[0] is AgentMoveable)
        {
            sm.SetSelecterMode(2);
        }
    }
}