using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    private bool newScene;
    private CharacterManager enemy;
    private CharacterManager player;

    private WaitForSeconds waitForSeconds;
    public Transform CameraObject { get; protected set; }
    public PickableObject[] MercenaryWeapons { get; private set; }

    public static CombatManager Instance { get; private set; }
    [field: SerializeField] public CharacterManager PlayerCombatPrefab { get; private set; }
    [field: SerializeField] public CharacterManager OppositionCombatPrefab { get; private set; }

    public event DuelStateChanged OnDuelStateChanged;
    public delegate void DuelStateChanged(DuelState newState);

    [Header("Parameters")]
    [SerializeField] private float transitionDelay;
    [SerializeField] private Animator sceneTransitionAnimator;
    [field: SerializeField] public DuelState DuelState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        waitForSeconds = new(transitionDelay);
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if(CameraObject == null)
        {
            CameraObject = Camera.main.transform;
        }
    }

    private void Start()
    {
        DuelState = DuelState.None;
        OnDuelStateChanged += HandleDuelStateChanged;
    }

    private void Update()
    {
        if(DuelState != DuelState.OnGoing)
        {
            return;
        }

        if(player == null || enemy == null)
        {
            Debug.Log("No Player Or Enemy");
            return;
        }

        DuelState newState = SwitchDuelState();
        if (newState != DuelState && DuelState == DuelState.OnGoing)
        {
            DuelState = newState;
            OnDuelStateChanged?.Invoke(DuelState);
        }
    }

    private void HandleDuelStateChanged(DuelState state)
    {
        if (state != DuelState.OnGoing && state != DuelState.None)
        {
            StartCoroutine(RewardSystem.Instance.HandleReward(enemy, state));
        }
    }

    public void SetDuelState(DuelState state, CharacterManager npc)
    {
        DuelState = state;
        enemy = npc;
    }

    private DuelState SwitchDuelState()
    {
        if(newScene != true)
        {
            if (player.isDead) return DuelState.Lost;
            if (enemy.isDead) return DuelState.Win;
        }
        if(DuelManager.Instance != null)
        {
            return DuelManager.Instance.SwitchDuelState();
        }
        return DuelState.OnGoing;
    }

    public void AssignPlayer(CharacterManager player)
    {
        PlayerCombatPrefab = player;
    }

    public void StartDuel(CharacterManager player, CharacterManager npc)
    {
        enemy = npc;
        SetPlayer(player);
        DuelState = DuelState.OnGoing;
        CombatMentalState mental = npc.mentalState;

        if(mental == CombatMentalState.High_Alert)
        {
            CharacterCombat combat = enemy.CombatManager;
            int count = combat.enemyPossibleWeapons.Count;

            MercenaryWeapons = new PickableObject[count];
            for(int i = 0; i < count; i++)
            {
                MercenaryWeapons[i] = combat.enemyPossibleWeapons[i].pickableObject.ItemObject.objectPrefab;
            }
        }
        else if(mental == CombatMentalState.Friendly)
        {
            newScene = true;
            OppositionCombatPrefab = npc.CombatManager.CombatCharacter.characterManager;
            StartCoroutine(LoadCombatScene());
        }    
    }

    public void SetPlayer(CharacterManager p)
    {
        player = p;
    }

    private IEnumerator LoadCombatScene()
    {
        sceneTransitionAnimator.SetTrigger("Fade Out");

        yield return waitForSeconds;
        SceneManager.LoadScene("Combat Scene");

        yield return waitForSeconds;
        sceneTransitionAnimator.CrossFade("RectangleGridOut", 0.0f);
    }
}
