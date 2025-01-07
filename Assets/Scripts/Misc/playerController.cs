using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    private Rigidbody2D rb;
    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // controll with wasd. set rb velocity to move the player
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            moveVertical += 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveVertical += -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal += -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal += 1f;
        }

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rb.velocity = movement * speed;
    }
}
