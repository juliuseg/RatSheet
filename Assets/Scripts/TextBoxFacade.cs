using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextBoxFacade : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Image portraitImage;

    public void SetTextBox(string name, string description)
    {
        nameText.text = name;
        descriptionText.text = description;
    }

    public void SetTextPortrait(Sprite portrait)
    {
        portraitImage.sprite = portrait;
    }

    public void ActivateText(bool active)
    {
        nameText.gameObject.SetActive(active);
        descriptionText.gameObject.SetActive(active);
    }
}
