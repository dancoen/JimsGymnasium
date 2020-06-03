using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public BoxCollider2D left_wall , right_wall;
    public Transform playerOne, playerTwo;
    public float yOffset;
    public GameObject floor;

    private float minXPosition, maxXPosition;
    private float cameraWidth;

    void Start()
    {
        checkDependencies();

        if(yOffset == 0)
        {
            Debug.Log("yOffset for camera was left as 0, so setting as the camera's y position", transform);
            yOffset = transform.position.y;
        }

        //Set the bounds of the stage so the camera doesnt go past it.  
        //Uses the length of the floor and gets the edge and assumes that is the edge of the stage.  DO NOT MAKE STAGE LONGER THAN NECESSARY.  This assumes the floor's center is at x = 0
        minXPosition = 0 - (floor.GetComponent<Transform>().localScale.x * floor.GetComponent<BoxCollider2D>().size.x) / 2;
        maxXPosition = -1 * minXPosition;

        Debug.Log("CameraMovement minXPosition: " + minXPosition);

        //Get camera width so we can use it in the clamping in FixedUpdate
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        cameraWidth = height * cam.aspect;
        Debug.Log("Camera Width: " + cameraWidth);
    }

    // Set camera at the middle of the 2 players x position.  Move walls to the edge of the camera's viewport until it reaches the edge of the stage using Mathf.Clamp
    // NOTE: keep camera's Z position at a negative number otherwise some items will not get shown.  Also, the clamp function is used assuming the stage is centered at x = 0
    void FixedUpdate()
    {
        transform.position = new Vector3(Mathf.Clamp((playerOne.position.x + playerTwo.position.x) / 2f, minXPosition + cameraWidth / 2f, maxXPosition - cameraWidth / 2f), yOffset, -100);

        //Move the walls to be at the edge of the camera viewport

        left_wall.offset = new Vector2(-1 * (minXPosition - (transform.position.x - cameraWidth / 2)), 0);
        right_wall.offset = new Vector2(-1 * (maxXPosition - (transform.position.x + cameraWidth / 2)), 0);

    }

    void checkDependencies()
    {
        if (left_wall == null || right_wall == null)
        {
            Debug.LogError("Camera's CameraMovement Script is missing the wall objects!", transform);
        }

        if (playerOne == null || playerTwo == null)
        {
            Debug.LogError("Camera's CameraMovement Script is missing the player objects!", transform);
        }

        if (floor == null)
        {
            Debug.LogError("Camera's CameraMovement Script is missing the floor object!", transform);
        }

    }
}
