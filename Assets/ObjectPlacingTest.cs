using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacingTest : MonoBehaviour
{
    private PathFindingController pfCont;

    [SerializeField] private GameObject StonePrefab;

    [SerializeField] private GameObject WorldObstacles;

    // Start is called before the first frame update
    void Start()
    {
        pfCont = GameObject.Find("PathFindingController").GetComponent<PathFindingController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)){
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            
            GameObject stone = Instantiate(StonePrefab, WorldObstacles.transform);
            stone.transform.position = mousePos;
            stone.transform.rotation = Quaternion.identity;

            // Force physics to update
            Physics2D.SyncTransforms();

            pfCont.UpdateGrids();
        }
    }
}

