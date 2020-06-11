using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBox : MonoBehaviour
{
    private PlayerMovement thisPlayer;
    private float pushForce = .5f;
    private float jumpingCoef = 1f;

    private PlayerMovement otherPlayer;

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

        //case 4 - Issue when on the wall and both players push into each other.  Works fine when only center player pushes into a stationary cornered player.
        if(thisPlayer.isGrounded() && otherPlayer.isGrounded() && (thisPlayer.controller.collisions.left || thisPlayer.controller.collisions.right || otherPlayer.controller.collisions.left || otherPlayer.controller.collisions.right))
        {
            Debug.Log("case 4");
            thisPlayer.setPushX(-1 * thisPlayer.getInputX() * thisPlayer.getMovementSpeed());
            otherPlayer.setPushX(-1 * otherPlayer.getInputX() * otherPlayer.getMovementSpeed());
            return;
        }
                
        //case 1  -  Seems to be working now.
        else if (thisPlayer.getInputX() + otherPlayer.getInputX() == 0 && thisPlayer.isGrounded() && otherPlayer.isGrounded())
        {
            Debug.Log("case 1");
            thisPlayer.setPushX(otherPlayer.getInputX() * otherPlayer.getMovementSpeed()); // subtracting the other players movement from yours.  Slower character gets pushed back
            return;
        }
        // case 2 - Seems to be working now.
        else if (Mathf.Abs(thisPlayer.getInputX() + otherPlayer.getInputX()) == 1 && thisPlayer.isGrounded() && otherPlayer.isGrounded())
        {
            Debug.Log("case 2");
            if (thisPlayer.getInputX() != 0)
            {
                thisPlayer.setPushX(-1 * thisPlayer.getInputX() * thisPlayer.getMovementSpeed() / 2);
                otherPlayer.setPushX(thisPlayer.getInputX() * thisPlayer.getMovementSpeed() / 2);
            }
            else
            {
                thisPlayer.setPushX(otherPlayer.getInputX() * otherPlayer.getMovementSpeed() / 2); ;
                otherPlayer.setPushX(-1 * otherPlayer.getInputX() * otherPlayer.getMovementSpeed() / 2);
            }
            return;
        }
        //case 3 - Seems to be working now.
        else if (Mathf.Abs(thisPlayer.getInputX() + otherPlayer.getInputX()) == 2 && thisPlayer.isGrounded() && otherPlayer.isGrounded())
        {
            Debug.Log("case 3");
            if (thisPlayer.getMovementSpeed() >= otherPlayer.getMovementSpeed())
            {
                otherPlayer.setPushX(otherPlayer.getInputX() * thisPlayer.getMovementSpeed() - otherPlayer.getInputX() * otherPlayer.getMovementSpeed());
            }
            else
            {
                thisPlayer.setPushX(thisPlayer.getInputX() * otherPlayer.getMovementSpeed() - thisPlayer.getInputX() * thisPlayer.getMovementSpeed());
            }
            return;
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
