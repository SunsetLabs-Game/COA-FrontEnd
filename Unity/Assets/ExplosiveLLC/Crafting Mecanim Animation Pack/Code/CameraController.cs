using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;  //The character to follow
    public float distance = 10f;  // Distance from the camera to the character
    public float height = 5f;  //  Height of the camera above the character
    public float rotationSpeed = 5f;  // Camera rotation speed
    public float zoomSpeed = 2f;  //Zoom speed
    public float minZoom = 5f;  // Minimum zoom distance
    public float maxZoom = 15f;  // Maximum zoom distance

    private float currentZoom = 10f;
    private float currentRotation = 0f;

    void Update()
    {
        // Control zoom with the mouse wheel
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Control the rotation with the right mouse button
        if (Input.GetMouseButton(1))
        {
            currentRotation += Input.GetAxis("Mouse X") * rotationSpeed;
        }

        //  Update camera position and rotation
        Vector3 direction = new Vector3(0, height, -currentZoom);
        Quaternion rotation = Quaternion.Euler(0, currentRotation, 0);
        transform.position = target.position + rotation * direction;

        //  Make the camera always look at the character
        transform.LookAt(target.position);
    }
}

