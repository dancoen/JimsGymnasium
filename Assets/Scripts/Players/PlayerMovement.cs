using UnityEngine;
using System.Collections;
using System;
using UnityEngine.InputSystem;

[System.Serializable]
[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : MonoBehaviour
{
    public float jumpHeight = 4;
    public float moveSpeed = 4;

    public float gravity;
    public float jumpVelocity;

    //For Testing
    public bool Player2;

    [HideInInspector]
    public Vector3 velocity;

    public float velocityXSmoothing;

    private bool holdingJump;
    private float inputX;

    [HideInInspector]
    public Controller2D controller;
    float Xscale;

    private float targetVelocityX;

    void Awake()
    {

    }

    void Start()
    {
        controller = GetComponent<Controller2D>();
        Xscale = this.gameObject.transform.localScale.x;
    }

    void FixedUpdate()
    {
            if (controller.collisions.below)
            {
                targetVelocityX = inputX * moveSpeed;
            }

            if (controller.collisions.below)
            {
                velocity.y = 0;
            }

            if (holdingJump && controller.collisions.below )
            {
                velocity.y = jumpVelocity;
            }

        velocity.x = targetVelocityX;

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        holdingJump = false;

        /*
        if (inputX < 0)
        {
            this.gameObject.transform.localScale = new Vector2(-Xscale, transform.localScale.y);
        }
        else if(inputX > 0)
        {
            this.gameObject.transform.localScale = new Vector2(Xscale, transform.localScale.y);
        }
        */
    }
    private void Update()
    {
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
    }
}