using UnityEngine;
using System;

public class AgentHPController : MonoBehaviour {
    private float health;
    private float maxHealth;

    private AgentStats agentStats;

    public RectTransform healthBar;

    public event Action OnDeath;

    public void SetHealthInit(AgentStats _agentStats){
        agentStats = _agentStats;

        maxHealth = agentStats.maxHealth;
        health = maxHealth;

        UpdateHealthBar();

    }

    public void TakeDamage(float damage){
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthBar();


        if (health <= 0){
            OnDeath?.Invoke();
        }

        
    }

    public void Heal(float heal){
        health += heal;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar(){
        healthBar.localScale = new Vector3(GetHPPerc(), 1, 1);
    }

    private float GetHPPerc(){
        return health / maxHealth;
    }

}