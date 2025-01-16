using UnityEngine;
using System.Collections.Generic;


public static class BoidBehavior {

    
    

    public static Vector2 CalculateBoidBehaviors(List<AgentMoveable> ns, bool arrived, Vector3 position, AgentStats agentStats)
    {
        Vector2 separation = CalculateSeparation(ns, position, agentStats) * agentStats.separationWeight;
        Vector2 cohesion = CalculateCohesion(ns, position) * agentStats.cohesionWeight;
        Vector2 alignment = arrived ? Vector2.zero : CalculateAlignment(ns) * agentStats.alignmentWeight;

        return separation + cohesion + alignment;
    }

    private static Vector2 CalculateSeparation(List<AgentMoveable> ns, Vector3 position, AgentStats agentStats)
    {
        Vector2 separationForce = Vector2.zero;
        int count = 0;

        foreach (AgentMoveable neighbor in ns)
        {
            float distance = Vector2.Distance(position, neighbor.transform.position);
            if (distance < agentStats.separationDistance && distance > 0)
            {
                Vector2 diff = (Vector2)(position - neighbor.transform.position);
                separationForce += diff.normalized / distance; // Stronger force for closer neighbors
                count++;
            }
        }

        if (count > 0)
        {
            separationForce /= count;
        }

        return separationForce;
    }

    private static Vector2 CalculateCohesion(List<AgentMoveable> ns, Vector3 position)
    {
        Vector2 cohesionCenter = Vector2.zero;
        int count = 0;

        foreach (AgentMoveable neighbor in ns)
        {
            cohesionCenter += (Vector2)neighbor.transform.position;
            count++;
        }

        if (count > 0)
        {
            cohesionCenter /= count;
            Vector2 cohesionDirection = cohesionCenter - (Vector2)position;
            return cohesionDirection.normalized; // Move towards the center of neighbors
        }

        return Vector2.zero;
    }

    private static Vector2 CalculateAlignment(List<AgentMoveable> ns)
    {
        Vector2 alignmentForce = Vector2.zero;
        int count = 0;

        foreach (AgentMoveable neighbor in ns)
        {
            alignmentForce += (Vector2)neighbor.transform.up;
            count++;
        }

        if (count > 0)
        {
            alignmentForce /= count;
            return alignmentForce.normalized;
        }

        return Vector2.zero;
    }
}