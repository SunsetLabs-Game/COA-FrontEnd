using UnityEngine;
using System.Collections.Generic;

public class HoveringVehicleController : MonoBehaviour
{
    public static HoveringVehicleController Instance { get; private set; }

    private List<HoveringVehicles> hoveringVehicles = new();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            return;
        }
        Debug.LogWarning("Multiple instances of HoveringVehicleController detected. Destroying duplicate.");
        Destroy(gameObject);
    }

    private void Update()
    {
        foreach (var vehicle in hoveringVehicles)
        {
            vehicle.HoveringVehicle_Update();
        }
    }

    public void SubscribeToTest(HoveringVehicles vehicle)
    {
        hoveringVehicles.Add(vehicle);
    }
}
