using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Reference to the Rigidbody component for movement
    Rigidbody rb;

    // References for camera and ground check
    [SerializeField] private Transform cam;
    [SerializeField] private Transform groundCheck;

    // Movement parameters (editable in the Inspector)
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float accelerationSpeed = 3f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] float friction = 0.6f;

    // Ground layer for collision detection
    [SerializeField] private LayerMask ground;

    // Smoothing variables for turning
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    // References for the crosshair
    [SerializeField] private Transform crosshair;

    // Input parameters
    private float horizontalInput;
    private float verticalInput;

    // Camera mode parameter
    private bool freeLook;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        freeLook = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Get player input for horizontal and vertical movement
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            // Handle jumping
            HandleJump();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            freeLook = !freeLook;
        }
        if (!freeLook)
        {
            transform.rotation = cam.rotation;
        }
    }

    void FixedUpdate()
    {
        // Create a normalized direction vector from input
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // If there's significant input:
        if (direction.magnitude >= 0.1f)
        {
            Vector3 moveDirection;

            // Free look 3rd-person camera
            // Calculate the target rotation based on input and camera direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0);

            // Calculate the movement direction based on the target angle
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            

            // Normalize the movement direction
            moveDirection = moveDirection.normalized;

            // Gradually accelerate towards the target velocity
            Vector3 targetVelocity = new Vector3(moveDirection.x * movementSpeed, rb.velocity.y, moveDirection.z * movementSpeed);
            rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * accelerationSpeed);
        }
        else
        {
            // Apply friction when there's no input
            rb.velocity = new Vector3(rb.velocity.x * friction, rb.velocity.y, rb.velocity.z * friction);
        }
    }

    // Function to handle jumping
    private void HandleJump()
    {
        if (IsGrounded())
        {
            // Apply jump force if the player is grounded
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
    }

    // Function to check if the player is grounded
    private bool IsGrounded()
    {
        // Use a sphere cast to check for ground contact
        return Physics.CheckSphere(groundCheck.position, .1f, ground);
    }
}
