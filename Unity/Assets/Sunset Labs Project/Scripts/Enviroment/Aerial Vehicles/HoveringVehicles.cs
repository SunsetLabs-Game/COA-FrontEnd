using UnityEngine;

public class HoveringVehicles : MonoBehaviour
{
    private int nodeIndex = 0;
    private int progressDir = 1;

    private Transform currentNode;
    private float minimumDistanceSQ;
    private const float EPS = 0.0001f;

    [Header("General Parameters")]
    [SerializeField] private bool canHover = true;
    [SerializeField] private AerialWayPoint wayPoint;

    [Header("Regular Parameters")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float minimumDistance = 1f;

    [Header("Hover(Bob) Parameters Vertical")]
    [SerializeField] private float verticalOffset = 0.0f;
    [SerializeField] private float verticalAmplitude = 0.25f;
    [SerializeField] private float verticalFrequency = 1.50f;

    [Header("Hover(Bob) Parameters Horizontal")]
    [SerializeField] private float horizontalPhase = 0.0f;
    [SerializeField] private float horizontalAmplitude = 0.15f;
    [SerializeField] private float horizontalFrequency = 1.00f;

    private void OnValidate()
    {
        minimumDistanceSQ = minimumDistance * minimumDistance;
        if (moveSpeed < 0f) moveSpeed = 0f;
        if (rotationSpeed < 0f) rotationSpeed = 0f;
    }

    private void Awake()
    {
        progressDir = 1;
        nodeIndex = wayPoint.GetClosestNode(transform.position);
        minimumDistanceSQ = MathPhysics_Helper.Square(minimumDistance);
    }

    private void Start()
    {
        currentNode = wayPoint.WayPointNodes[nodeIndex];
        HoveringVehicleController.Instance.SubscribeToTest(this);
    }

    public void HoveringVehicle_Update()
    {
        float delta = Time.deltaTime;
        if (wayPoint == null || wayPoint.WayPointNodes.Count == 0)
        {
            return;
        }
        currentNode = wayPoint.WayPointNodes[nodeIndex];
        Vector3 hoverOffset = canHover ? CalculateHover(currentNode.rotation) : Vector3.zero;
        Vector3 destination = currentNode.position + hoverOffset;

        HandleMovement(delta, destination);
        float distanceSq = (currentNode.position - transform.position).sqrMagnitude;
        if (distanceSq < minimumDistanceSQ)
        {
            NextIndex();
        }
    }

    private void NextIndex()
    {
        nodeIndex += progressDir;
        int count = wayPoint.WayPointNodes.Count;

        if (nodeIndex >= count)
        {
            nodeIndex = count - 2;
            progressDir = -1;
        }
        else if (nodeIndex < 0)
        {
            nodeIndex = 1;
            progressDir = 1;
        }
        nodeIndex = Mathf.Clamp(nodeIndex, 0, Mathf.Max(0, count - 1));
    }

    private void HandleMovement(float delta, Vector3 destination)
    {
        transform.SetPositionAndRotation(TargetPosition(delta, destination), TargetRotation(delta, destination));
    }

    private Vector3 CalculateHover(Quaternion orientation)
    {
        float t = Time.time;
        Vector3 offset = Vector3.zero;

        // vertical bob
        float v = Mathf.Sin(t * Mathf.PI * 2f * verticalFrequency) * verticalAmplitude + verticalOffset;
        offset += Vector3.up * v;

        // horizontal strafing relative to node orientation (so the horizontal offset follows waypoint orientation)
        Vector3 right = orientation * Vector3.right;
        right.y = 0.0f;
        if (right.sqrMagnitude > EPS)
        {
            right.Normalize();
        }
        float h = Mathf.Sin(t * Mathf.PI * 2f * horizontalFrequency + horizontalPhase) * horizontalAmplitude;
        offset += right * h;
        return offset;
    }

    private Quaternion TargetRotation(float delta, Vector3 destination)
    {
        Quaternion lookRotation = transform.rotation;
        Vector3 toTarget = destination - transform.position;
        if (toTarget.sqrMagnitude > EPS)
        {
            Vector3 forward = toTarget;
            forward.y = 0f;
            if (forward.sqrMagnitude > EPS)
                lookRotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
        }
        float t = 1f - Mathf.Exp(-rotationSpeed * delta);
        return Quaternion.Slerp(transform.rotation, lookRotation, t);
    }

    private Vector3 TargetPosition(float delta, Vector3 destination)
    {
        return Vector3.MoveTowards(transform.position, destination, moveSpeed * delta);
    }
}
