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

    public static AgentControllerBoid GetClosestNeigborOnOtherTeam(List<AgentControllerBoid> neighbors, Vector3 targetPoint, int team){
        string p = "im on team: " + team;
        foreach (AgentControllerBoid neighbor in neighbors)
        {
            p += " neighbor team: " + neighbor.team+ "\n";
        }
        //Debug.Log(p);
        return neighbors
        .Where(neighbor => neighbor.team != team)
        .OrderBy(neighbor => Vector2.Distance(neighbor.transform.position, targetPoint))
        .FirstOrDefault();
    }

    public static bool CanSeeOther(Vector2 agent, Vector2 other){
        int obstacleLayer = LayerMask.GetMask("PathfindingWeights");
        Vector2 direction = other - agent;
        float distance = direction.magnitude;
        RaycastHit2D[] hits = Physics2D.RaycastAll(agent, direction, distance, obstacleLayer);

        //Debug.Log($"hits: {hits.Length}, pos: " + agent.transform.position + " direction: " + direction + " distance: " + distance);
        foreach (RaycastHit2D hit in hits)
        {
            //Debug.Log($"hit: {hit.collider.gameObject.name}");
            if (hit.collider != null && hit.collider.gameObject.tag == "Rock")
            {
                return false; // Obstacle in the way
            }
        }

        return true; // No obstacles
    }

}