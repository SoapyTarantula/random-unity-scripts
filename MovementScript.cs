using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A homebrewed player & camera movement script meant for first-person with mouse aim. Includes the ability to dash and crouch with snappier "mario" style jumping.

// Poorly made by SoapyTarantula | https://github.com/SoapyTarantula | https://twitter.com/soapytarantula

[RequireComponent(typeof(Rigidbody))]
public class MovementScript : MonoBehaviour
{
    [Header("General Movement")]
    public float originalMoveSpeed = 300f; // The original move speed of the character, in case we need to temporarily change and revert back to it.
    [SerializeField] Rigidbody _rb; // The Rigidbody we're assuming the player is using.
    [SerializeField] Transform _playerTransform; // Real fucking important to place this in the inspector as I can't be fucked to do it through code right now.
    public Transform OrientBox; // OrientBox is an empty the script uses to handle rotations of the camera.
    public float dragAmount; // Controls Rigidbody drag.
    public bool CanDash = true; // Can we dash?
    public bool CanCrouch = true; // Can we crouch?
    public float DashDistance = 10f; // How far do we want to dash?
    public float DashCooldown = 1f; // How much time in seconds between dash attempts?
    private float _TimeToNextDash = 0f; // Dash timer stuff.
    float _horizontalInput, _verticalInput; // Player inputs using the old input system system.

    [Header("Mouse Look")]
    [SerializeField] Camera _camera;
    public float mouseSens = 100f;
    private float _xRot, _yRot;

    [Header("Jumping Variables")]
    public float _fallMulti = 2.5f; // Effective gravity multiplier if we're holding the jump button.
    public float _shortJumpMulti = 2f; // Same as above, but if we're not holding jump.
    public float jumpForce = 300f; // Jump impulse strength.

    // All the stuff for checks to see if we're on the ground.
    [Header("Ground Check")]
    public float _gndDist; // A radius for a checksphere we do from the _groundCheck transform's origin.
    [SerializeField] Transform _groundCheck; // An empty placed somewhere near the player's feet.
    public LayerMask layerGround; // What layer are we considering the ground to be?
    bool _isGrounded;

    private Vector3 _moveDir;

    // Use Start() to make sure some things are filled out before we continue, also handles preventing the rigidbody from falling over & locks the cursor to center/makes it invisible.
    void Start()
    {
        if(_camera == null)
        {
            try
            {
                _camera = Camera.main;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        if(_rb == null)
        {
            try
            {
                _rb = GetComponent<Rigidbody>();
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        _rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        Inputs();
        IsGrounded();
        CameraLook();
        Jumping();
    }

    private void FixedUpdate()
    {
        Movement();
        if (CanCrouch)
        {
            Crouching();
        }
        BetterJumpingAndFalling();
    }

    // Just helpful to better visualize your checksphere radius for ground checking.
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(_groundCheck.position, _gndDist);
    }

    // Returns true if the checksphere hits anything marked with the ground layer.
    bool IsGrounded()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _gndDist, layerGround);
        return _isGrounded;
    }

    // Player Rigidbody movement & dashing.
    void Movement()
    {
        _rb.drag = dragAmount;
        _moveDir = OrientBox.forward * _verticalInput + OrientBox.right * _horizontalInput; // Figure out our movement vector
        _rb.AddForce(10f * originalMoveSpeed * Time.fixedDeltaTime * _moveDir.normalized, ForceMode.Force); // Apply a force because we're using a rigidbody and that's the preferred way

        // Handles dashing
        if (CanDash && _TimeToNextDash <= Time.time)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _rb.AddForce(DashDistance * Time.fixedDeltaTime * new Vector3(_rb.velocity.x, 0f, _rb.velocity.z), ForceMode.Impulse);
                _TimeToNextDash = Time.time + DashCooldown;
            }
        }
    }
    // Handles jumping.
    void Jumping()
    {
        if (IsGrounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _rb.AddForce(jumpForce * Time.fixedDeltaTime * transform.up, ForceMode.Impulse);
            }
        }
        else return;
    }

    // Handles crouching by lowering the y-scale of the player to 0.5f and reverts back to 1 when the crouch key is released
    void Crouching()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            _playerTransform.transform.localScale = new Vector3(1, 0.5f, 1);
        }
        else
        {
            _playerTransform.transform.localScale = Vector3.one;
        }
    }

    // Uses the "mario jumping" technique to provide snappier feeling jumps & falls
    void BetterJumpingAndFalling()
    {
        if (_rb.velocity.y < 0)
        {
            _rb.velocity += (_fallMulti - 1 * Time.deltaTime) * Physics.gravity.y * 2 * Vector3.up;
        }
        else if (_rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            _rb.velocity += (_shortJumpMulti - 1 * Time.deltaTime) * Physics.gravity.y * 2 * Vector3.up;
        }
    }
    
    // Mouse aim by way of old input system and rotating the OrientBox. Clamps the camera's up/down rotation by positive & negative 90 degrees.
    void CameraLook()
    {
        float _mouseHorizontal = Input.GetAxisRaw("Mouse X") * Time.deltaTime * mouseSens;
        float _mouseVertical = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * mouseSens;

        _yRot += _mouseHorizontal;
        _xRot -= _mouseVertical;
        _xRot = Mathf.Clamp(_xRot, -90f, 90f);

        _camera.transform.rotation = Quaternion.Euler(_xRot, _yRot, 0f);
        OrientBox.rotation = Quaternion.Euler(0, _yRot, 0f);
    }

    // Get player's WASD input.
    void Inputs()
    {
        // Old style input method, should probably update this to the modern system soon
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
    }
}
