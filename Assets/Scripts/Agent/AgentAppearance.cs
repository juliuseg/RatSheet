using UnityEngine;

public class AgentAppearance{
    private GameObject selectionCircle;
    private SpriteRenderer spriteRenderer;
    

    public AgentAppearance(GameObject _selectionCircle, SpriteRenderer _spriteRenderer)
    {
        selectionCircle = _selectionCircle;
        spriteRenderer = _spriteRenderer;
    }

    public void SetSelectionCircleActive(int mode) // mode 0 = off, mode 1 = highlighted in red, mode 2 = selected in green
    {
        if (mode == 0)
        {
            selectionCircle.SetActive(false);
        }
        else if (mode == 1)
        {
            selectionCircle.SetActive(true);
            selectionCircle.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.5f);
        }
        else if (mode == 2)
        {
            selectionCircle.SetActive(true);
            selectionCircle.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.7f);
        }
    }


    public void AdjustAgentAppearance(bool arrived, bool arrivedCorrection, MovementManager movementManager, int neighborCount)
    {
        spriteRenderer.color = arrived ? Color.green : Color.white;
        if (arrivedCorrection) spriteRenderer.color = Color.red;
        if (neighborCount == 0 && movementManager.GetAgentCount() != 1) spriteRenderer.color = Color.yellow;
    }
}