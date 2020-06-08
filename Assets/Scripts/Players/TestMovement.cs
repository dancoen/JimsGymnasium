using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    public float walkSpeed = 5;
    public float jumpHeight = 3;

    public float gravity = 3;

    private Rigidbody2D rb;


    private float distToGround;
    public bool Player2;

    private bool holdingJump;
    private float inputX;

    private float yVelocity;
    private float xVelocity;

    private bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        if(rb == null)
        {
            Debug.LogError("This player does not have a rigidbody");
        }

        distToGround = transform.GetComponent<PolygonCollider2D>().bounds.extents.y;

        if (distToGround == null)
        {
            Debug.LogError("This player does not have a rigidbody");
        }
    }

    public void FixedUpdate()
    {

            xVelocity = inputX * walkSpeed;

            if(holdingJump)
            {
                yVelocity = jumpHeight;
            }

        if (yVelocity > 0)
        {
            yVelocity -= gravity * Time.deltaTime;
        }

        Vector3 movementVector = new Vector3(xVelocity, yVelocity, 0f);

        rb.MovePosition(rb.transform.position + movementVector * Time.deltaTime);

        holdingJump = false;
    }

    private void Update()
    {
        Debug.Log(Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f));


        inputX = 0;
        if (Player2)
        {
            inputX += Input.GetKey(KeyCode.RightArrow) ? 1f : 0f;
            inputX -= Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f;
        }
        else
        {
            inputX += Input.GetKey("d") ? 1f : 0f;
            inputX -= Input.GetKey("a") ? 1f : 0f;
        }

        //Checks every frame on update between frames of FixedUpdate to see if jump key was pressed and then resets holdingJump after FixedUpdate
        //Essentially just a crappy input buffer for frame differences

        //This will need to be updated later on to implement different jump lengths based on character and moving forward/backward
        if (!holdingJump)
        {
            if (Player2)
            {
                holdingJump = Input.GetKey(KeyCode.UpArrow);
            }
            else
            {
                holdingJump = Input.GetKey("w");
            }
        }

        grounded = Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f);
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
    }
}
