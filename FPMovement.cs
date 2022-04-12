using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Uses a Character Controller component to handle colliders and stuff.
// This is also using the old input system.
// This is extremely basic and could be done in many different, fancier ways.
// I think this was heavily based on a Brackey's controller, can't remember.

public class FPMovement : MonoBehaviour
{
    [SerializeField] CharacterController _controller;
    [SerializeField] Transform _groundCheck; // Uses an empty with checksphere to determine if we're grounded.
    [SerializeField] float _groundDist = 0.25f; // The radius of the above checksphere.
    [SerializeField] LayerMask _groundMask; // Used to figure out what is the ground
    [SerializeField] float _playerLowerVerticalBounds = -5f; // Used as a lower y axis limit for the player in the IsPlayerFucked() method. Default is -5f

    public float gravityVelocity = -9.8f * 2; // Gravity. Public instead of private in case we want to have some things affect gravity for the player.
    public float speed = 8f; // Move speed
    public float jumpVelocity = 6f; // Jumping velocity to add

    private Vector3 _velocity; // Variable to store our velocity and shit.
    private bool _isGrounded;

    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDist, _groundMask); // are we on the floor?

        if(_isGrounded && _velocity.y < 0) // prevents floaty anti-gravity shit
        {
            _velocity.y = -2f;
        }
        // Old style input axes.
        float xAxis = Input.GetAxisRaw("Horizontal");
        float zAxis = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * xAxis + transform.forward * zAxis; // get player movement input and normalize it below.
        move.Normalize();

        _controller.Move(speed * Time.deltaTime * move); // apply movement to player entity

        if(Input.GetButtonDown("Jump") && _isGrounded) // checking to see if we can jump, and jumping if so.
        {
            _velocity.y = jumpVelocity; 
        }

        _velocity.y -= gravityVelocity * Time.deltaTime; // applying gravity to player

        _controller.Move(_velocity * Time.deltaTime); // more gravity shit
        IsPlayerFucked(); // Did the player fall out of the world?
    }

    // This will put the player back into the play area if they somehow fall out of the world.
    void IsPlayerFucked()
    {
        if (_controller.transform.position.y <= -_playerLowerVerticalBounds)
        {
            print("The player fell out of the world somehow");
            _controller.transform.position = new Vector3(0, 2, 0); // Set this vector 3 to somewhere in your playable area.
        }
    }
}
