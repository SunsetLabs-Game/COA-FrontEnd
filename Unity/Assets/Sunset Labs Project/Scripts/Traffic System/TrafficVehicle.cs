using UnityEngine;
using UnityEngine.Pool;

public class TrafficVehicle : MonoBehaviour
{
    private int nodeIndex = 0;
    private float parkDuration;
    private float maximumParkDuration;

    private float minimumDistanceSQ;
    private const float EPS = 0.0001f;

    private Transform currentNode;

    private bool hasPath;
    private bool isParked;
    public bool canUpdate;
    private bool hasSetParkDuration;
    public ObjectPool<TrafficVehicle> MySpawnPool { get; private set; }

    [Header("Status")]
    [SerializeField] private WayPointPath assignedPath = new();
    [SerializeField] private TrafficPathController currentPathController;
    [SerializeField] private DriverExperience driverExperience = DriverExperience.Mid;

    [Header("Regular Parameters")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float minimumDistance = 1f;
    [SerializeField] private Transform[] wheelTransforms;
    [SerializeField] private BoundFloat parkDurationBound;

    private void Awake()
    {
        minimumDistanceSQ = MathPhysics_Helper.Square(minimumDistance);
    }

    public void SetController(TrafficPathController controller)
    {
        currentPathController = controller;
    }

    public void TrafficVehicle_Update()
    {
        if(canUpdate != true || assignedPath == null)
        {
            return;
        }
        float delta = Time.deltaTime;
        bool hasCompletedPath = (hasPath != true) || (assignedPath == null) || (nodeIndex >= assignedPath.PathNodeCount);
        if(hasCompletedPath)
        {
            HasReachedDestination(delta);
            return;
        }
        currentNode = assignedPath.PathNodes[nodeIndex].transform;
        HandleMovement(delta, currentNode.position);

        float sqrDistance = (currentNode.position - transform.position).sqrMagnitude;
        if(sqrDistance < minimumDistanceSQ)
        {
            nodeIndex++;
        }
    }

    private void HandleMovement(float delta, Vector3 destination)
    {
        if(isParked)
        {
            return;
        }

        if(hasPath != true)
        {
            SetPathIstructions();
            return;
        }
        transform.SetPositionAndRotation(TargetPosition(delta, destination), TargetRotation(delta, destination));
        SimulateWheelRotation(delta);
    }

    private void HasReachedDestination(float delta)
    {
        if (hasSetParkDuration != true)
        {
            hasSetParkDuration = true;
            maximumParkDuration = Random.Range(parkDurationBound.minValue, parkDurationBound.maxValue);
        }

        isParked = true;
        parkDuration += delta;
        if (parkDuration > maximumParkDuration)
        {
            nodeIndex = 0;
            isParked = false;
            SetPathIstructions();
            hasSetParkDuration = false;
        }
    }

    private void SimulateWheelRotation(float delta)
    {
        foreach(var wheel in wheelTransforms)
        {
            wheel.Rotate(Vector3.right.normalized * moveSpeed * 1f * delta);
        }
    }

    private Vector3 TargetPosition(float delta, Vector3 destination)
    {
        return Vector3.MoveTowards(transform.position, destination, delta * moveSpeed);
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
            {
                lookRotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
            } 
        }
        float t = 1f - Mathf.Exp(-rotationSpeed * delta);
        return Quaternion.Slerp(transform.rotation, lookRotation, t);
    }

    #region Path Calculations
    private void ClearPath()
    {
        hasPath = false;
        assignedPath?.Clear();
    }

    public void SetPathIstructions()
    {
        ClearPath();
        TrafficNode startNode = null;
        TrafficNode finalDestination = null;
        if(assignedPath != null)
        {
            TrafficNode exclude = assignedPath.FinalDestinationNode;
            finalDestination = currentPathController.GetNode(transform, exclude, out startNode);
        }
        
        if(finalDestination == null)
        {
            return;
        }
        assignedPath.RefreshPath(driverExperience, startNode, finalDestination);
        hasPath = assignedPath.HasPath;
    }
    #endregion
}