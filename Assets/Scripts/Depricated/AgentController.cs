// using UnityEngine;

// [RequireComponent(typeof(Rigidbody2D))]
// public class AgentController : MonoBehaviour
// {
//     public FlowFieldManager flowFieldManager; // Reference to the FlowFieldManager

//     public AgentSpawner agentSpawner; // Reference to the AgentSpawner
//     public float speed = 5f;                  // Speed of the agent

//     private Rigidbody2D rb;

//     public bool arrived = false;

//     private Vector3 oldTarget;

//     private void Awake()
//     {
//         rb = GetComponent<Rigidbody2D>();
//     }

//     private void FixedUpdate()
//     {
//         if (flowFieldManager == null) return;

//         // Get the flow field from the FlowFieldManager
//         Grid<Vector2> flowField = flowFieldManager.GetFlowField();

//         // Convert the agent's position to the flow field grid position
//         Vector2Int gridPos = flowField.GetGridPosition(transform.position);

//         // Get the flow direction from the flow field
//         Vector2 flowDirection = flowField.GetGridValue(gridPos.y+1, gridPos.x+1);

//         // Set the Rigidbody2D velocity based on the flow direction
//         rb.velocity = flowDirection * speed;

//         // Calculate the distance between the agent's position and the target point
//         Vector2 distanceToTarget = (Vector2)flowFieldManager.targetPoint - (Vector2)transform.position;
//         float distancemin = Mathf.Sqrt(agentSpawner.agentsArrived * 0.1f+0.1f);

//         print(distancemin);
//         if (distanceToTarget.magnitude < distancemin)
//         {
//             speed = 0;
//             arrived = true;
            
//         } else {
//             if (oldTarget != flowFieldManager.targetPoint)
//             {
//                 speed = 5f;
//                 arrived = false;
//             }
//         }
        

//         rb.mass = arrived ? 0.1f : 1f;

//         GetComponent<SpriteRenderer>().color = arrived ? Color.green : Color.white;

//         oldTarget = flowFieldManager.targetPoint;
//     }
// }