using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBox : MonoBehaviour
{
    private PlayerMovement thisPlayer;
    private PlayerMovement otherPlayer;
    public BoxCollider2D collider;

    void Start()
    {
        thisPlayer = transform.parent.gameObject.GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        CollisionHandler(col);
    }
    void OnTriggerStay2D(Collider2D col)
    {
        CollisionHandler(col);
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
                Debug.Log("case 1");
                thisPlayer.setPushX(otherPlayer.getInputX() * otherPlayer.getMovementSpeed()); // subtracting the other players movement from yours.  Slower character gets pushed back
                return;
            }
            // case 2 - Seems to be working now.
            else if ((Mathf.Abs(thisPlayer.getInputX() + otherPlayer.getInputX()) == 1) && !(thisPlayer.controller.collisions.left || thisPlayer.controller.collisions.right || otherPlayer.controller.collisions.left || otherPlayer.controller.collisions.right))
            {
                Debug.Log("case 2");
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
                Debug.Log("case 3");
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
                Debug.Log("case 4");
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

        //MID AIR CASES
        //case 5 - Seems to be working now.
        if ((thisPlayer.isGrounded() && !otherPlayer.isGrounded() && otherPlayer.getYVelocity() > 0) || (thisPlayer.isGrounded() && !otherPlayer.isGrounded() && thisPlayer.getYVelocity() > 0))
        {
            //Dealt with in the playermovement class because it needed to disable the collision before the 1st frame of the jump to make the jump while pushing into the other character go full distance.
            return;
        }

        //case 6 - Might have to break this into 2 cases so that it doesnt double up on the pushing
        if(otherPlayer.isGrounded() && !thisPlayer.isGrounded() && thisPlayer.getYVelocity() < 0)
        {
            return;
        }
    
        if(thisPlayer.isGrounded() && !otherPlayer.isGrounded() && otherPlayer.getYVelocity() < 0)
        {
            return;
        }

        //case 7
        if (!thisPlayer.isGrounded() && !otherPlayer.isGrounded())
        {
            if (Mathf.Abs(thisPlayer.velocitySign() + otherPlayer.velocitySign()) == 0)
            {
                //Needed to create a second variable to control midair movement so i could set momentum and fix positioning based on overlap.
                thisPlayer.setMidAirCollision(true);
                return;
            }
            else if (Mathf.Abs(thisPlayer.velocitySign() + otherPlayer.velocitySign()) == 1)
            {
                //set push of moving player to 1/2 of the speed, set push of stationary player to 1/2 of the speed of otherplayer
            }
            else if(Mathf.Abs(thisPlayer.velocitySign() + otherPlayer.velocitySign()) == 2)
            {
                //set push of the slower character to the difference between the 2 speeds
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
