using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    public Transform centerPoint; 
    public float radius = 10f; 
    public float speed = 1f; 
    public float rotationSpeed = 50f; 

    private float angle = 0f; 

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
