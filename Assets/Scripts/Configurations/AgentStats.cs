using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentStats", menuName = "ScriptableObjects/AgentStats", order = 1)]
public class AgentStats : SelectableStats
{
    [Header("Movement")]
    [Range(0f, 20f)] public float maxSpeed = 5f;

    [Header("Behavior Distances")]
    [Range(0f, 10f)] public float neighborRadius = 1.25f;
    [Range(0f, 10f)] public float separationDistance = 2.42f;

    [Header("Behavior Weights")]
    [Range(0f, 10f)] public float separationWeight = 9.78f;
    [Range(0f, 10f)] public float cohesionWeight = 5.37f;
    [Range(0f, 10f)] public float alignmentWeight = 5.52f;

    [Header("Flowfield")]
    [Range(0.0f, 1f)] public float boidStrength = 0.381f;

    [Range(0f, 1f)] public float velocityInterpolation = 0.552f;

    [Header("Attack")]
    public AttackConfig attack;


    [Header("Abilities")]
    public List<string> abilities;




}
