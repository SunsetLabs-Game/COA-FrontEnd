using UnityEngine;

public class PlayerCompanion : MonoBehaviour
{
    //Parameters
    private Vector3 currentOffset;
    private float hoverTimerOffset;
    private float offsetUpdateTime;

    private PlayerCompanion_Combat combat;
    public bool CombatMode { get; private set; }
    public Transform FollowTarget { get; private set; }
    public CharacterManager FollowCharacter { get; private set; }

    [Header("Parameters")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Status")]
    public bool hasViolentTarget;
    public CombatMentalState mentalState = CombatMentalState.Friendly;

    [Header("Offsets")]
    [SerializeField] private BoundFloat heightOffset;
    [SerializeField] private BoundFloat distanceOffset;
    [Range(0.0f, 5.0f)][SerializeField] private float followOffset;
    [Range(0.0f, 5.0f)][SerializeField] private float desiredDistance;

    [Header("Oscillations")]
    [SerializeField] private float offsetUpdateInterval;
    [Range(0.0f, 2.0f)][SerializeField] private float hoverSpeed;
    [Range(0.0f, 0.2f)][SerializeField] private float hoverHeight;

    [Header("Random Offset")]
    [Range(0.0f, 5.0f)][SerializeField] private float offsetYRadius;
    [Range(0.0f, 5.0f)][SerializeField] private float offsetXZRadius;

    private void Awake()
    {
        combat = GetComponent<PlayerCompanion_Combat>();
    }

    private void Start()
    {
        currentOffset = GenerateRandomOffset();

        hoverTimerOffset = Random.Range(0, Mathf.PI * 2);
        offsetUpdateTime = Time.time + offsetUpdateInterval;
    }

    private void Update()
    {
        FollowTarget = (hasViolentTarget && combat.EnemyTarget != null) ? combat.EnemyTarget.transform : FollowCharacter.transform;
        if (FollowCharacter == null || DialogueManager.Instance.dialogueIsPlaying)
        {
            return;
        }
        float delta = Time.deltaTime;
        CombatMode = (combat.target != null);

        HandleMovement(delta);
        combat.Combat_Update(delta);
    }

    public void SetFollowCharacter(CharacterManager character)
    {
        FollowCharacter = character;
    }

    private void HandleMovement(float delta)
    {
        HandleCompanionMovement(Time.time, delta);

        Quaternion targetRotation = Quaternion.LookRotation(RotateDirection());
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * delta);
    }

    private Vector3 RotateDirection()
    {
        if(CombatMode)
        {
            Vector3 targetDir = transform.position - combat.target.position;
            targetDir.y = 0.0f;
            targetDir.Normalize();

            if (targetDir == Vector3.zero)
            {
                _ = transform.forward;
            }
        }
        return FollowTarget.forward;
    }

    private void HandleCompanionMovement(float time, float delta)
    {
        Vector3 followPoint = FollowTarget.position - FollowTarget.forward * desiredDistance;
        Vector3 targetPosition = CalculateTargetPosition(followPoint);

        targetPosition = ClampPosition(targetPosition);
        targetPosition += CalculateHoverOffset(time);
        transform.position = Vector3.Lerp(transform.position, targetPosition, delta * movementSpeed);
    }

    private Vector3 ClampPosition(Vector3 targetPosition)
    {
        float minY = FollowTarget.position.y + heightOffset.minValue;
        float maxY = FollowTarget.position.y + heightOffset.maxValue;

        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        return targetPosition;
    }

    private Vector3 CalculateHoverOffset(float time)
    {
        float hoverOffsetY = Mathf.Sin(time * hoverSpeed + hoverTimerOffset) * hoverHeight;

        float oscillationSpeedXZ = hoverSpeed * 0.75f;
        float oscillationAmountXZ = hoverHeight * 0.5f;
        float hoverOffsetX = Mathf.Sin(time * oscillationSpeedXZ + hoverTimerOffset) * oscillationAmountXZ;
        float hoverOffsetZ = Mathf.Cos(time * oscillationSpeedXZ + hoverTimerOffset) * oscillationAmountXZ;

        Vector3 right = FollowTarget.right;
        Vector3 fwd = Vector3.Cross(Vector3.up, right);
        return right * hoverOffsetX + fwd * hoverOffsetZ + Vector3.up * hoverOffsetY;
    }

    private Vector3 CalculateTargetPosition(Vector3 followPoint)
    {
        Vector3 targetPos = followPoint + currentOffset;

        float dis = Vector3.Distance(transform.position, targetPos);
        if(Time.time >= offsetUpdateTime || dis > followOffset)
        {
            currentOffset = GenerateRandomOffset();
            offsetUpdateTime = Time.time + offsetUpdateInterval;
        }

        targetPos = followPoint + currentOffset;
        Vector3 toTarget = targetPos - FollowTarget.position;

        float dist = toTarget.magnitude;
        targetPos = ClampDistance(targetPos, dist, toTarget);
        return targetPos;
    }

    private Vector3 ClampDistance(Vector3 targetPos, float dist, Vector3 toTarget)
    {
        if(dist > distanceOffset.maxValue)
        {
            targetPos = FollowTarget.position + toTarget.normalized * distanceOffset.maxValue;
        }
        else if(dist < distanceOffset.minValue)
        {
            targetPos = FollowTarget.position + toTarget.normalized * distanceOffset.minValue;
        }
        return targetPos;
    }

    private Vector3 GenerateRandomOffset()
    {
        Vector2 randomXZ = Random.insideUnitCircle * offsetXZRadius;
        float random = Random.Range(-offsetYRadius, offsetYRadius);
        return new Vector3(randomXZ.x, random, randomXZ.y);
    }
}
