using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    public static Player instance;

    public float jumpHeight = 4;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .18f;
    public float moveSpeed = 4;

    [Range (0,15)]
    public int coyoteTimeFrameLimit = 3;
    private int coyoteTimeCurrentFrame = 0;
    private int IdleAnimCounter = 0;
    public int SpecialIdleAnimTimer;

    public float gravity;
    public float jumpVelocity;
    public float minJumpVelocity;

    [HideInInspector]
    public Vector3 velocity;

    public float velocityXSmoothing;

    private bool holdingJump;
    private float inputX;
    private bool alreadyJumped = true;
    private bool inCustcene = false;

    [HideInInspector]
    public Controller2D controller;
    Animator animate;
    float Xscale;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Player in the scene.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        //DontDestroyOnLoad(this.gameObject);
    }


    void Start()
    {
        controller = GetComponent<Controller2D>();
        animate = GetComponent<Animator>();
        Xscale = this.gameObject.transform.localScale.x;
    }

    void FixedUpdate()
    {
        //animate.ResetTrigger("Airborne");
        float targetVelocityX = inputX * moveSpeed;


            if (velocity.x == 0 && controller.collisions.below)
            {
                IdleAnimCounter++;
            }
            else
            {
                IdleAnimCounter = 0;
            }

            if (controller.collisions.above)
            {
                velocity.y = 0;
            }
            if (controller.collisions.below)
            {
                velocity.y = 0;
                coyoteTimeCurrentFrame = 0;
                alreadyJumped = false;
            }

            if (holdingJump && (controller.collisions.below || (coyoteTimeCurrentFrame < coyoteTimeFrameLimit && alreadyJumped == false)))
            {
                velocity.y = jumpVelocity;
                alreadyJumped = true;
            }

            if (!holdingJump && !controller.collisions.below && velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }

            coyoteTimeCurrentFrame++;

            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);


        if (inputX < 0)
        {
            this.gameObject.transform.localScale = new Vector2(-Xscale, transform.localScale.y);
        }
        else if(inputX > 0)
        {
            this.gameObject.transform.localScale = new Vector2(Xscale, transform.localScale.y);
        }
    }
    private void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        holdingJump = Input.GetKey("w") || Input.GetKey(KeyCode.Space);
    }


    public void setPlayerSpeed(int speed)
    {
        inputX = speed;
    }

    public void setPlayerInputLock(bool Lock) //true -> disable input
    {
        inCustcene = Lock;
    }
}