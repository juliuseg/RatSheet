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
        if (sm.selectables.Count > 0){
            uiFacade.UI.SetActive(true);
            if (sm.selectables[0] is AgentControllerBoid)
            {
                for (int i = 0; i < uiFacade.UIButtons.Length; i++)
                {
                    uiFacade.UIButtons[i].image.color = sm.selecterMode == i ? SelectionUtils.GetUIColor() : Color.white;
                }
            }

            Abilities abilities = sm.selectables[0].GetAbilities();

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

        } else {
            sm.selecterMode = 0;
            uiFacade.UI.SetActive(false);
        }

        // Check for input for UI
        CheckUIInput();
    }

    void CheckUIInput(){
        if (Input.GetKeyDown(KeyCode.T) && sm.selectables.Count > 0 && sm.selectables[0] is AgentControllerBoid)
        {
            sm.SetSelecterMode(1);
        }
        if (Input.GetKeyDown(KeyCode.S) && sm.selectables.Count > 0 && sm.selectables[0] is AgentControllerBoid)
        {
            sm.SetSelecterMode(2);
        }
    }
}