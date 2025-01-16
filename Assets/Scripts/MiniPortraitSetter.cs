using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniPortraitSetter
{

    private List<Image> miniPortraitsRenderers;

    private List<Sprite> miniPortraitsSprites;
    private List<float> healths;

    private int offsetIndex;

    private int maxOffsetIndex;

    private List<Image> switchButtons;

    public MiniPortraitSetter(List<Image> miniPortraitsRenderers, List<Image> switchButtons)
    {
        this.miniPortraitsRenderers = miniPortraitsRenderers;
        this.switchButtons = switchButtons;

        offsetIndex = 0;
    }

    public void SetMiniPortrait(List<Sprite> _miniPortraitsSprites, List<float> _healths)
    {
        
        miniPortraitsSprites = _miniPortraitsSprites;
        healths = _healths;

        maxOffsetIndex = Mathf.Max(0,Mathf.CeilToInt((miniPortraitsSprites.Count - 12)/6f));
        //Debug.Log("maxOffsetIndex: " + maxOffsetIndex + " from: " + miniPortraitsSprites.Count);
        offsetIndex = 0;

        UpdateMiniPortraits();

        SetSwitchButtons();
    
    }

    public void AddOffsetIndex(int value)
    {
        int beforeOffset = offsetIndex;
        offsetIndex = Mathf.Clamp(offsetIndex + value, 0, maxOffsetIndex);
        UpdateMiniPortraits();
        UpdateArrows();        
    }

    public bool CanAddOffsetIndex(int value)
    {
        int beforeOffset = offsetIndex;
        int newOffset = Mathf.Clamp(offsetIndex + value, 0, maxOffsetIndex);
        return newOffset != offsetIndex;
    }

    private void UpdateArrows()
    {
        if (maxOffsetIndex > 0){
            switchButtons[0].color = CanAddOffsetIndex(-1)? switchButtonPressable: switchButtonUnpressable;
            switchButtons[1].color = CanAddOffsetIndex(1)? switchButtonPressable: switchButtonUnpressable;
        }
    }

    private void UpdateMiniPortraits()
    {
        for (int i = 0; i < miniPortraitsRenderers.Count; i++)
        {
            int spriteIndex = i + offsetIndex*6;
            int renderIndex = miniPortraitsSprites.Count > 6 ? i : i + 6;


        
            if (spriteIndex >= miniPortraitsSprites.Count)
            {
                int index = renderIndex>=miniPortraitsRenderers.Count? renderIndex-miniPortraitsRenderers.Count:renderIndex;
                miniPortraitsRenderers[index].gameObject.SetActive(false);
                continue;
            }
            miniPortraitsRenderers[renderIndex].gameObject.SetActive(true);

            
            miniPortraitsRenderers[renderIndex].sprite = miniPortraitsSprites[spriteIndex];
            miniPortraitsRenderers[renderIndex].color = MapToColor(healths[spriteIndex]);
            
        }
    } 

    private void SetSwitchButtons(){
        if (maxOffsetIndex > 0)
        {
            switchButtons[0].gameObject.SetActive(true);
            switchButtons[1].gameObject.SetActive(true);
        } else {
            switchButtons[0].gameObject.SetActive(false);
            switchButtons[1].gameObject.SetActive(false);
        }
        UpdateArrows();
    }



    Color MapToColor(float value)
    {
        value = Mathf.Clamp01(value); // Ensure value is within [0, 1]
        
        if (value <= 0.5f)
        {
            // Map [0, 0.5] to red -> yellow
            return Color.Lerp(Color.red, Color.yellow, value / 0.5f);
        }
        else
        {
            // Map [0.5, 1] to yellow -> white
            return Color.Lerp(Color.yellow, Color.white, (value - 0.5f) / 0.5f);
        }
    }

    private readonly Color switchButtonPressable = new Color(1, 1, 1, 1f);
    private readonly Color switchButtonUnpressable = new Color(1, 1, 1, 0.35f);

    
}