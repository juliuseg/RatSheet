using UnityEngine;

public class AgentAppearance{
    private GameObject selectionCircle;
    private SpriteRenderer spriteRenderer;

    private int team;

    private debugMode debug = debugMode.attack;

    private enum debugMode {
        none,
        arrived,
        attack,
    }
    

    public AgentAppearance(GameObject _selectionCircle, SpriteRenderer _spriteRenderer, int _team)
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


    public void AdjustAgentAppearance(bool arrived, bool arrivedCorrection, AttackState attackState, MovementManager movementManager, int neighborCount)
    {
        if (debug == debugMode.arrived){
            spriteRenderer.color = arrived ? Color.green : Color.white;
            if (arrivedCorrection) spriteRenderer.color = Color.red;
            if (neighborCount == 0 && movementManager.GetAgentCount() != 1) spriteRenderer.color = Color.yellow;
        } else if (debug == debugMode.attack){
            switch (attackState)
            {
                case AttackState.idle:
                    spriteRenderer.color = GetColorFromTeam(team);
                    break;
                case AttackState.moving:
                    spriteRenderer.color = GetColorFromTeam(team);
                    break;
                case AttackState.movingToAttack:
                    spriteRenderer.color = Color.magenta;
                    break;
                case AttackState.reloading:
                    spriteRenderer.color = Color.red;
                    break;
            }
        } else if (debug == debugMode.none){ 
            spriteRenderer.color = GetColorFromTeam(team);
        }
    }

    private Color GetColorFromTeam(int team){
        if (team == 0) return Color.yellow;
        else if (team == 1) return Color.cyan;
        else return Color.white;
    }
    
}

