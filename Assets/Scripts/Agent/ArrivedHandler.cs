// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// public class ArrivedHandler : MonoBehaviour{

//     public MovementManager movementManager;  

//     public Rigidbody2D rb;   

//     private bool arrived;
//     private bool arrivedCorrection;

//     private Coroutine arrivedCoroutine;
//     private Coroutine arrivedCorrectionCoroutine;

//     private Coroutine arrivedSetCoroutine;

//     private float arrivalDistance;

//     private bool newFlowfield;

//     private List<AgentControllerBoid> neighbors = new List<AgentControllerBoid>(); // List of nearby agents
//     private List<AgentControllerBoid> neighborsWhenStopped = new List<AgentControllerBoid>(); // List of nearby agents


    

//     public void Setup(MovementManager _movementManager, Rigidbody2D _rb, List<AgentControllerBoid> _neighbors){
//         arrived = false;
//         arrivedCorrection = false;
//         newFlowfield = false;
//         movementManager = _movementManager;
//         rb = _rb;
//         neighbors = _neighbors;

//     }

//     public void TriggerNewFlowfield(){
//         newFlowfield = true;
//     }
    

//     public bool GetArrived(){
//         return arrived;
//     }

//     public bool GetArrivedCorrection(){
//         return arrivedCorrection;
//     }

//     public bool UpdateArrivalStatus()
//     {
//         print("neighbors: " + neighbors.Count);
//         // Calculate the distance to the target point
//         float distanceToTarget = AgentUtils.GetClosestTargetDistance(transform.position, movementManager.flowFieldManager.targetPoint);

//         float arrival_distance = GetArrivalDistance();

//         // Check if the agent is within the arrival threshold
//         if (newFlowfield)
//         {
//             newFlowfield = false;

//             if (arrivedCoroutine != null){
//                 StopCoroutine(arrivedCoroutine);
//                 arrivedCoroutine = null;
//             }
//             if(arrivedCorrectionCoroutine != null){
//                 StopCoroutine(arrivedCorrectionCoroutine);
//                 arrivedCorrectionCoroutine = null;
//                 arrivedCorrection = false;
//             }
//             if (arrivedSetCoroutine != null){
//                 StopCoroutine(arrivedSetCoroutine);
//                 arrivedSetCoroutine = null;
//             }
            
//             if (arrived) {
//                 SetArrived(false);
//             } 
//             return false;
//         } 
//         else if (distanceToTarget < arrival_distance)
//         {
//             if (!arrived && arrivedSetCoroutine == null) {
//                 SetArrived(true);
//             }
//         }
//         return true;

//     }


//     public void CheckNeighborsArrival(){
//         if (AgentUtils.GetNeighborsInGroup(neighbors, movementManager).Count == 0){
//             return;
//         }
//         foreach (AgentControllerBoid neighbor in AgentUtils.GetNeighborsInGroup(neighbors, movementManager).Where(neighbor => Vector2.Distance(neighbor.transform.position, transform.position) < 1.0f)){
//             if (!arrived && neighbor.arrivedHandler.GetArrived() && arrivedCoroutine == null && arrivedSetCoroutine == null){
//                 arrivedCoroutine = StartCoroutine(SetArrivedAfterSeconds());
//                 //print("Arrived! from neighbors");
//                 break;

//             }
//         }
//         if (arrived){
//             if (neighborsWhenStopped.Count == 0) {SetArrived(false); return;}
//             foreach (AgentControllerBoid neighbor in neighborsWhenStopped){

//                 float distanceToTarget = AgentUtils.GetClosestTargetDistance(transform.position, movementManager.flowFieldManager.targetPoint);

