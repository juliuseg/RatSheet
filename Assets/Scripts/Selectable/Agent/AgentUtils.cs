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
            
            if (neighbor != null && neighbor.movementManager.GetID() == movementManager.GetID())
            {
                return true;
            }
            return false;
        }).ToList();
    }

    public static Selectable GetClosestNeigborOnOtherTeam(List<Selectable> neighbors, Vector3 targetPoint, int team){
        string p = "im on team: " + team;
        foreach (Selectable neighbor in neighbors)
        {
            p += " neighbor team: " + neighbor.team+ "\n";
        }
        //Debug.Log(p);
        return neighbors
        .Where(neighbor => neighbor.team != team)
        .OrderBy(neighbor => Vector2.Distance(neighbor.transform.position, targetPoint))
        .FirstOrDefault();
    }

    public static Selectable GetClosestNeigborOnOtherTeamProritizeUnits(List<Selectable> neighbors, Vector3 targetPoint, int team){
        List<Selectable> ns = neighbors.Where(neighbor => neighbor is AgentControllerBoid && neighbor.team != team).Cast<Selectable>().ToList();
        if (ns.Count() == 0){
            //Debug.Log("no units so should return building");
            return GetClosestNeigborOnOtherTeam(neighbors, targetPoint, team);
        } else {
            //Debug.Log("should get some other team unit");
            return GetClosestNeigborOnOtherTeam(ns, targetPoint, team);
        }
    }

    public static bool CanSeeOther(Transform agent, Transform other){
        int obstacleLayer = LayerMask.GetMask("PathfindingWeights");
        Vector2 direction = other.position - agent.position;
        float distance = direction.magnitude;
        RaycastHit2D[] hits = Physics2D.RaycastAll(agent.position, direction, distance, obstacleLayer);

        //Debug.Log($"hits: {hits.Length}, pos: " + agent.transform.position + " direction: " + direction + " distance: " + distance);
        foreach (RaycastHit2D hit in hits)
        {
            //Debug.Log($"hit: {hit.collider.gameObject.name}");
            if (hit.collider != null && hit.collider.gameObject.tag == "Rock"
            && hit.transform != agent && hit.transform != other)
            {
                return false; // Obstacle in the way
            }
        }

        return true; // No obstacles
    }

    public static List<AgentControllerBoid> GetNeighborsAgents(List<Selectable> neighbors){ 
        return neighbors.Where(neighbor => neighbor is AgentControllerBoid).Cast<AgentControllerBoid>().ToList();
    }

}