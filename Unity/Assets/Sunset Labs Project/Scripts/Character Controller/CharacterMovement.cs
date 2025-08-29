using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    private CharacterManager characterManager;

    private Vector3 moveDirection;
    public Transform CameraObject { get; protected set; }

    //Gravity
    private int fallingTimerHash;
    private Vector3 verticalVelocity;
    public float fallingTimer { get; protected set; }

    [Header("Locomotion Parameters")]
    public float agentSpeed;
    public float walkingSpeed;
    public float rotationSpeed;
    public float sprintingSpeed;
    public float sprintEnduranceCost;

    [field: Header("Gravity Stats")]
    [SerializeField] protected float jumpHeight = 4.0f;
    [SerializeField] protected float gravityForce = -30.0f;
    [field: SerializeField] public bool fallingVelocitySet { get; protected set; } = false;
    [Tooltip("Force at which character is sticking to the ground")][SerializeField] protected float groundedForce = -20f;
    [Tooltip("Force at which character begins to fall")][field: SerializeField] public float FallStartVelocity { get; protected set; } = -5.0f;

    private void Awake()
    {
        CameraObject = Camera.main.transform;
        characterManager = GetComponent<CharacterManager>();
    }

    private void Start()
    {
        fallingTimerHash = Animator.StringToHash("inAirTimer");
    }

    public void CharacterMovement_Update(float delta)
    {
        HandleGravity(delta);
        if (characterManager.characterType == CharacterType.Player)
        {
            PlayerMovement(delta);
        }
    }

    public void PlayerMovement(float delta)
    {
        CharacterCombat combat = characterManager.CombatManager;
        bool isLockedIn = (characterManager.isLockedIn && combat.HasGun());

        HandleMovement(delta, isLockedIn);
        HandleRotation(delta, isLockedIn);
    }

    private void HandleGravity(float delta)
    {
        if (characterManager.isGrounded)
        {
            if (verticalVelocity.y < 0.0f)
            {
                fallingTimer = 0.0f;
                fallingVelocitySet = false;
                verticalVelocity.y = groundedForce;
            }
        }

        else if (characterManager.isGrounded != true)
        {
            if (characterManager.isJumping != true && fallingVelocitySet != true)
            {
                fallingVelocitySet = true;
                verticalVelocity.y = FallStartVelocity;
            }

            fallingTimer += delta;
            characterManager.Anim.SetFloat(fallingTimerHash, fallingTimer);
            verticalVelocity.y += gravityForce * delta;
        }
        characterManager.Controller.Move(verticalVelocity * delta);
    }

    private void HandleRotation(float delta, bool isLockedIn)
    {
        transform.rotation = SetTargetRotation(delta, isLockedIn);
    }

    private Quaternion SetTargetRotation(float delta, bool isLockedIn)
    {
        Quaternion targetRotation = Quaternion.identity;
        bool hasGun = characterManager.CombatManager.HasGun();

        if(hasGun != true)
        {
            Vector3 rotationDirection = CameraObject.forward * characterManager.PlayerInput.verticalMoveInput;
            rotationDirection += CameraObject.right * characterManager.PlayerInput.horizontalMoveInput;

            rotationDirection.Normalize();
            rotationDirection.y = 0.0f;
            if (rotationDirection == Vector3.zero)
            {
                rotationDirection = transform.forward;
            }
            targetRotation = Quaternion.LookRotation(rotationDirection);
        }
        else
        {
            float yawCamera = CameraObject.rotation.eulerAngles.y;
            targetRotation = Quaternion.Euler(0f, yawCamera, 0f);
        }
        return Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * delta);
    }

    private void HandleMovement(float delta, bool isLockedIn)
    {
        if (characterManager.isGrounded != true)
        {
            return;
        }

        InputManager input = characterManager.PlayerInput;

        float verticalInput = input.verticalMoveInput;
        float horizontalInput = input.horizontalMoveInput;
        CharacterController characterController = characterManager.Controller;

        moveDirection = CameraObject.forward * verticalInput;
        moveDirection += CameraObject.right * horizontalInput;

        moveDirection.Normalize();
        moveDirection.y = 0.0f;

        if (characterManager.isSprinting)
        {
            characterController.Move(delta * sprintingSpeed * moveDirection);
            characterManager.StatsManager.ReduceEndurance(sprintEnduranceCost);
        }
        else
        {
            characterController.Move(delta * walkingSpeed * moveDirection);
        }

        float horizontalKey = (isLockedIn) ? horizontalInput : 0.0f;
        float verticalKey = (isLockedIn) ? verticalInput : input.moveAmount;
        characterManager.AnimatorManagaer.SetBlendTreeParameter(verticalKey, horizontalKey, characterManager.isSprinting, delta);
    }

    public void MoveToDestination(float speed, Vector3 destination)
    {
        if (characterManager.dontMove)
        {
            return;
        }

        NavMeshAgent agent = characterManager.Agent;
        if(characterManager.mentalState == CombatMentalState.High_Alert)
        {
            MercenaryMovement(speed, destination, agent);
            return;
        }
        int walkableIndex = NavMesh.GetAreaFromName("Walkable");
        int areaMask = (walkableIndex >= 0) ? (1 << walkableIndex) : NavMesh.AllAreas;
        NPCMovement(speed, areaMask, destination, agent);
    }

    private void MercenaryMovement(float speed, Vector3 destination, NavMeshAgent agent)
    {
        characterManager.navMeshPath ??= new NavMeshPath();
        if (characterManager.navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            characterManager.navMeshPath.ClearCorners();
        }
        if (agent.CalculatePath(destination, characterManager.navMeshPath))
        {
            agent.SetPath(characterManager.navMeshPath);
        }

        if (!NavMesh.SamplePosition(destination, out _, 1.0f, NavMesh.AllAreas))
        {
            return;
        }
        Vector3 moveDirection = agent.desiredVelocity;
        characterManager.Controller.Move(speed * Time.deltaTime * moveDirection);
    }

    private void NPCMovement(float speed, int areaMask, Vector3 destination, NavMeshAgent agent)
    {
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(agent.transform.position, out var sampleHit, 2.0f, areaMask))
            {
                agent.Warp(sampleHit.position);
                agent.enabled = true;
            }
            else if (NavMesh.SamplePosition(destination, out var destHit, 2.0f, areaMask))
            {
                agent.Warp(destHit.position);
                agent.enabled = true;
            }
            return;
        }

        characterManager.navMeshPath ??= new NavMeshPath();
        if (characterManager.navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            characterManager.navMeshPath.ClearCorners();
        }
        if (!NavMesh.SamplePosition(destination, out var destinationHit, 1.0f, areaMask))
        {
            return;
        }
        if (agent.CalculatePath(destinationHit.position, characterManager.navMeshPath))
        {
            agent.SetPath(characterManager.navMeshPath);
        }
        Vector3 moveDirection = agent.desiredVelocity;
        characterManager.Controller.Move(speed * Time.deltaTime * moveDirection);
    }

    public void HandleRotationWhileAttacking(CharacterManager characterManager)
    {
        if (characterManager.Target == null)
        {
            return;
        }

        if (characterManager.canRotate != true)
        {
            return;
        }
        Vector3 targetDirection = characterManager.PositionOfTarget - characterManager.transform.position;
        targetDirection.y = 0.0f;
        targetDirection.Normalize();

        if (characterManager.DirectionToTarget == Vector3.zero)
        {
            targetDirection = characterManager.transform.forward;
        }
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        characterManager.transform.rotation = Quaternion.Slerp(characterManager.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void RotateTowardsTarget()
    {
        if (characterManager.isMoving == true)
        {
            characterManager.transform.rotation = characterManager.Agent.transform.rotation;
        }
    }
}