//                 if ((Vector2.Distance(neighbor.transform.position, transform.position) > 1.3f 
//                 || distanceToTarget > (GetArrivalDistance()+0.2f))
//                  && arrivedCorrectionCoroutine == null){
//                     if (distanceToTarget > (GetArrivalDistance()+0.2f)){
//                         //print ("Arrived correction: dist to target: " + distanceToTarget + " arrivalDist: " + (GetArrivalDistance()+0.5f));
//                         arrivedCorrectionCoroutine = StartCoroutine(SetArrivedCorrectionAfterSeconds(true));
//                     }
//                     if(Vector2.Distance(neighbor.transform.position, transform.position) > 1.3f){
//                         //print ("Arrived correction: dist to neighbor is greater than 1.3f: " + Vector2.Distance(neighbor.transform.position, transform.position));
//                         arrivedCorrectionCoroutine = StartCoroutine(SetArrivedCorrectionAfterSeconds(false));
//                     }
//                 }
//             }
//         }
//     }


//     private IEnumerator SetArrivedAfterSeconds()
//     {

//         yield return new WaitForSeconds(0.4f);

//         SetArrived(true);
//         arrivedCoroutine = null;

//     }

//     private IEnumerator SetArrivedCorrectionAfterSeconds(bool wasTargetDist)
//     {
//         arrivedCorrection = false;
//         yield return new WaitForSeconds(1.0f);

//         float distanceToTarget = AgentUtils.GetClosestTargetDistance(transform.position, movementManager.flowFieldManager.targetPoint);
//         if (wasTargetDist && distanceToTarget > (GetArrivalDistance()+0.2f))
//         {
//             arrivedCorrection = true;

//             while(AgentUtils.GetNeighborsInGroup(neighbors, movementManager).Where(neighbor => Vector2.Distance(neighbor.transform.position, transform.position) < 0.8f).ToList().Count == 0 
//             ||(AgentUtils.GetNeighborsInGroup(neighbors, movementManager).Where(neighbor => Vector2.Distance(neighbor.transform.position, transform.position) < 0.8f).ToList().Count <= 2 && distanceToTarget > (GetArrivalDistance()+0.3f))){
//                 //print("Waiting more: " + distanceToTarget + " target pos " + targetPos + " go pos: " + transform.position);
//                 yield return new WaitForSeconds(0.3f);
                
//                 distanceToTarget = AgentUtils.GetClosestTargetDistance(transform.position, movementManager.flowFieldManager.targetPoint);
//             }

//             yield return new WaitForSeconds(0.5f);
//         }
//         arrivedCorrection = false;
//         arrivalDistance = GetArrivalDistance();

//         SetneighborsWhenStopped();
//         arrivedCorrectionCoroutine = null;

//     }

//     private void SetneighborsWhenStopped (){
//         if (AgentUtils.GetNeighborsInGroup(neighbors, movementManager).Count == 0){
//             arrivedCorrection = false;
//             SetArrived(false);
//             return;
//         }
//         neighborsWhenStopped = AgentUtils.GetNeighborsInGroup(neighbors, movementManager).Where(neighbor => Vector2.Distance(neighbor.transform.position, transform.position) < 1.0f).ToList();
//         if (neighborsWhenStopped.Count == 0){
//             neighborsWhenStopped = AgentUtils.GetNeighborsInGroup(neighbors, movementManager);
//         }
//     }

//     float GetArrivalDistance(){
//         if (arrived){
//             return arrivalDistance+0.2f;
//         }
//         return Mathf.Sqrt(movementManager.GetAgentArrived() * 0.1f + 0.1f);
//     }


//     private void SetArrived(bool hasArrived)
//     {
//         print("Arrived!");
//         if (hasArrived && arrivedCoroutine != null){
//             StopCoroutine(arrivedCoroutine);
//             arrivedCoroutine = null;
//         }
//         if (hasArrived == arrived) return;

//         if (hasArrived){
//             SetneighborsWhenStopped();
//         }

//         rb.mass = hasArrived ? 0.05f : 1f;  // Adjust mass for smoother arrival stopping

//         if (!hasArrived)
//         {
//             arrived = false;
//         } else if (arrivedSetCoroutine == null) {
//             arrivedSetCoroutine = StartCoroutine(ArrivedTrue());
//         }

//     }

//     IEnumerator ArrivedTrue(){
//         yield return new WaitForSeconds(0.2f); 
//         arrivalDistance = GetArrivalDistance();
//         arrived = true;
//         arrivedSetCoroutine = null;

        

//     }


// }
