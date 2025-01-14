using UnityEngine;
using System.Collections.Generic;

public class AgentVelocity
{
    private Rigidbody2D rb;
    private Transform transform;
    private AgentStats agentStats;

    private MovementManager movementManager;
    private List<AgentControllerBoid> neighbors;

        
    public AgentVelocity(Rigidbody2D _rb, Transform _transform, AgentStats _agentStats, List<AgentControllerBoid> _neighbors)
    {
        rb = _rb;
        transform = _transform;
        agentStats = _agentStats;
        neighbors = _neighbors;
    }

    public void SetMovementManager(MovementManager _movementManager)
    {
        movementManager = _movementManager;
    }

    public Vector2 GetVelocityFromFlowField(bool arrived, bool arrivedCorrection)
    {
        Vector2 velocity = Vector2.zero;
        Vector2 flowFieldForce = FlowFieldHandler.CalculateFlowFieldForce(transform.position, movementManager).normalized;
        Vector2 boidForce = BoidBehavior.CalculateBoidBehaviors(AgentUtils.GetNeighborsInGroup(neighbors, movementManager), arrived, transform.position, agentStats).normalized;

        if (!arrived || arrivedCorrection){

            // Calculate dot product to check if boidForce is within 90 degrees of flowFieldForce
            float dotProduct = Vector2.Dot(flowFieldForce, boidForce);

            // Only add boidForce if the dot product is positive (indicating an angle < 90 degrees)
            Vector2 combinedForce = flowFieldForce;
            if (dotProduct > 0)
            {
                combinedForce += boidForce * agentStats.boidStrength;
            }

            velocity = combinedForce.normalized;
            
        } 
        return velocity;
    }

    public Vector2 SetVelocity(Vector2 targetVelocity){
        //Debug.Log("SetVelocity: " + targetVelocity);
        // Interpolate between the current velocity and the new calculated velocity
        targetVelocity = targetVelocity.normalized * agentStats.maxSpeed;
        Vector2 velocity = Vector2.Lerp(rb.velocity, targetVelocity, agentStats.velocityInterpolation);
        transform.position += (Vector3)velocity * Time.fixedDeltaTime*0.2f;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y*0.001f);
        rb.velocity = velocity;
        return targetVelocity;
    }

    public Vector2 GetVelocityToEnemy(Selectable enemy){
        return (enemy.transform.position - transform.position).normalized;
        
    }

    public bool IsInAttackRange (Selectable enemy){
        Vector2 direction = enemy.transform.position - transform.position;
        return direction.magnitude < agentStats.attack.attackRange;
    }
}