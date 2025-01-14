using UnityEngine;

public class BuildingAppearance{
    private GameObject selectionCircle;
    private SpriteRenderer spriteRenderer;

    private int team;

    private debugMode debug = debugMode.attack;

    private enum debugMode {
        none,
        arrived,
        attack,
    }
    

    public BuildingAppearance(GameObject _selectionCircle, SpriteRenderer _spriteRenderer, int _team)
    {
        selectionCircle = _selectionCircle;
        spriteRenderer = _spriteRenderer;
        team = _team;
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
}