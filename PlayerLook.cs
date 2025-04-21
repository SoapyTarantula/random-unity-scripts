
/*
TREE:
    |PlayerEmpty
    |>CameraPivot
    |>>Camera
*/

using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerLook : MonoBehaviour
{
    [Header("Mouse Settings")]
    [Range(0.1f, 1f)] [SerializeField] float mouseSensitivity = 0.25f;
    [SerializeField] private float verticalLookLimit = 180f; // Max total vertical look angle, each direction gets half
    [SerializeField] private bool cursorHidden = false;
    [Header("References")]
    [SerializeField] Transform cameraPivot; // Reference to the camera pivot
    private float verticalRotation = 0f; // Tracks cumulative vertical rotation
    private Controls _controls;


    private void Awake()
    {
        _controls = new Controls();
    }

    void Start()
    {
        if (cameraPivot == null) { cameraPivot = transform; }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = cursorHidden;
    }

    void Update()
    {
        HandleMouseLook();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = _controls.KBM.Look.ReadValue<Vector2>().x;
        float mouseY = _controls.KBM.Look.ReadValue<Vector2>().y;

        // Horizontal rotation (yaw) applied to the player
        transform.parent.Rotate(0, mouseX * mouseSensitivity, 0);

        // Vertical rotation (pitch) applied to the camera pivot
        verticalRotation -= mouseY * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);

        cameraPivot.localEulerAngles = new Vector3(verticalRotation, 0, 0);
    }
}
