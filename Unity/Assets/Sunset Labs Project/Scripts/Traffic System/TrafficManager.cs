using UnityEngine;
using System.Collections.Generic;

public class TrafficManager : MonoBehaviour
{
    private Camera mainCamera;
    private Transform cameraTransform;

    private float cullDistanceSQ;
    private Vector3 lastCameraPosition;
    private List<TrafficVehicle> activeVehicles = new();

    [Header("Spawn")]
    [SerializeField] private int index;
    [SerializeField] private TrafficVehicle[] vehicles;

    [Header("Parameters")]
    [SerializeField] private Transform spawnParent;
    [SerializeField] private float cullDistance = 100f;
    [SerializeField] private float distanceThreshold = 20.0f;

    private void Awake()
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
        cullDistanceSQ = MathPhysics_Helper.Square(cullDistance);
        activeVehicles = new(spawnParent.GetComponentsInChildren<TrafficVehicle>());
    }

    private void Start()
    {
        activeVehicles.ForEach(x => PrepareVehicles(x));
    }

    private void Update()
    {
        if (Vector3.Distance(lastCameraPosition, cameraTransform.position) > distanceThreshold)
        {
            CullDistantVehicles();
        }
        foreach (var vehicle in activeVehicles)
        {
            vehicle.TrafficVehicle_Update();
        }
    }

    public void SpawnVehicle()
    {
        TrafficPathController[] pathController = GetComponentsInChildren<TrafficPathController>();
        foreach(var control in pathController)
        {
            if(index >= control.WaypointCount)
            {
                continue;
            }
            Vector3 spawnPosition = control.WayPointNodes[index].NodePosition;
            TrafficVehicle vehicle = vehicles[Random.Range(0, vehicles.Length)];

            TrafficVehicle spawn = Instantiate(vehicle, spawnParent);
            spawnPosition = spawnParent.InverseTransformPoint(spawnPosition);
            spawn.transform.SetLocalPositionAndRotation(spawnPosition, Quaternion.identity);
            spawn.SetController(control);
            activeVehicles.Add(spawn);
        }
    }

    public void ClearSpawn()
    {
        foreach (var item in activeVehicles)
        {
            DestroyImmediate(item.gameObject);
        }
        activeVehicles.Clear();
    }

    private void PrepareVehicles(TrafficVehicle vehicle)
    {
        vehicle.SetPathIstructions();     
        vehicle.canUpdate = true;
    }

    private void CullDistantVehicles()
    {
        for (int i = activeVehicles.Count - 1; i >= 0; i--)
        {
            TrafficVehicle vehicle = activeVehicles[i];
            Transform vehicleTransform = vehicle.transform;
            if (vehicle == null)
            {
                activeVehicles.RemoveAt(i);
                continue;
            }

            float sqrDistance = (cameraTransform.position - vehicleTransform.position).sqrMagnitude;
            if (sqrDistance > cullDistanceSQ)
            {
                activeVehicles[i].gameObject.SetActive(false);
            }
        }
    }
}