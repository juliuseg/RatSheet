using UnityEngine;

public class Ability
{
    // Cooldown
    private float cooldownMax;
    private float cooldownTimer;

    // Hit type
    private HitType hitType;

    // Cost
    private int energyCost;

    // Ability name
    private string abilityName;

    // Effect
    private Effect[] debuffEffect; // Can be debuff to health, speed, etc. Applies to all enemies hit
    private Effect[] buffEffect; // Can be buff to health, speed, etc. Applies to all allies hit




    // Constructor
    public Ability(float _cooldownMax, HitType _hitType, int _energyCost = 0, string _abilityName = "Name missing", float _areaRadius = 0)
    {
        cooldownMax = _cooldownMax;
        cooldownTimer = 0;
        hitType = _hitType;
        energyCost = _energyCost;
        abilityName = _abilityName;
    }


}
