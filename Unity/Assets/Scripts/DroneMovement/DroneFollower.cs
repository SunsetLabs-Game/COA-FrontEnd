using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages drone behavior, allowing it to follow a target character with smooth motion, rotation,
/// and floating effects. The drone adjusts its position based on the camera's perspective.
/// </summary>
public class DroneFollower : MonoBehaviour
{
    /// <summary>
    /// The character that the drone will follow.
    /// </summary>
    public Transform target;

    /// <summary>
    /// The main camera's transform used for calculating relative positioning.
    /// </summary>
    public Transform cameraTransform;

    /// <summary>
    /// Desired follow distance behind the target.
    /// </summary>
    public float followDistance = 2.3f;

    /// <summary>
    /// Base height for the drone above the target.
    /// </summary>
    public float followHeight = 2.0f;

    /// <summary>
    /// Smoothing speed for position transitions.
    /// </summary>
    public float smoothSpeed = 8.0f;

    /// <summary>
    /// Amplitude of the floating effect.
    /// </summary>
    public float floatAmount = 0.5f;

    /// <summary>
    /// Speed of the floating oscillation.
    /// </summary>
    public float floatSpeed = 1.5f;

    /// <summary>
    /// Smoothing speed for rotation transitions.
    /// </summary>
    public float rotationSmoothSpeed = 3.0f;

    /// <summary>
    /// Smoothing speed for position transitions when idle.
    /// </summary>
    public float transitionSpeed = 5.0f;

    /// <summary>
    /// Internal velocity tracker for smooth movement.
    /// </summary>
    private Vector3 velocity = Vector3.zero;

    /// <summary>
    /// Initial offset for the drone's relative position.
    /// </summary>
    private Vector3 offset;

    /// <summary>
    /// Timer for managing floating oscillation.
    /// </summary>
    private float floatTimer = 0f;

    /// <summary>
    /// Flag to check if the target is moving.
    /// </summary>
    private bool isMoving = false;

    /// <summary>
    /// Target rotation for smooth orientation adjustments.
    /// </summary>
    private Quaternion targetRotation;

    /// <summary>
    /// Sets the initial relative position and rotation of the drone.
    /// </summary>
    void Start()
    {
        offset = new Vector3(1.0f, followHeight, -followDistance); // Offset to position the drone
        transform.position = Vector3.SmoothDamp(transform.position, offset, ref velocity, 1f / smoothSpeed);
        targetRotation = transform.rotation;
    }

    /// <summary>
    /// Updates the drone's position and rotation in real-time based on the target's and camera's positions.
    /// Applies a floating effect to simulate hovering.
    /// </summary>
    void Update()
    {
        if (target != null && cameraTransform != null)
        {
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 cameraRight = cameraTransform.right;
            Vector3 desiredPosition = target.position
                                      + (-cameraForward * followDistance)
                                      + (cameraRight * offset.x)
                                      + (Vector3.up * followHeight);

            floatTimer += Time.deltaTime * floatSpeed;
            float floatOffset = Mathf.Sin(floatTimer) * floatAmount;
            desiredPosition.y += floatOffset;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / transitionSpeed);

            Vector3 directionToTarget = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            targetRotation = Quaternion.Slerp(targetRotation, lookRotation, rotationSmoothSpeed * Time.deltaTime);

            transform.rotation = targetRotation;
        }
    }

    /// <summary>
    /// Performs fixed-time updates for smoother drone motion when the target is moving.
    /// Calculates the desired position and applies floating effects.
    /// </summary>
    void FixedUpdate()
    {
        if (target != null && cameraTransform != null)
        {
            isMoving = target.GetComponent<Rigidbody>()?.velocity.magnitude > 0.1f;

            if (isMoving)
            {
                Vector3 cameraForward = cameraTransform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();

                Vector3 cameraRight = cameraTransform.right;
                Vector3 desiredPosition = target.position
                                          + (-cameraForward * followDistance)
                                          + (cameraRight * offset.x)
                                          + (Vector3.up * followHeight);

                floatTimer += Time.fixedDeltaTime * floatSpeed;
                float floatOffset = Mathf.Sin(floatTimer) * floatAmount;
                desiredPosition.y += floatOffset;

                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);
            }
        }
    }
}
