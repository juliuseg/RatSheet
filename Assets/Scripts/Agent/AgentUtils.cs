using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class AgentUtils
{
    public static float GetClosestTargetDistance(Vector3 currentPosition, List<Vector3> targetPoints)
    {
        if (targetPoints == null || targetPoints.Count == 0)
        {
            return float.MaxValue; // No valid targets
        }

        float closestDistance = float.MaxValue;

        foreach (Vector3 target in targetPoints)
        {
            float distance = Vector2.Distance(currentPosition, target);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }


        return closestDistance;
    }

    public static List<AgentControllerBoid> GetNeighborsInGroup(List<AgentControllerBoid> neighbors, MovementManager movementManager){
        return neighbors.Where(neighbor => 
        {
            //Debug.Log($"id: {movementManager.GetID()} neighbor id: {neighbor.movementManager.GetID()}");

            if (neighbor.movementManager.GetID() == movementManager.GetID())
            {
                return true;
            }
            return false;
        }).ToList();
    }

}