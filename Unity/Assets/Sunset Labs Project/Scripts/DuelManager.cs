using UnityEngine;
using Unity.Cinemachine;

public class DuelManager : MonoBehaviour
{
    public static DuelManager Instance;

    private WeaponManager weapon;

    private CharacterManager enemy;
    private CharacterManager player;

    private CombatManager combatManager;
    public CharacterManager Player => player;

    [Header("Character Objects")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Camera Objects")]
    [SerializeField] private Transform cameraAimObject;
    [SerializeField] private CinemachineCamera gunCamera;
    [SerializeField] private CinemachineCamera thirdPersonCamera;

    [Header("Weapon Objects")]
    [SerializeField] private WeaponManager[] weaponManagers;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        combatManager = CombatManager.Instance;

        SetObject(enemySpawnPoint, combatManager.OppositionCombatPrefab, CharacterType.AI);
        SetObject(playerSpawnPoint, combatManager.PlayerCombatPrefab, CharacterType.Player);

        enemy.SetTarget(player);
        uiManager.PrepareTimer();
        combatManager.SetDuelState(DuelState.OnGoing, enemy);
        SetCameraProperty(gunCamera, player.CameraTarget);
        SetCameraProperty(thirdPersonCamera, player.CameraTarget);
    }

    private void Update()
    {
        if (combatManager.DuelState == DuelState.OnGoing)
        {
            uiManager.HandleCountdown(Time.deltaTime);
        }
    }

    public DuelState SwitchDuelState()
    {
        if (player.isDead) return DuelState.Lost;
        if (enemy.isDead) return DuelState.Win;
        if (uiManager.timeUp) return DuelState.Draw;
        return DuelState.OnGoing;
    }

    private void SetCameraProperty(CinemachineVirtualCameraBase camera, Transform playerTransform)
    {
        camera.Follow = playerTransform;
        camera.LookAt = playerTransform;
    }

    private void PrepareWeapon(CharacterManager characterManager)
    {
        characterManager.CombatManager.CreateEnemyWeapons(weapon, weaponManagers);
    }

    private void SetObject(Transform parent, CharacterManager character, CharacterType characterType)
    {
        CharacterManager newCharacter = Instantiate(character, parent);

        newCharacter.combatMode = true;
        newCharacter.SetCharacterType(characterType);

        if (characterType == CharacterType.AI)
        {
            enemy = newCharacter;
            PrepareWeapon(enemy);

            enemy.canUpdate = true;
            enemy.currentTeam = Team.Red;
        }
        else
        {
            player = newCharacter;
            CombatManager.Instance.SetPlayer(player);
            player.currentTeam = Team.Blue;
            player.CombatManager.SetCrossHair(cameraAimObject);
        }
        uiManager.PrepareDuelingCharacter(newCharacter);
    }
}