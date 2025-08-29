using UnityEngine;
using Unity.Cinemachine;

public class LoadCharacter : MonoBehaviour
{
    private int selectedIndex;
    public CharacterManager SpawnedCharacter { get; private set; }

    [Header("Parameters")]  
    [Tooltip("Transform reference for the position where the character will be spawned")]
    public Transform spawnPoint;
    [SerializeField] private Transform aimObject;
    [SerializeField] private Transform cameraAimObject;

    [Header("UI Elements")]
    [SerializeField] private PlayerUI playerUI;

    [Header("Camera Objects")]
    [SerializeField] private CinemachineCamera gunCamera;
    [SerializeField] private CinemachineCamera thirdPersonCamera;

    [Header("Instantiated Objects")]
    [Tooltip(" Array of drone prefabs that can be instantiated.")]
    public PlayerCompanion[] companions;
    [Tooltip("Array of character prefabs that can be instantiated.")]
    [field: SerializeField] public CharacterData[] CharacterDatas { get; private set; }

    private void Awake()
    {
        CharacterDatas = Resources.LoadAll<CharacterData>("Character Data");
        CreateCharacter();
    }

    private void CreateCharacter()
    {
        selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        if (selectedIndex < 0 || selectedIndex >= CharacterDatas.Length)
        {
            Debug.LogError("�ndice de personaje seleccionado est� fuera de rango. Verifica los prefabs asignados en el inspector.");
            return;
        }
        CharacterManager spawnedCharacter = Instantiate(CharacterDatas[selectedIndex].PlayableCharacter, spawnPoint);

        spawnedCharacter.name = CharacterDatas[selectedIndex].characterName;
        spawnedCharacter.SetCharacterType(CharacterType.Player);
        CreateDroneObject(spawnedCharacter, out PlayerCompanion companion);

        if (NPCController.Instance != null)
        {
            NPCController.Instance.SetPlayerAndCompanion(spawnedCharacter, companion);
        }
        SetCharacterParameters(spawnedCharacter);
        spawnedCharacter.CombatManager.SetDuellingCharacter();
        SetMiniMapAndCameraProperties(spawnedCharacter.CameraTarget);
    }

    private void CreateDroneObject(CharacterManager player, out PlayerCompanion companion)
    {
        int droneIndex = Random.Range(0, companions.Length);
        companion = Instantiate(companions[droneIndex], player.transform.position + new Vector3(3, 3, -3), Quaternion.identity);

        companion.SetFollowCharacter(player);
        companion.transform.SetParent(spawnPoint);
    }

    private void SetMiniMapAndCameraProperties(Transform playerTransform)
    {
        MinimapController minimapController = FindObjectOfType<MinimapController>();

        if (minimapController != null)
        {
            minimapController.player = playerTransform;
            SetCameraProperty(gunCamera, playerTransform);
            SetCameraProperty(thirdPersonCamera, playerTransform);
        }
    }

    private void SetCharacterParameters(CharacterManager player)
    {
        if(player == null)
        {
            return;
        }

        playerUI.SetParameters(player);
        player.currentTeam = Team.Blue;
        player.RigController.SetAimTarget(aimObject);
        player.CombatManager.SetCrossHair(cameraAimObject);
    }

    private void SetCameraProperty(CinemachineVirtualCameraBase camera, Transform playerTransform)
    {
        camera.Follow = playerTransform;
        camera.LookAt = playerTransform;
    }
}
