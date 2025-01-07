using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class AgentControllerBoid : MonoBehaviour
{
    public AgentStats agentStats;               // Reference to BoidStats ScriptableObject
    public MovementManager movementManager;     // Reference to the MovementManager
    private Rigidbody2D rb;
    private CircleCollider2D col;
    private List<AgentControllerBoid> neighbors = new List<AgentControllerBoid>(); // List of nearby agents
    public ArrivedHandler arrivedHandler;


    private Vector2 velocity;


    private AgentAppearance agentAppearance;

    [SerializeField] private GameObject selectionCircle;

    


    public void SetMovementManager(MovementManager _movementManager)
    {
        if (movementManager != null)
        {
            movementManager.RemoveAgent(this);
        }
        movementManager = _movementManager;


        rb = GetComponent<Rigidbody2D>();
        if (arrivedHandler == null){
            arrivedHandler = gameObject.AddComponent<ArrivedHandler>();
        }
        arrivedHandler.Setup(movementManager, rb, neighbors);

        SetupNeighborDetection();

        arrivedHandler.TriggerNewFlowfield();

        agentAppearance = new AgentAppearance(selectionCircle, GetComponent<SpriteRenderer>());
    }

    

    private void SetupNeighborDetection()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        SetRadius();
    }
    
    private void SetRadius(){
        if (arrivedHandler.GetArrived()){
            //col.radius = 1.0f;
        } else if (col.radius != agentStats.neighborRadius){
            col.radius = agentStats.neighborRadius;
        }
        
    }

    

    private void FixedUpdate()
    {
        if (movementManager.flowFieldManager == null || agentStats == null) return;

        bool checkN = arrivedHandler.UpdateArrivalStatus();
        SetVelocity();

        if (checkN) arrivedHandler.CheckNeighborsArrival();

        agentAppearance.AdjustAgentAppearance(arrivedHandler.GetArrived(), arrivedHandler.GetArrivedCorrection(), movementManager, AgentUtils.GetNeighborsInGroup(neighbors, movementManager).Count);

        SetRadius();


    }


    

    private void SetVelocity()
    {

        Vector2 flowFieldForce = FlowFieldHandler.CalculateFlowFieldForce(transform.position, movementManager).normalized;
        Vector2 boidForce = BoidBehavior.CalculateBoidBehaviors(AgentUtils.GetNeighborsInGroup(neighbors, movementManager), arrivedHandler.GetArrived(), transform.position, agentStats).normalized;

        if (!arrivedHandler.GetArrived() || arrivedHandler.GetArrivedCorrection()){

            // Calculate dot product to check if boidForce is within 90 degrees of flowFieldForce
            float dotProduct = Vector2.Dot(flowFieldForce, boidForce);

            // Only add boidForce if the dot product is positive (indicating an angle < 90 degrees)
            Vector2 combinedForce = flowFieldForce;
            if (dotProduct > 0)
            {
                combinedForce += boidForce * agentStats.boidStrength;
            }


            // Interpolate between the current velocity and the new calculated velocity
            Vector2 targetVelocity = combinedForce.normalized * agentStats.maxSpeed;
            velocity = Vector2.Lerp(rb.velocity, targetVelocity, agentStats.velocityInterpolation);
            transform.position += (Vector3)velocity * Time.fixedDeltaTime*0.2f;
            rb.velocity = velocity;
        } else {
            rb.velocity = Vector2.zero;
        }
    }




    public void SetSelectionCircleActive(int mode) // mode 0 = off, mode 1 = highlighted in red, mode 2 = selected in green
    {
        agentAppearance.SetSelectionCircleActive(mode);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Add neighbor if it's another AgentController and not this agent
        if (other.TryGetComponent(out AgentControllerBoid neighbor) && neighbor != this && !neighbors.Contains(neighbor))
        {
            neighbors.Add(neighbor);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Remove the agent from neighbors when exiting the detection radius
        if (other.TryGetComponent(out AgentControllerBoid neighbor)  && other.isTrigger)
        {
            neighbors.Remove(neighbor);
            //print ("neighbor removed: " + other.gameObject.GetInstanceID() + " from " + gameObject.GetInstanceID());
        }
    }
}
