using UnityEngine;

using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;  // El personaje a seguir
    public float distance = 10f;  // Distancia de la cámara al personaje
    public float height = 5f;  // Altura de la cámara sobre el personaje
    public float rotationSpeed = 5f;  // Velocidad de rotación de la cámara
    public float zoomSpeed = 2f;  // Velocidad del zoom
    public float minZoom = 5f;  // Distancia mínima del zoom
    public float maxZoom = 15f;  // Distancia máxima del zoom

    private float currentZoom = 10f;
    private float currentRotation = 0f;

    void Update()
    {
        // Controlar el zoom con la rueda del ratón
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Controlar la rotación con el botón derecho del ratón
        if (Input.GetMouseButton(1))
        {
            currentRotation += Input.GetAxis("Mouse X") * rotationSpeed;
        }

        // Actualizar la posición y rotación de la cámara
        Vector3 direction = new Vector3(0, height, -currentZoom);
        Quaternion rotation = Quaternion.Euler(0, currentRotation, 0);
        transform.position = target.position + rotation * direction;

        // Hacer que la cámara siempre mire al personaje
        transform.LookAt(target.position);
    }
}

