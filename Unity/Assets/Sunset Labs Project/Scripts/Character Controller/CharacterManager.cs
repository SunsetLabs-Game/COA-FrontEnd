using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public enum CharacterType
{
    AI,
    Player
}

public enum Team
{
    Red,
    Blue
}

public class CharacterManager : MonoBehaviour
{
    //Unity Components
    public Animator Anim { get; private set; }
    public CharacterController Controller { get; private set; }

    //Created Components
    public CharacterAnim AnimatorManagaer { get; private set; }
    public CharacterCombat CombatManager { get; private set; }
    public CharacterStatistic StatsManager { get; private set; }
    public CharacterMovement MovementManager { get; private set; }
    public CharacterAnimatorRigController RigController { get; private set; }

    //AI Components
    public UnityEvent Assignment;
    public NavMeshPath navMeshPath;
    private Collider[] targetColliders;
    public NavMeshAgent Agent { get; private set; }
    public CharacterManager Target { get; private set; }

    //Player Componets
    public InputManager PlayerInput { get; private set; }
    public CharacterCameraController CameraController { get; private set; }
    public CharacterInteractionScript InteractionScript { get; private set; }

    //Parameters
    public float AngleTarget { get; private set; }
    public float DistanceToTarget { get;  private set; }
    public Vector3 PositionOfTarget { get; private set; }
    public Vector3 DirectionToTarget { get; private set; }
    public UnityEngine.Pool.ObjectPool<CharacterManager> mySpawnPool;

    //Status
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool dontMove;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isTalking;
    [HideInInspector] public bool canRotate;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isLockedIn;
    [HideInInspector] public bool isSprinting;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool performingAction;

    [Header("Status")]
    public bool canUpdate;
    public bool combatMode;
    public bool findTarget;
    public bool hasReached;
    public bool hasAssignment;
    public Team currentTeam;
    public CharacterType characterType;
    [SerializeField] private float stopDistance;
    [SerializeField] private float sphereRadius;
    public CombatMentalState mentalState = CombatMentalState.Friendly;

    [Header("Properties")]
    [SerializeField] private LayerMask targetMask;
    [field: SerializeField] public Transform CameraTarget { get; private set; }
    [field: SerializeField] public Sprite CharacterImage { get; private set; }

    [field: Header("State Machine")]
    [SerializeField] private AIState activeState;
    [field: SerializeField] public PatrolState Patrol { get; private set; }
    [field: SerializeField] public PursueState Pursue { get; private set; }
    [field: SerializeField] public CombatState Combat { get; private set; }
    [field: SerializeField] public AttackState Attack { get; private set; }
    
    private void Awake()
    {
        Anim = GetComponent<Animator>();
        Assignment.RemoveAllListeners();
        Controller = GetComponent<CharacterController>();

        AnimatorManagaer = GetComponent<CharacterAnim>();
        CombatManager = GetComponent<CharacterCombat>();
        StatsManager = GetComponent<CharacterStatistic>();

        MovementManager = GetComponent<CharacterMovement>();
        RigController = GetComponentInChildren<CharacterAnimatorRigController>();
    }

    private void Start()
    {
        if (characterType == CharacterType.Player)
        {
            PlayerInput = GetComponent<InputManager>();
            if (PlayerInput == null)
            {
                PlayerInput = gameObject.AddComponent<InputManager>();
                InteractionScript = GetComponent<CharacterInteractionScript>();
                CameraController = gameObject.AddComponent<CharacterCameraController>();

                CameraController.SetCameraTarget(CameraTarget);
            }
            if (CharacterInventoryManager.Instance != null) CharacterInventoryManager.Instance.SetCharacterManager(this);
        }
        else
        {
            Agent = GetComponentInChildren<NavMeshAgent>();
            if (Agent == null)
            {
                GameObject aiObject = new("NavMesh Agent");
                aiObject.transform.SetParent(transform);
                Agent = aiObject.AddComponent<NavMeshAgent>();

                Agent.stoppingDistance = stopDistance;
                Agent.obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
                aiObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
            InitializeStates();
            gameObject.AddComponent<DialogueTrigger>();
        }
        StatsManager.ResetStats();
        RigController.SetRigs(false);
    }

    //Character Type Based Components
    public void SetCharacterType(CharacterType type)
    {
        characterType = type;
    }

    private void Update()
    {
        if(isDead)
        {
            return;
        }

        if(characterType == CharacterType.Player)
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.dialogueIsPlaying == true)
            {
                return;
            }
        }

