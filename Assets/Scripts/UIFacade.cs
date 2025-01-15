using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFacade : MonoBehaviour
{
    public GameObject UI;
    public UIButton[] UIButtons;
    public RectTransform UIProgressBar;
    public RectTransform UIProgressBarBackground;

    public void SetUIActive(bool active)
    {
        UI.SetActive(active);
        foreach (UIButton button in UIButtons)
        {
            button.image.enabled = active;
            button.tmpText.enabled = active;
        }

        UIProgressBar.gameObject.SetActive(active);
        UIProgressBarBackground.gameObject.SetActive(active);
    }

}
