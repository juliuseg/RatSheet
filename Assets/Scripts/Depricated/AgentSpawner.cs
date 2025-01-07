// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// public class AgentSpawner : MonoBehaviour
// {
//     public FlowFieldManager flowFieldManager;
//     public GameObject agentPrefab;

//     public int agentsArrived;

//     public List<AgentController> agents;

//     void Start()
//     {
//         agents = new List<AgentController>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // Spawn on space key press
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             GameObject pf = Instantiate(agentPrefab, transform.position, Quaternion.identity);
//             pf.GetComponent<AgentController>().flowFieldManager = flowFieldManager;
//             pf.GetComponent<AgentController>().agentSpawner = this;

//             agents.Add(pf.GetComponent<AgentController>());
            
//         }

//         agentsArrived = agents.Where(agent => agent.arrived).Count();
//         print(agentsArrived);
//     }
// }