        if (canUpdate != true && characterType == CharacterType.AI)
        {
            return;
        }

        float delta = Time.deltaTime;
        isGrounded = Controller.isGrounded;

        AnimatorManagaer.SetAnimatorBool(Anim);
        if (characterType == CharacterType.Player)
        {
            PlayerInput.InputManager_Update();
            InteractionScript.InteractionUpdate();
            isLockedIn = CombatManager.HasGun() && PlayerInput.lockedInput;
        }

        if (characterType == CharacterType.AI)
        {
            HandleStateChange();
        }
        SetTargetDetails();
        CombatManager.Combat_Update(delta);
        MovementManager.CharacterMovement_Update(delta);
        if (RigController != null)
        {
            RigController.CharacterAnimationRig_Updater(delta);
        }
    }

    private void LateUpdate()
    {
        if (characterType == CharacterType.Player)
        {
            PlayerInput.ResetInput();
            //CameraController.CameraRotation();
        }
    }

    private void HandleStateChange()
    {
        if (activeState != null)
        {
            var nextState = activeState.StateUpdater(this);

            if (nextState != null)
            {
                activeState = nextState;
            }
        }
        CheckIfMoving();
        Agent.transform.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private void CheckIfMoving()
    {
        if(dontMove == true)
        {
            Agent.enabled = false;
            isMoving = false;
            return;
        }

        if (activeState == Combat)
        {
            return;
        }

        if (Agent.enabled == false)
        {
            isMoving = false;
            return;
        }
        isMoving = SetMoving();
    }

    private bool SetMoving()
    {
        if (DistanceToTarget > Agent.stoppingDistance)
        {
            return true;
        }

        if(activeState == Patrol)
        {
            return (Patrol.patrolMode == PatrolMode.Walk);
        }
        return false;
    }

    private void InitializeStates()
    {
        if(characterType == CharacterType.Player)
        {
            return;
        }

        Patrol = Instantiate(Patrol);
        Pursue = Instantiate(Pursue);
        Combat = Instantiate(Combat);
        Attack = Instantiate(Attack);

        Combat.Initialize();
        Patrol.Initialize();
        targetColliders = new Collider[10];
        activeState = Pursue.SwitchState(this, Patrol);
    }

    private void FindTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, sphereRadius, targetColliders, targetMask);

        for(int i = 0; i < count; i++)
        {
            if (targetColliders[i] == null)
            {
                continue;
            }

            CharacterManager potentialTarget = targetColliders[i].GetComponentInParent<CharacterManager>();
            if(potentialTarget != null && potentialTarget.currentTeam != currentTeam)
            {
                SetTarget(potentialTarget);
            }
        }
    }

    public void SetTarget(CharacterManager target)
    {
        Target = target;
    }

    private void SetTargetDetails()
    {
        if (characterType != CharacterType.AI)
        {
            return;
        }

        if (Target == null)
        {
            if (findTarget) { FindTarget(); }
            return;
        }

        PositionOfTarget = Target.transform.position;
        DirectionToTarget = transform.position - PositionOfTarget;
        DistanceToTarget = DirectionToTarget.magnitude;
    }

    public void PatrolParametersSet(Vector3 patrolDestination)
    {
        if (Target != null)
        {
            return;
        }
        DirectionToTarget = transform.position - patrolDestination;
        DistanceToTarget = DirectionToTarget.magnitude;
    }
}
