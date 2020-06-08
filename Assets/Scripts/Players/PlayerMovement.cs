using UnityEngine;
using System.Collections;
using System;

[System.Serializable]

[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : MonoBehaviour
{
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

    private float pushX = 0;

    [HideInInspector]
    public Controller2D controller;
    float Xscale;

    private float targetVelocityX;

    private Transform otherPlayer;

    void Awake()
    {

    }

    void Start()
    {
        controller = GetComponent<Controller2D>();
        Xscale = this.gameObject.transform.localScale.x;

        if(Player2)
        {
            otherPlayer = GameObject.Find("Player 1").GetComponent<Transform>();
        }
        else 
        {
            otherPlayer = GameObject.Find("Player 2").GetComponent<Transform>();
        }
    }

    void FixedUpdate()
    {
        //This is where we will setup the state machine in the future with a switch case statement based on the state.  We will check inputs in another class and then the inputs will switch states.  Each state has differnt movment abilities and require a different function that will be called from this FixedUpdate;

        if (controller.collisions.below)
        {
            
            targetVelocityX = inputX * moveSpeed;

            velocity.y = 0;

            if (holdingJump)
            {
                velocity.y = jumpVelocity;
            }
        }

        //this will deal with inputs along with input against another player walking into you.  PushX is set in the PushBox script;
        targetVelocityX += pushX;

        velocity.x = targetVelocityX;

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        holdingJump = false;

        checkToSwitchPositions();      
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

    public void setPushX(float x)
    {
        pushX = x;
    }

    public float getPushX()
    {
        return pushX;
    }

    public float getInputX()
    {
        return inputX;
    }

    public float getMovementSpeed()
    {
        return moveSpeed;
    }

    void checkToSwitchPositions()
    {
        if (controller.collisions.below) //Change Later to switch sides only when in the standing state
        {

            //Assuming player1 is on the left.  Turning around on position switch.
            if (Player2)
            {
                if (otherPlayer.position.x > transform.position.x)
                {
                    this.gameObject.transform.localScale = new Vector2(-Xscale, transform.localScale.y);
                }
                else
                {
                    this.gameObject.transform.localScale = new Vector2(Xscale, transform.localScale.y);
                }
            }

            else
            {
                if (otherPlayer.position.x < transform.position.x)
                {
                    this.gameObject.transform.localScale = new Vector2(-Xscale, transform.localScale.y);
                }
                else
                {
                    this.gameObject.transform.localScale = new Vector2(Xscale, transform.localScale.y);
                }
            }
        }
    }

    public bool isGrounded()
    {
        return controller.collisions.below;
    }
}