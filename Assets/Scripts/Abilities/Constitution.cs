using UnityEngine;
using System;


public class StatsManager
{
    // Health properties
    private float _currentHealth;
    private float _maxHealth;

    // Defense properties
    private float _baseDefense;
    private float _currentDefenseModifier;

    // Speed properties
    private float _baseSpeed;
    private float _currentSpeedModifier;

    // Constructor
    public StatsManager(float maxHealth, float baseDefense, float baseSpeed)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth; // Initialize with full health
        _baseDefense = baseDefense;
        _currentDefenseModifier = 1.0f; // Default no defense modifier
        _baseSpeed = baseSpeed;
        _currentSpeedModifier = 1.0f; // Default no speed modifier
    }

    // Health management
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;

    public void AddHealth(float amount)
    {
        _currentHealth = Math.Min(_currentHealth + amount, _maxHealth);
    }

    public void SubtractHealth(float amount)
    {
        _currentHealth = Math.Max(_currentHealth - amount, 0);
    }

    public void SetMaxHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = Math.Min(_currentHealth, _maxHealth); // Adjust current health if it exceeds max
    }

    // Defense management
    public float CurrentDefense => _baseDefense * _currentDefenseModifier;

    public void SetDefenseModifier(float modifier)
    {
        if (modifier > _currentDefenseModifier) // Only accept better modifiers
        {
            _currentDefenseModifier = modifier;
        }
    }

    public void ResetDefenseModifier()
    {
        _currentDefenseModifier = 1.0f; // Reset to default
    }

    // Speed management
    public float CurrentSpeed => _baseSpeed * _currentSpeedModifier;

    public void SetSpeedModifier(float modifier)
    {
        if (modifier > _currentSpeedModifier) // Only accept better modifiers
        {
            _currentSpeedModifier = modifier;
        }
    }

    public void ResetSpeedModifier()
    {
        _currentSpeedModifier = 1.0f; // Reset to default
    }
}
