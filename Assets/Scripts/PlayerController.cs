using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 5f;
    public bool allowSprint = true;
    public float gravity = -20f;

    [Header("Jumping")]
    public bool allowJump = false;
    public float jumpHeight = 1.2f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 85f;
    public bool invertY = false;

    private CharacterController controller;
    private float verticalVelocity;   // your gravity / jump speed
    private float pitch;              // camera's up/down angle

    void Start()
    {
        controller = GetComponent<CharacterController>();

        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>().transform;

        LockCursor(true);
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        // Mouse axes are already per-frame deltas, so no Time.deltaTime here.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertY ? -1f : 1f);

        // Turn the whole body left/right...
        transform.Rotate(Vector3.up * mouseX);

        // ...but only tilt the camera up/down, clamped so you can't flip over.
        pitch = Mathf.Clamp(pitch - mouseY, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        bool grounded = controller.isGrounded;

        // Small downward push keeps isGrounded reliable on slopes.
        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        float x = Input.GetAxisRaw("Horizontal"); // A/D
        float z = Input.GetAxisRaw("Vertical");   // W/S


        // Move relative to where the player is facing; clamp so diagonals aren't faster.
        Vector3 direction = Vector3.ClampMagnitude(transform.right * x + transform.forward * z, 1f);

        float speed = (allowSprint && Input.GetKey(KeyCode.LeftShift)) ? sprintSpeed : walkSpeed;

        if (allowJump && grounded && Input.GetButtonDown("Jump"))
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = direction * speed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}