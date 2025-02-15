using UnityEngine;
using UnityEngine.AI;

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

    //AI Components
    public NavMeshPath navMeshPath;
    private Collider[] targetColliders;
    public NavMeshAgent Agent { get; private set; }
    public CharacterManager Target { get; private set; }

    //Player Componets
    public InputManager PlayerInput { get; private set; }

    //Parameters
    public float AngleTarget { get; private set; }
    public float DistanceToTarget { get;  private set; }
    public Vector3 PositionOfTarget { get; private set; }
    public Vector3 DirectionToTarget { get; private set; }

    //Status
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool canRotate;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isSprinting;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool performingAction;

    [Header("Status")]
    public Team currentTeam;
    public CharacterType characterType;
    [SerializeField] private float stopDistance;
    [SerializeField] private float sphereRadius;
    [SerializeField] private LayerMask targetMask;

    [field: Header("State Machine")]
    [SerializeField] private AIState activeState;
    [field: SerializeField] public PursueState Pursue { get; private set; }
    [field: SerializeField] public CombatState Combat { get; private set; }
    [field: SerializeField] public AttackState Attack { get; private set; }
    

    private void Awake()
    {
        Anim = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();

        AnimatorManagaer = GetComponent<CharacterAnim>();
        CombatManager = GetComponent<CharacterCombat>();
        StatsManager = GetComponent<CharacterStatistic>();
        MovementManager = GetComponent<CharacterMovement>();

        //Character Type Based Components
        if(characterType == CharacterType.Player)
        {
            PlayerInput = GetComponent<InputManager>();
            if(PlayerInput == null)
            {
                PlayerInput = gameObject.AddComponent<InputManager>();
            }
        }

        if(characterType == CharacterType.AI)
        {
            Agent = GetComponentInChildren<NavMeshAgent>();
            if(Agent == null)
            {
                GameObject aiObject = new GameObject();
                aiObject.transform.SetParent(transform);
                Agent = aiObject.AddComponent<NavMeshAgent>();

                Agent.stoppingDistance = stopDistance;
                aiObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
    }

    private void Start()
    {
        InitializeStates();
        StatsManager.ResetStats();
    }

    private void Update()
    {
        if(isDead)
        {
            return;
        }
        float delta = Time.deltaTime;

        isGrounded = Controller.isGrounded;

        if(characterType == CharacterType.Player)
        {
            PlayerInput.InputManager_Update();
        }

        if(characterType == CharacterType.AI)
        {
            HandleStateChange();
        }
        AnimatorManagaer.SetAnimatorBool(Anim);

        SetTargetDetails();
        CombatManager.Combat_Update(delta);
        MovementManager.CharacterMovement_Update(delta);
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
        Agent.transform.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        CheckIfMoving();
    }

    private void CheckIfMoving()
    {
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
        return false;
    }

    private void InitializeStates()
    {
        if(characterType != CharacterType.AI)
        {
            return;
        }

        Pursue = Instantiate(Pursue);
        Combat = Instantiate(Combat);
        Attack = Instantiate(Attack);

        Combat.Initialize();
        targetColliders = new Collider[10];
        activeState = Pursue.SwitchState(this, Pursue);
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
            if(potentialTarget.currentTeam != currentTeam)
            {
                Target = potentialTarget;
            }
        }
    }

    private void SetTargetDetails()
    {
        if (characterType != CharacterType.AI)
        {
            return;
        }

        if (Target == null)
        {
            FindTarget();
            return;
        }

        PositionOfTarget = Target.transform.position;
        DirectionToTarget = transform.position - PositionOfTarget;
        DistanceToTarget = DirectionToTarget.magnitude;
    }
}
