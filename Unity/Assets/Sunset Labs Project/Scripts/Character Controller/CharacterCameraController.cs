using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class CharacterCameraController : MonoBehaviour
{
    private bool hasUpdatedCameraProperties;
    private CharacterManager characterManager;

    private CinemachinePanTilt panTilt;
    private CinemachineInputAxisController inputAxisController;

    // cinemachine
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private const float _threshold = 0.01f;

    [Header("Sensitivity")]
    [SerializeField] private float normalSensivity = 6.75f;
    [SerializeField] private float lockedInSensitivity = 5.5f;

    [field: Header("Cinemachine Cameras and Others")]
    [field: SerializeField] public Image CrossHairImg { get; private set; }
    [field: SerializeField] public CinemachineCamera MainVirtualCamera { get; private set; }
    [field: SerializeField] public CinemachineCamera ShooterVirtualCamera { get; private set; }

    [Header("Cinemachine Properties")]
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField] private float TopClamp = 30.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField] private float BottomClamp = -10.0f;
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField] private Transform CinemachineCameraTarget;

    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();

        CrossHairImg = GameObjectTool.GetComponentByName<Image>("Crosshair");
        MainVirtualCamera = GameObjectTool.GetComponentByName<CinemachineCamera>("Main Virtual Camera");
        ShooterVirtualCamera = GameObjectTool.GetComponentByName<CinemachineCamera>("Gun Virtual Camera");

        panTilt = ShooterVirtualCamera.GetComponent<CinemachinePanTilt>();
        inputAxisController = ShooterVirtualCamera.GetComponent<CinemachineInputAxisController>();
    }

    public void SetCameraTarget(Transform target)
    {
        CinemachineCameraTarget = target;
    }

    public void CameraRotation()
    {
        InputManager input = characterManager.PlayerInput;
        if (input.cameraInput.sqrMagnitude >= _threshold)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = input.IsCurrentDeviceMouse() ? 0.35f : Time.deltaTime;

            float sensitivity = Sensitivity();
            cinemachineTargetYaw += input.cameraInput.x * deltaTimeMultiplier * sensitivity;
            cinemachineTargetPitch += input.cameraInput.y * deltaTimeMultiplier * sensitivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
    }

    public void EnableShooterGraphics(bool status, bool lockedIn, float delta)
    {
        SetCameraProperties(status, lockedIn, delta);
        if(CrossHairImg != null) CrossHairImg.gameObject.SetActive(status);
        if (MainVirtualCamera != null) MainVirtualCamera.gameObject.SetActive(!status);
        if (ShooterVirtualCamera != null) ShooterVirtualCamera.gameObject.SetActive(status);
        hasUpdatedCameraProperties = lockedIn;
    }

    private void SetCameraProperties(bool status, bool lockedIn, float delta)
    {
        if(hasUpdatedCameraProperties == lockedIn || status != true)
        {
            return;
        }

        foreach(var controller in inputAxisController.Controllers)
        {
            float multiplier = (controller.Name == "Look X (Pan)") ? 1 : -1;
            float sensitivity = ((lockedIn && status) ? lockedInSensitivity : normalSensivity);
            controller.Input.Gain = sensitivity * multiplier;
        }
        float range = (lockedIn) ? 90f : 180f;
        float fieldOfView = (status && lockedIn) ? 45f : 60f;

        panTilt.PanAxis.Wrap = (lockedIn != true); //If Locked In Dont Wrap Pan Axis
        panTilt.PanAxis.Range = new Vector2(-range, range);
        ShooterVirtualCamera.Lens.FieldOfView = fieldOfView;
    }

    private float Sensitivity()
    {
        if(characterManager.isLockedIn)
        {
            if(characterManager.CombatManager.HasGun())
            {
                return 0.65f;
            }
            return lockedInSensitivity;
        }
        return normalSensivity;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
