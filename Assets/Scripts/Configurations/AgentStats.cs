using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentStats", menuName = "ScriptableObjects/AgentStats", order = 1)]
public class AgentStats : SelectableStats
{
    [Header("Movement")]
    [Range(0f, 20f)] public float maxSpeed = 5f;
    [Range(0f, 10f)] public float maxForce = 2f;

    [Header("Behavior Distances")]
    [Range(0f, 10f)] public float neighborRadius = 2f;
    [Range(0f, 10f)] public float separationDistance = 1.5f;

    [Header("Behavior Weights")]
    [Range(0f, 10f)] public float separationWeight = 1.5f;
    [Range(0f, 10f)] public float cohesionWeight = 1f;
    [Range(0f, 10f)] public float alignmentWeight = 1.2f;

    [Header("Flowfield")]
    [Range(0.0f, 1f)] public float boidStrength;

    [Range(0f, 1f)] public float velocityInterpolation = 0.1f;

    public AttackConfig attack;


    [Header("Abilities")]
    public List<string> abilities;




}
