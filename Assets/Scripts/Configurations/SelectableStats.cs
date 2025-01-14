using UnityEngine;
using System.Collections.Generic;

public abstract class SelectableStats : ScriptableObject
{
    [Header("Health")]
    public float maxHealth = 5f;
    public float defense = 1.5f;

}