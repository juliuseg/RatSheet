using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentNeighborCollisionHandler : MonoBehaviour
{
    public List<string> neighborString;
    private List<AgentControllerBoid> neighbors;

    public List<AgentControllerBoid> GetNeighbors()
    {
        neighbors = new List<AgentControllerBoid>();
        return neighbors;
    }

    private void Update() {
        neighborString = neighbors.Select(x => x.gameObject.name).ToList();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Add neighbor if it's another AgentController and not this agent
        if (other.TryGetComponent(out AgentControllerBoid neighbor) && neighbor != this && !neighbors.Contains(neighbor))
        {
            //Debug.Log("neighbor added: " + other.gameObject.name + " to " + gameObject.name);
            neighbors.Add(neighbor);
            neighbor.health.OnDeath += () => neighbors.Remove(neighbor);
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Remove the agent from neighbors when exiting the detection radius
        if (other.TryGetComponent(out AgentControllerBoid neighbor)  && other.isTrigger)
        {
            neighbors.Remove(neighbor);
            neighbor.health.OnDeath -= () => neighbors.Remove(neighbor);
            //Debug.Log ("neighbor removed: " + other.gameObject.GetInstanceID() + " from " + gameObject.GetInstanceID());
        }
    }
}