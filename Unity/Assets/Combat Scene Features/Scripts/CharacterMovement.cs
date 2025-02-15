using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    private CharacterManager characterManager;

    private Vector3 moveDirection;
    private Transform cameraObject;

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
    [Tooltip("Force at which character begins to fall")][field: SerializeField] public float fallStartVelocity { get; protected set; } = -5.0f;

    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
    }

    private void Start()
    {
        cameraObject = Camera.main.transform;
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
        HandleMovement(delta);
        HandleRotation(delta);
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
                verticalVelocity.y = fallStartVelocity;
            }

            fallingTimer += delta;
            characterManager.Anim.SetFloat(fallingTimerHash, fallingTimer);
            verticalVelocity.y += gravityForce * delta;
        }
        characterManager.Controller.Move(verticalVelocity * delta);
    }

    private void HandleRotation(float delta)
    {
        float yawCamera = cameraObject.rotation.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, yawCamera, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleMovement(float delta)
    {
        if (characterManager.isGrounded != true)
        {
            return;
        }

        float verticalInput = characterManager.PlayerInput.verticalMoveInput;
        float horizontalInput = characterManager.PlayerInput.horizontalMoveInput;
        CharacterController characterController = characterManager.Controller;

        moveDirection = cameraObject.forward * verticalInput;
        moveDirection += cameraObject.right * horizontalInput;

        moveDirection.Normalize();
        moveDirection.y = 0.0f;

        if (characterManager.isSprinting)
        {
            characterController.Move(sprintingSpeed * moveDirection * delta);
            characterManager.StatsManager.ReduceEndurance(sprintEnduranceCost);
        }
        else
        {
            characterController.Move(walkingSpeed * moveDirection * delta);
        }
        characterManager.AnimatorManagaer.SetBlendTreeParameter(verticalInput, horizontalInput, characterManager.isSprinting, delta);
    }

    public void MoveToDestination(float speed, Vector3 destination)
    {
        NavMeshAgent agent = characterManager.Agent;
        characterManager.navMeshPath ??= new NavMeshPath();
        if (characterManager.navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            characterManager.navMeshPath.ClearCorners();
        }
        if (agent.CalculatePath(destination, characterManager.navMeshPath))
        {
            agent.SetPath(characterManager.navMeshPath);
        }

        if (!NavMesh.SamplePosition(destination, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            return;
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
