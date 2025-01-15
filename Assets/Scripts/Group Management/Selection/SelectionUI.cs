using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectionUI
{
    public SelectionManager sm;
    public UIFacade uiFacade;


    public SelectionUI(SelectionManager _sm, UIFacade _uiFacade){
        sm = _sm;
        uiFacade = _uiFacade;
    }

    public void HandleUI(){
        if (sm.selectedAgents.Count > 0){
            uiFacade.UI.SetActive(true);
            if (sm.selectedAgents[0] is AgentControllerBoid)
            {
                // Change UI button colors based on selecterMode
                uiFacade.UIButtons[0].image.color = sm.selecterMode == 0 ? Color.green : Color.white;
            } 

            Abilities abilities = sm.selectedAgents[0].GetAbilities();

            for (int i = 0; i < uiFacade.UIButtons.Length; i++)
            {
                if (i >= abilities.abilityNames.Count){
                    uiFacade.UIButtons[i].gameObject.SetActive(false);
                    continue;
                } else {
                    //print("settingActive: " + i);
                    uiFacade.UIButtons[i].gameObject.SetActive(true);
                }
                //uiFacade.UIButtons[i].tmpText.text = abilities.abilityNames[i];
            }

            if (sm.selectedAgents[0] is BuildingController)
            {
                BuildingController b = sm.selectedAgents[0] as BuildingController;
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
            sm.selecterMode = -1;
            uiFacade.UI.SetActive(false);
        }

        // Check for input for UI
        CheckUIInput();
    }

    void CheckUIInput(){
        if (Input.GetKeyDown(KeyCode.T) && sm.selectedAgents.Count > 0 && sm.selectedAgents[0] is AgentControllerBoid)
        {
            sm.SetSelecterMode(0);
        }
    }
}