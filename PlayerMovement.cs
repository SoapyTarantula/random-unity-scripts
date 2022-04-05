using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// This script assumes the new input system is being used, but could be modified for the old input system without too much effort.
// The script is assuming a 3D Rigidbody is used for the player object, and it also assumes you have constrained the XYZ *rotations* of the Rigidbody.
// I am using a Sphere cast to determine if the player is on the ground and therefor can jump. A Spherecast is just a fat Raycast.
// I am also assuming a Cinemachine Virtual Camera is being used for player perspective, as such this script does not directly handle moving the player camera itself.
// Some comments are spread throughout the script to try to explain what is happening.

// Poorly made by SoapyTarantula | https://github.com/SoapyTarantula | https://twitter.com/soapytarantula

// REQUIRES NAMESPACES:
// using UnityEngine;
// using UnityEngine.InputSystem; // Only if you are using the new Unity Input System package.


[RequireComponent(typeof(Rigidbody))] // This forces a 3D Rigidbody onto whatever this script is attached to.


public class PlayerMovement : MonoBehaviour
{
    private Controls controls; // Controls (capital C) is the name of my new input system generated script.
    private Vector3 _move; // WASD movement.
    private Vector2 _mouseLook; // Mouse delta, aka change of position since last frame.
    private Rigidbody _rb; // The Rigidbody we're using.
    private float _distToGround; // Used for our ground-check Spherecast.
    private bool _jumpPressed; // Used to determine if we're trying to jump.
    private float _originalMoveSpeed; // Used to save our original _moveSpeed variable so we can adjust it while in the air, then reset it back to its default.

    [Range(1f, 100f)] [SerializeField] float _mouseSensitivity = 50f; // Limits the mouse sensitivity between 1 and 100.
    [SerializeField] float _turnSpeed = 90f; // Only applies if tank controls are true.
    [SerializeField] bool _isTankControls = false; // Do we want to use tank style turning? By default it is set to false.
    [SerializeField] bool _moveInAir = true; // Do we want to allow player input while in the air? Defaults to true. NOTE: If this is set to false it is entirely possible for the player to get stuck on ledges where they cannot move. Only disable if you've solved this or are certain it won't happen.
    [SerializeField] float _moveSpeed = 1000f; // Has to be a high value if using `Time.fixedDeltaTime` however you can adjust the value to something else for your own project.
    [SerializeField] float _jumpForce = 300f; // Same as above.
    [SerializeField] float _sphereRadius = 0.15f; // The radius of the SphereCast we use to determine if we're grounded.
    [SerializeField] float _inAirSpeedMultiplier = 0.5f; // Used to adjust player move speed while not grounded.

    // In Awake() we are initializing a new instance of the Controls() script and applying it to a local variable, because we have to do it that way for some reason.
    private void Awake()
    {
        controls = new Controls();
    }

    // OnEnable() and OnDisable() are required by the new input system.
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Start() gets called AFTER Awake()
    private void Start()
    {
        _originalMoveSpeed = _moveSpeed; // Save our original _moveSpeed value so we can adjust it later and reset to the default.

        // This is just to hide the cursor from view and lock it to the center of the screen, so we don't click outside of the game on accident.
        Cursor.lockState = CursorLockMode.Locked; // Other CursorLockMode options are "none" and "confined."
        Cursor.visible = false; //Hides the cursor entirely so it doesn't just sit in our screen.

        _rb = GetComponent<Rigidbody>(); // The 3D Rigidbody of the object this script it attached to.
        _distToGround = GetComponent<Collider>().bounds.extents.y; // Used to see if we're on the ground.
    }
    // Update() is where we check player input & do things that can operate every frame without causing issues.
    private void Update()
    {
        // Check if we're on the ground and adjust our movement speed for in air control.
        if (!IsGrounded() && _moveInAir)
        {
            _moveSpeed = _originalMoveSpeed * _inAirSpeedMultiplier; // Sets _moveSpeed to _moveSpeed multiplied by our float, set above.
        }
        else
        {
            _moveSpeed = _originalMoveSpeed; // Returns _moveSpeed to its original value once we hit the ground.
        }

         _move = controls.Movement.WASD.ReadValue<Vector2>(); // We are watching the "WASD" action in our Movement control map to see what value it is currently at.
        _mouseLook = controls.Movement.MouseLook.ReadValue<Vector2>(); // Same as above but for our mouse.
        _jumpPressed = controls.Movement.Jump.triggered; // Watching to see if we're trying to jump by pressing the button assigned (I use spacebar).

        // We normalize values that we do not want to be above 1, otherwise this could lead to strange speed differences when moving in two directions at once. The most common usecase for this is when getting direction vectors.
        _mouseLook.Normalize();
        _move.Normalize();
    }

    // FixedUpdate() is where we must do anything with regards to the Unity Physics system for intended performance, including moving Rigidbodies around.
    private void FixedUpdate()
    {
        HandleRotation();

        if (!IsGrounded() && !_moveInAir) // Check right away to see if we're on the ground and if we've got _moveInAir disabled.
        {
            return;
        }
        else
        {
            HandleMovement();
        }
    }

    // Checks to see if we're on the ground by shooting a Spherecast out from the player transform. _rayDist is the distance the Spherecast travels *from the "skin" of the transform*. Returns true if it hits anything in that distance.
    private bool IsGrounded()
    {
        float _rayDist = 0.1f;
        Ray _ray = new Ray(transform.position, -Vector3.up);
        return Physics.SphereCast(_ray, _sphereRadius, _distToGround + _rayDist);
    }

    // Here is our overall movement control function.
    void HandleMovement()
    {
        // Handles movement on the ground assuming tank control mode is false
        if (!_isTankControls)
        {
            _rb.AddForce(transform.forward * _move.y * _moveSpeed * Time.fixedDeltaTime + transform.right * _move.x * _moveSpeed * Time.fixedDeltaTime);
        }
        // Ground movement if tank control mode is true.
        else
        {
            _rb.AddForce(transform.forward * _move.y * _moveSpeed * Time.fixedDeltaTime);
        }
        // Jumping stuff, we use a Spherecast in the IsGrounded() function to determine if we're on the ground or not.
        if (_jumpPressed && IsGrounded())
        {
            _rb.AddForce(Vector3.up * _jumpForce * Time.fixedDeltaTime, ForceMode.Impulse); // This will apply an upward force to the rigidbody, using world space. Switch `Vector3.up` to `transform.up` if you want to use local space.
            _jumpPressed = false; // Immediately reset our boolean to false after applying the inital force to get rid of weird moments where some jumps are higher than others.
        }
    }

    // Rotation is handled seperately from movement so we can continue to turn while in the air.
    void HandleRotation() 
    {    
        // Handles player rotation if tank controls is set to false. Uses the mouse's left & right delta position.
        if (!_isTankControls)
        {
            transform.Rotate(Vector3.up* _mouseLook.x* _mouseSensitivity * 2 * Time.fixedDeltaTime);
        }
        //Handles player rotation if tank controls is true. Uses the left & right keyboard controls.
        else
        {
            transform.Rotate(Vector3.up * _move.x * _turnSpeed * Time.fixedDeltaTime);
        }
    }
}

