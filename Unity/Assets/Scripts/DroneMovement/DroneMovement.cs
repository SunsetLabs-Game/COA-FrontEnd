using UnityEngine;

/// <summary>
/// Controls the circular movement of a drone around a center point.
/// This script should be attached to a drone GameObject that needs to orbit around a specific point in the scene.
/// </summary>
public class DroneMovement : MonoBehaviour
{
    /// <summary>
    /// Reference to the transform that serves as the center point for the drone's circular movement.
    /// </summary>
    public Transform centerPoint; 
    /// <summary>
    /// The radius of the circular path that the drone follows.
    /// </summary>
    public float radius = 10f; 
    /// <summary>
    /// The speed at which the drone moves along its circular path.
    /// Higher values result in faster orbital movement.
    /// </summary>
    public float speed = 1f; 
    /// <summary>
    /// The speed at which the drone rotates around its local right axis.
    /// </summary>
    public float rotationSpeed = 50f; 
    private float angle = 0f; 

    /// <summary>
    /// Updates the drone's position and rotation each frame.
    /// Calculates the new position based on circular movement around the center point
    /// and updates the drone's rotation to face its movement direction.
    /// </summary>
    void Update()
    {
        angle += speed * Time.deltaTime;

        float x = centerPoint.position.x + Mathf.Cos(angle) * radius;
        float z = centerPoint.position.z + Mathf.Sin(angle) * radius;
        float y = transform.position.y; 

        transform.position = new Vector3(x, y, z);

        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);

        Vector3 direction = new Vector3(x, y, z) - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
