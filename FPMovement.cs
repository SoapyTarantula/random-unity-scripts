using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Uses a Character Controller component to handle colliders and stuff.
// This is extremely basic and could be down in many different, fancier ways.
// I think this was heavily based on a Brackey's controller, can't remember.

public class FPMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform groundCheck;
    public float groundDist = 0.25f;
    public LayerMask groundMask;
    public float gravityVelocity = -9.8f * 2;
    Vector3 velocity;
    public float speed = 8f; // Move speed
    public float jumpVelocity = 6f; // Jumping velocity to add
    private bool isGrounded;
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask); // are we on the floor?

        if(isGrounded && velocity.y < 0) // prevents floaty anti-gravity shit
        {
            velocity.y = -2f;
        }

        float xAxis = Input.GetAxisRaw("Horizontal");
        float zAxis = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * xAxis + transform.forward * zAxis; // get player movement input
        move.Normalize();

        controller.Move(speed * Time.deltaTime * move); // apply movement to player entity

        if(Input.GetButtonDown("Jump") && isGrounded) // checking to see if we can jump, and jumping if so.
        {
            velocity.y = jumpVelocity; 
        }

        velocity.y -= gravityVelocity * Time.deltaTime; // applying gravity to player

        controller.Move(velocity * Time.deltaTime); // more gravity shit
        IsPlayerFucked();
    }

    // This will put the player back into the play area if they somehow fall out of the world.
    void IsPlayerFucked()
    {
        if (controller.transform.position.y <= -5f)
        {
            print("The player fell out of the world somehow");
            controller.transform.position = new Vector3(0, 2, 0);
        }
    }
}
