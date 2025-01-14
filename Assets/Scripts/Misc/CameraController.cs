using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float zoomSpeed = 20f;

    

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    private void ZoomCamera(){
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Camera.main.orthographicSize -= scroll * zoomSpeed * Time.deltaTime;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 1, 15);
    }

    void MoveCamera(){
        float camSize = Camera.main.orthographicSize;

        Vector2 moveDir = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDir.y += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDir.y -= 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir.x -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDir.x += 1;
        }

        moveDir.Normalize();

        transform.Translate(moveDir * panSpeed * (float)Math.Sqrt(camSize) * Time.deltaTime);
    }
}
