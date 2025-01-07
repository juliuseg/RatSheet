using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class AgentControllerBoid : MonoBehaviour
{
    public AgentStats agentStats;               // Reference to BoidStats ScriptableObject
    //public FlowFieldManager flowFieldManager; // Reference to the FlowFieldManager
    public MovementManager movementManager;     // Reference to the MovementManager
    public AgentSpawnerBoid agentSpawner;         // Reference to the AgentSpawner

    private Rigidbody2D rb;
    private CircleCollider2D col;
    private List<AgentControllerBoid> neighbors = new List<AgentControllerBoid>(); // List of nearby agents
    private List<AgentControllerBoid> neighborsWhenStopped = new List<AgentControllerBoid>(); // List of nearby agents

    [HideInInspector] public bool arrived = false;
    [HideInInspector] public bool arrivedCorrection = false;

    private Coroutine arrivedCoroutine = null;
    private Coroutine arrivedCorrectionCoroutine;


    private Vector2 velocity;

    private Coroutine arrivedSetCoroutine;

    private bool newFlowfield = false;

    private float arrivalDistance;

    private AgentAppearance agentAppearance;

    [SerializeField] private GameObject selectionCircle;

    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SetupNeighborDetection();

    }

    private void SetupNeighborDetection()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        SetRadius();
    }
    
    private void SetRadius(){
        if (arrived){
            //col.radius = 1.0f;
        } else if (col.radius != agentStats.neighborRadius){
            col.radius = agentStats.neighborRadius;
        }
        
    }

    public void SetMovementManager(MovementManager _movementManager)
    {
        if (movementManager != null)
        {
            movementManager.RemoveAgent(this);
        }
        movementManager = _movementManager;

        newFlowfield = true;

        agentAppearance = new AgentAppearance(selectionCircle, GetComponent<SpriteRenderer>());
    }

    

    private void FixedUpdate()
    {
        if (movementManager.flowFieldManager == null || agentStats == null) return;

        bool checkN = UpdateArrivalStatus();
        SetVelocity();

        if (checkN) CheckNeighborsArrival();

        agentAppearance.AdjustAgentAppearance(arrived, arrivedCorrection, movementManager,GetNeighborsInGroup().Count);

        SetRadius();


    }

    public void SetSelectionCircleActive(int mode) // mode 0 = off, mode 1 = highlighted in red, mode 2 = selected in green
    {
        agentAppearance.SetSelectionCircleActive(mode);
    }

    private void CheckNeighborsArrival(){
        if (GetNeighborsInGroup().Count == 0){
            return;
        }
        foreach (AgentControllerBoid neighbor in GetNeighborsInGroup().Where(neighbor => Vector2.Distance(neighbor.transform.position, transform.position) < 1.0f)){
            if (!arrived && neighbor.arrived && arrivedCoroutine == null && arrivedSetCoroutine == null){
                arrivedCoroutine = StartCoroutine(SetArrivedAfterSeconds());
                //print("Arrived! from neighbors");
                break;

            }
        }
        if (arrived){
            if (neighborsWhenStopped.Count == 0) {SetArrived(false); return;}
            foreach (AgentControllerBoid neighbor in neighborsWhenStopped){

                float distanceToTarget = GetClosestTargetDistance(transform.position, movementManager.flowFieldManager.targetPoint);

                if ((Vector2.Distance(neighbor.transform.position, transform.position) > 1.3f 
                || distanceToTarget > (GetArrivalDistance()+0.2f))
                 && arrivedCorrectionCoroutine == null){
                    if (distanceToTarget > (GetArrivalDistance()+0.2f)){
                        //print ("Arrived correction: dist to target: " + distanceToTarget + " arrivalDist: " + (GetArrivalDistance()+0.5f));
                        arrivedCorrectionCoroutine = StartCoroutine(SetArrivedCorrectionAfterSeconds(true));
                    }
                    if(Vector2.Distance(neighbor.transform.position, transform.position) > 1.3f){
                        //print ("Arrived correction: dist to neighbor is greater than 1.3f: " + Vector2.Distance(neighbor.transform.position, transform.position));
                        arrivedCorrectionCoroutine = StartCoroutine(SetArrivedCorrectionAfterSeconds(false));
                    }
                    
                }
            }
        }

    }


    private IEnumerator SetArrivedAfterSeconds()
    {

        yield return new WaitForSeconds(0.4f);

        SetArrived(true);
        arrivedCoroutine = null;

    }

    private IEnumerator SetArrivedCorrectionAfterSeconds(bool wasTargetDist)
    {
        arrivedCorrection = false;
        yield return new WaitForSeconds(1.0f);

        float distanceToTarget = GetClosestTargetDistance(transform.position, movementManager.flowFieldManager.targetPoint);
        if (wasTargetDist && distanceToTarget > (GetArrivalDistance()+0.2f))
        {
            arrivedCorrection = true;

            while(GetNeighborsInGroup().Where(neighbor => Vector2.Distance(neighbor.transform.position, transform.position) < 0.8f).ToList().Count == 0 
            ||(GetNeighborsInGroup().Where(neighbor => Vector2.Distance(neighbor.transform.position, transform.position) < 0.8f).ToList().Count <= 2 && distanceToTarget > (GetArrivalDistance()+0.3f))){
                //print("Waiting more: " + distanceToTarget + " target pos " + targetPos + " go pos: " + transform.position);
                yield return new WaitForSeconds(0.3f);
                
                distanceToTarget = GetClosestTargetDistance(transform.position, movementManager.flowFieldManager.targetPoint);
            }

            yield return new WaitForSeconds(0.5f);
        }
        arrivedCorrection = false;
        arrivalDistance = GetArrivalDistance();

        SetneighborsWhenStopped();
        arrivedCorrectionCoroutine = null;

    }

    private void SetneighborsWhenStopped (){
        if (GetNeighborsInGroup().Count == 0){
            arrivedCorrection = false;
            SetArrived(false);
            return;
        }
        neighborsWhenStopped = GetNeighborsInGroup().Where(neighbor => Vector2.Distance(neighbor.transform.position, transform.position) < 1.0f).ToList();
        if (neighborsWhenStopped.Count == 0){
            neighborsWhenStopped = GetNeighborsInGroup();
        }
    }

    List<AgentControllerBoid> GetNeighborsInGroup(){
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



    private void SetVelocity()
    {

        Vector2 flowFieldForce = FlowFieldHandler.CalculateFlowFieldForce(transform.position, movementManager).normalized;
        Vector2 boidForce = BoidBehavior.CalculateBoidBehaviors(GetNeighborsInGroup(), arrived, transform.position, agentStats).normalized;

        if (!arrived || arrivedCorrection){

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

    

    private bool UpdateArrivalStatus()
    {
        print ("neighbors: " + neighbors.Count);
        // Calculate the distance to the target point
        float distanceToTarget = GetClosestTargetDistance(transform.position, movementManager.flowFieldManager.targetPoint);

        float arrival_distance = GetArrivalDistance();

        // Check if the agent is within the arrival threshold
        if (newFlowfield)
        {
            newFlowfield = false;

            if (arrivedCoroutine != null){
                StopCoroutine(arrivedCoroutine);
                arrivedCoroutine = null;
            }
            if(arrivedCorrectionCoroutine != null){
                StopCoroutine(arrivedCorrectionCoroutine);
                arrivedCorrectionCoroutine = null;
                arrivedCorrection = false;
            }
            if (arrivedSetCoroutine != null){
                StopCoroutine(arrivedSetCoroutine);
                arrivedSetCoroutine = null;
            }
            
            if (arrived) {
                SetArrived(false);
            } 
            return false;
        } 
        else if (distanceToTarget < arrival_distance)
        {
            if (!arrived && arrivedSetCoroutine == null) {
                SetArrived(true);
            }
        }
        return true;

    }

    float GetArrivalDistance(){
        if (arrived){
            return arrivalDistance+0.2f;
        }
        return Mathf.Sqrt(movementManager.GetAgentArrived() * 0.1f + 0.1f);
    }


    private void SetArrived(bool hasArrived)
    {

        if (hasArrived && arrivedCoroutine != null){
            StopCoroutine(arrivedCoroutine);
            arrivedCoroutine = null;
        }
        if (hasArrived == arrived) return;

        if (hasArrived){
            SetneighborsWhenStopped();
        }

        rb.mass = hasArrived ? 0.05f : 1f;  // Adjust mass for smoother arrival stopping

        if (!hasArrived)
        {
            arrived = false;
        } else if (arrivedSetCoroutine == null) {
            arrivedSetCoroutine = StartCoroutine(ArrivedTrue());
        }

    }

    IEnumerator ArrivedTrue(){
        yield return new WaitForSeconds(0.2f); 
        arrivalDistance = GetArrivalDistance();
        arrived = true;
        arrivedSetCoroutine = null;

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
        if (other.TryGetComponent(out AgentControllerBoid neighbor))
        {
            neighbors.Remove(neighbor);
        }
    }
}
