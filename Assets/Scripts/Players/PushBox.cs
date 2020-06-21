using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBox : MonoBehaviour
{
    private PlayerMovement thisPlayer, otherPlayer;
    private BoxCollider2D leftWall, rightWall;

    private float leftWallX, rightWallX;

    private float skinWidth = .02f;

    void Start()
    {
        thisPlayer = transform.parent.gameObject.GetComponent<PlayerMovement>();
        leftWall = GameObject.Find("Left Wall").GetComponent<BoxCollider2D>();
        rightWall = GameObject.Find("Right Wall").GetComponent<BoxCollider2D>();

        if(!leftWall || !rightWall)
        {
            Debug.LogError("The Walls must have a set name for collisions to work, also because im lazy");
        }
        else
        {
            leftWallX = leftWall.transform.position.x + leftWall.size.x / 2;
            rightWallX = rightWall.transform.position.x - rightWall.size.x / 2;

            Debug.Log("Left Wall X = " + leftWallX + "     Right Wall X = " + rightWallX);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        CollisionHandler(col);
        AirCollisionHandler(col);
    }
    void OnTriggerStay2D(Collider2D col)
    {
        CollisionHandler(col);
        AirCollisionHandler(col);
    }

    void CollisionHandler(Collider2D col)
    {
        col = col as BoxCollider2D;
        otherPlayer = col.transform.parent.gameObject.GetComponent<PlayerMovement>();

        /*ALL CHECKS:   (IN AIR MEANS EITHER RISING OR FALLING)
            case 1: PLAYER 1 GROUND PLAYER 2 GROUND AND BOTH PUSHING INTO EACH OTHER = EQUAL PUSH BASED ON SPEEDS
            case 2: PLAYER 1 GROUND PLAYER 2 GROUND ONLY 1 MOVING = MOVING PLAYERS NORMAL SPEED / 2?????
            case 3: PLAYER 1 GROUND PLAYER 2 GROUND 1 WALKING INTO THE OTHER WHILE THE OTHER BACKS UP (BACKING UP PLAYER MUST BE SLOWER THAN WALKING FORWARD PLAYER) = SETTING SLOW CHARACTERS BACKUP SPEED TO FAST CHARACTERS WALKING SPEED
            case 4: PLAYER 1 GROUND PLAYER 2 ONE PUSHING INTO THE OTHER WHILE OTHER IS AGAINST WALL = 0 NET MOVEMENT UNLESS WALL CHARACTER IS PUSHING INTO OTHER CHARACTER AND FASTER THAN THEM
            case 5: PLAYER 1 RISING PLAYER 2 GROUND = DO NOT PUSH
            case 6: PLAYER 1 FALLING PLAYER 2 GROUND = PUSHING THE GROUNDED PLAYER MUCH MORE
            case 7: PLAYER 1 IN AIR PLAYER 2 IN AIR = EQUAL PUSH BASED ON SPEEDS
            */

        //GROUNDED CASES
        if (thisPlayer.isGrounded() && otherPlayer.isGrounded() && !thisPlayer.pressingJump() && !otherPlayer.pressingJump())
        {
            //case 1  -  Seems to be working now.
            if (thisPlayer.getInputX() + otherPlayer.getInputX() == 0 && !(thisPlayer.controller.collisions.left || thisPlayer.controller.collisions.right || otherPlayer.controller.collisions.left || otherPlayer.controller.collisions.right))
            {
                thisPlayer.setPushX(otherPlayer.getInputX() * otherPlayer.getMovementSpeed()); // subtracting the other players movement from yours.  Slower character gets pushed back
                return;
            }
            // case 2 - Seems to be working now.
            else if ((Mathf.Abs(thisPlayer.getInputX() + otherPlayer.getInputX()) == 1) && !(thisPlayer.controller.collisions.left || thisPlayer.controller.collisions.right || otherPlayer.controller.collisions.left || otherPlayer.controller.collisions.right))
            {
                if (thisPlayer.getInputX() != 0)
                {
                    thisPlayer.setPushX(-1 * thisPlayer.getInputX() * thisPlayer.getMovementSpeed() / 2);
                }
                else
                {
                    thisPlayer.setPushX(otherPlayer.getInputX() * otherPlayer.getMovementSpeed() / 2); ;
                }
                return;
            }
            //case 3 - Seems to be working now.
            else if (Mathf.Abs(thisPlayer.getInputX() + otherPlayer.getInputX()) == 2 && !(thisPlayer.controller.collisions.left || thisPlayer.controller.collisions.right || otherPlayer.controller.collisions.left || otherPlayer.controller.collisions.right))
            {
                if (thisPlayer.getMovementSpeed() >= otherPlayer.getMovementSpeed())
                {
                    return;
                }
                else
                {
                    thisPlayer.setPushX(thisPlayer.getInputX() * otherPlayer.getMovementSpeed() - thisPlayer.getInputX() * thisPlayer.getMovementSpeed());
                }
                return;
            }

            //case 4 - Seems to be working now.
            else if ((thisPlayer.controller.collisions.left || thisPlayer.controller.collisions.right || otherPlayer.controller.collisions.left || otherPlayer.controller.collisions.right))
            {
                if (thisPlayer.controller.collisions.left || thisPlayer.controller.collisions.right)
                {
                    return;
                }
                else if (otherPlayer.controller.collisions.left || otherPlayer.controller.collisions.right)
                {
                    thisPlayer.setPushX(-1 * thisPlayer.getInputX() * thisPlayer.getMovementSpeed());
                }
                return;
            }
        }

    }

    void AirCollisionHandler(Collider2D col)
    {
        col = col as BoxCollider2D;
        otherPlayer = col.transform.parent.gameObject.GetComponent<PlayerMovement>();
        //Debug.Log("scale = " + this.transform.parent.transform.localScale.x);

        float combinedPushBoxSize = Mathf.Abs(this.transform.GetComponent<BoxCollider2D>().size.x * this.transform.parent.transform.localScale.x) + Mathf.Abs(col.transform.GetComponent<BoxCollider2D>().size.x * col.transform.parent.transform.localScale.x);
        float overlap = Mathf.Clamp(combinedPushBoxSize/2 - Mathf.Abs(this.transform.parent.transform.position.x - col.transform.parent.transform.position.x),0,10000);

        //Debug.Log("overlap - " + overlap);


        //MID AIR CASES
        //case 5 - Seems to be working now.
        if ((thisPlayer.isGrounded() && !otherPlayer.isGrounded() && otherPlayer.getYVelocity() > 0) || (thisPlayer.isGrounded() && !otherPlayer.isGrounded() && thisPlayer.getYVelocity() > 0))
        {
            //Dealt with in the playermovement class because it needed to disable the collision before the 1st frame of the jump to make the jump while pushing into the other character go full distance.
            return;
        }

        //case 6 - Seems to be working
        if (otherPlayer.isGrounded() && !thisPlayer.isGrounded() && thisPlayer.getYVelocity() < 0)
        {
            thisPlayer.setMidAirCollision(true);
            LandingCollision(col, overlap);
            return;
        }


        if (thisPlayer.isGrounded() && !otherPlayer.isGrounded() && otherPlayer.getYVelocity() < 0)
        {
            otherPlayer.setMidAirCollision(true);
            LandingCollision(col, overlap);
            return;
        }

        //case 7
        if (!thisPlayer.isGrounded() && !otherPlayer.isGrounded())
        {
            if (Mathf.Abs(thisPlayer.velocitySign() + otherPlayer.velocitySign()) == 0)
            {
                //Needed to create a second variable to control midair movement so i could set momentum and fix positioning based on overlap.
                thisPlayer.setMidAirCollision(true);
                Debug.Log("Mid Air Hit where directions cancel each other out");
                if (System.Math.Round(this.transform.parent.transform.position.x,4) < System.Math.Round(col.transform.parent.transform.position.x,4))
                {
                    thisPlayer.transform.Translate(Vector3.right * (-overlap / 2 - skinWidth));
                    //otherPlayer.transform.Translate(Vector3.left * (-overlap / 2 - skinWidth));
                }
                else
                {
                    thisPlayer.transform.Translate(Vector3.right * (overlap / 2 + skinWidth));
                    //otherPlayer.transform.Translate(Vector3.left * (overlap / 2 - skinWidth));
                }
                //Debug.Log("overlap - " + overlap);
                return;
            }
            else if (Mathf.Abs(thisPlayer.velocitySign() + otherPlayer.velocitySign()) == 1)
            {
                //set push of moving player to 1/2 of the speed, set push of stationary player to 1/2 of the speed of otherplayer
            }
            else if (Mathf.Abs(thisPlayer.velocitySign() + otherPlayer.velocitySign()) == 2)
            {
                //set push of the slower character to the difference between the 2 speeds
            }
        }
    }

    void LandingCollision(Collider2D col, float overlap)
    {
        float playerSideClosestToWall;
        Debug.Log("This Player x = " + this.transform.parent.transform.position.x);
        Debug.Log("Other Player x = " + col.transform.parent.transform.position.x);
        if (System.Math.Round(this.transform.parent.transform.position.x, 4) < System.Math.Round(col.transform.parent.transform.position.x, 4))
        {
            if(thisPlayer.Player2 == false)
            {
                Debug.Log("Player1 is on the left :  Player 1 left side X = " + (thisPlayer.transform.position.x - (this.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(this.transform.parent.transform.localScale.x))) + "     Player 2 right side X = " + (otherPlayer.transform.position.x + (col.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(col.transform.parent.transform.localScale.x))));
            }
            if (otherPlayer.transform.position.x + (col.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(col.transform.parent.transform.localScale.x)) + (overlap / 2 - skinWidth) >= rightWallX)
            {
                float newXPos = rightWallX - (col.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(col.transform.parent.transform.localScale.x)) - (this.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(this.transform.parent.transform.localScale.x))/ 2 - skinWidth;
                thisPlayer.transform.position = new Vector3(newXPos, thisPlayer.transform.position.y, thisPlayer.transform.position.z);
            }
            else if (thisPlayer.transform.position.x - (this.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(this.transform.parent.transform.localScale.x)) - (overlap / 2 - skinWidth) <= leftWallX)
            {
                float newXPos = leftWallX + (this.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(this.transform.parent.transform.localScale.x))/ 2;
                thisPlayer.transform.position = new Vector3(newXPos ,thisPlayer.transform.position.y, thisPlayer.transform.position.z);
            }
            else
            {
                thisPlayer.transform.Translate(Vector3.right * (-overlap / 2 - skinWidth));
            }
        }
        else if (System.Math.Round(this.transform.parent.transform.position.x, 4) > System.Math.Round(col.transform.parent.transform.position.x, 4))
        {
            if (thisPlayer.Player2 == false)
            {
                Debug.Log("Player1 is on the right :  Player 1 right side X = " + (thisPlayer.transform.position.x + (this.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(this.transform.parent.transform.localScale.x))) + "  Player 2 left side X = " + (otherPlayer.transform.position.x - (col.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(col.transform.parent.transform.localScale.x))));
            }
            if (thisPlayer.transform.position.x + (this.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(this.transform.parent.transform.localScale.x)) + (overlap / 2 - skinWidth) >= rightWallX)
            {
                float newXPos = rightWallX - (this.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(this.transform.parent.transform.localScale.x))/ 2;
                Debug.Log(newXPos);
                thisPlayer.transform.position = new Vector3(newXPos, thisPlayer.transform.position.y, thisPlayer.transform.position.z);
            }
            else if (otherPlayer.transform.position.x - (col.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(col.transform.parent.transform.localScale.x))  - (overlap / 2 - skinWidth) <= leftWallX)
            {
                float newXPos = leftWallX + (col.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(col.transform.parent.transform.localScale.x)) + (this.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(this.transform.parent.transform.localScale.x))/ 2 + skinWidth;
                thisPlayer.transform.position = new Vector3(newXPos, thisPlayer.transform.position.y, thisPlayer.transform.position.z);
            }
            else
            {
                thisPlayer.transform.Translate(Vector3.right * (overlap / 2 + skinWidth));
            }
        }
        else if (System.Math.Round(this.transform.parent.transform.position.x, 4) == System.Math.Round(col.transform.parent.transform.position.x, 4))
        {
            //Get what side of the stage they are on, then let the person in the air switch to the outside of the stage when pushing.
            float midPoint = (leftWallX + rightWallX) / 2;
            if(this.transform.parent.transform.position.x < midPoint) //Left side of stage
            {
                if(thisPlayer.controller.collisions.left)  //on wall
                {
                    if(thisPlayer.isGrounded())
                    {
                        thisPlayer.transform.Translate(Vector3.right * ((col.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(col.transform.parent.transform.localScale.x) + skinWidth)));
                    }
                    else
                    {
                        return;
                    }
                }
                else 
                {
                    thisPlayer.transform.Translate(Vector3.right * ((col.transform.GetComponent<BoxCollider2D>().size.x * col.transform.parent.transform.localScale.x + skinWidth)));
                }
            }
            else //Right Side of stage
            {
                if (thisPlayer.controller.collisions.right)  //on wall
                {
                    if (thisPlayer.isGrounded())
                    {
                        thisPlayer.transform.Translate(Vector3.left * ((col.transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(col.transform.parent.transform.localScale.x) - skinWidth)));
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    thisPlayer.transform.Translate(Vector3.left * ((col.transform.GetComponent<BoxCollider2D>().size.x * col.transform.parent.transform.localScale.x - skinWidth)));
                }
            }
        }
    }

    void Update()
    {
        
    }

    void OnTriggerExit2D(Collider2D col)
    {
        thisPlayer.setPushX(0);
    }

}
