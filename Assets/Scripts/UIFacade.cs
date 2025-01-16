using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFacade : MonoBehaviour
{
    public GameObject UI;
    public GameObject textBox;
    public UIButton[] UIButtons;
    public RectTransform UIProgressBar;
    public RectTransform UIProgressBarBackground;

    public MiniPortraitSetter miniPortraitSetter;

    public Vector2 miniPortraitPosition;
    public List<Image> miniPortraitsRenderers;
    public List<Image> switchButtons;

    [SerializeField] private TextBoxFacade textBoxFacade;

    private List<string> actionNames;
    private List<string> actionDescriptions;

    [SerializeField] private List<string> ResNames;
    [SerializeField] private List<string> ResDescriptions;

    private string agentName;
    private string agentDescription;

    bool showingActions;

    public void SetUIActive(bool active)
    {
        UI.SetActive(active);
        foreach (UIButton button in UIButtons)
        {
            button.image.enabled = active;
        }

        UIProgressBar.gameObject.SetActive(active);
        UIProgressBarBackground.gameObject.SetActive(active);

        miniPortraitSetter = new MiniPortraitSetter(miniPortraitsRenderers, switchButtons);

        SetTextBoxTransparency(active);

        if (!active)
        {
            showingActions = false;
        }

        
    }

    public void SetMiniPortrait(List<Sprite> miniPortraitsSprites, List<float> healths)
    {
        miniPortraitSetter.SetMiniPortrait(miniPortraitsSprites, healths);
    }

    public void SetSelectableInfo(string name, string description, Sprite portrait)
    {
        agentName = name;
        agentDescription = description;


        textBoxFacade.SetTextPortrait(portrait);

        if (!showingActions)
        {
            SetTextBox(name, description);
        }
    }

    public void SetActionIcons(List<Sprite> buttonIcons){
        for (int i = 0; i < UIButtons.Length; i++)
        {
            if (i >= buttonIcons.Count){
                UIButtons[i].gameObject.SetActive(false);
                continue;
            } else {
                UIButtons[i].gameObject.SetActive(true);
                UIButtons[i].image.sprite = buttonIcons[i];
            }
        }
    }

    public void SetTextBox(string name, string description)
    {
        Debug.Log("Setting text box");
        textBoxFacade.SetTextBox(name, description);
    }

    public void SetActionText(List<String> _actionNames, List<String> _actionDescriptions)
    {
        actionNames = _actionNames;
        actionDescriptions = _actionDescriptions;
    }

    void SetTextBoxTransparency(bool active)
    {
        Color color = textBox.GetComponent<Image>().color;
        color.a = active ? 1 : 0.5f;
        textBox.GetComponent<Image>().color = color;
        textBoxFacade.ActivateText(active);
    }

    public void AddOffsetIndex(int value)
    {
        miniPortraitSetter.AddOffsetIndex(value);

    }

    public void HoverOverEnter(string hoverName)
    {
        int index = 0;
        if (int.TryParse(hoverName.Replace("Abb", ""), out index) && index < actionNames.Count)
        {
            SetTextBox(actionNames[index], actionDescriptions[index]);
        }

        if (int.TryParse(hoverName.Replace("Res", ""), out index) && index < ResDescriptions.Count)
        {
            SetTextBoxTransparency(true);
            SetTextBox(ResNames[index], ResDescriptions[index]);
            
            print("Hovering over " + ResNames[index]);
        }

        
    }

    public void HoverOverExit(string hoverName)
    {
        SetTextBox(agentName, agentDescription);

        if (hoverName.StartsWith("Res") && !UI.activeSelf)
        {
            SetTextBoxTransparency(false);
        }
    }

}

