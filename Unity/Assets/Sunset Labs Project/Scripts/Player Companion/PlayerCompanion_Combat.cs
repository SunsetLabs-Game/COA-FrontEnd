using UnityEngine;

public class PlayerCompanion_Combat : MonoBehaviour
{
    private PlayerCompanion manager;
    private Collider[] enemyColliders;

    private bool canShoot;
    public Transform target;
    private float shootingTimer = 0.0f;
    public CharacterManager EnemyTarget { get; private set; }

    [Header("Weapon Parameters")]
    [SerializeField] private float inaccuracy;
    [SerializeField] private Vector3 targetOffset;

    [Header("Combat Properties")]
    [SerializeField] private float shieldHealth;
    [SerializeField] private float shieldActiveTimer;
    [SerializeField] private float shootCoolDownTime = 5.0f;
    [SerializeField] private PlayerCompanion_Weapon weaponManager;
    
    [Header("Detection Parameters")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask obstacleMask;
    [Range(-180, 180)][SerializeField] private float detectAngle;
    [Range(1.0f, 10.0f)][SerializeField] private float detectRadius;

    private void Awake()
    {
        manager = GetComponent<PlayerCompanion>();
        weaponManager = GetComponent<PlayerCompanion_Weapon>();
    }

    private void Start()
    {
        canShoot = true;
        enemyColliders = new Collider[5];
    }

    public void Combat_Update(float delta)
    {
        if(manager.mentalState == CombatMentalState.Friendly)
        {
            return;
        }

        bool hasReachedSearchPeriod = (Time.frameCount % 20 == 0);
        bool noEnemy = (EnemyTarget == null || EnemyTarget.isDead);

        if (hasReachedSearchPeriod && noEnemy)
        {
            EnemyTarget = GetTarget();
        }
        weaponManager.UpdateBullet(delta);

        if(EnemyTarget == null)
        {
            return;
        }

        HandleShooting(delta);
        if (EnemyTarget.isDead)
        {
            manager.hasViolentTarget = false;
        }
    }

    private CharacterManager GetTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, detectRadius, enemyColliders, enemyMask);
        for(int i = 0; i < count; i++)
        {
            Collider col = enemyColliders[i];
            if(col == null)
            {
                continue;
            }

            if(GameObjectTool.TryGetComponentInParent(col.transform, out CharacterManager potentialTarget))
            {
                if(potentialTarget == manager.FollowCharacter || potentialTarget.mentalState == CombatMentalState.Friendly)
                {
                    continue;
                }

                if(InLineOfSight(potentialTarget.transform) != true)
                {
                    continue;
                }
                manager.hasViolentTarget = true;
                target = potentialTarget.transform;
                return potentialTarget;
            }
        }
        return null;
    }

    private void HandleShooting(float delta)
    {
        if (canShoot != true)
        {
            shootingTimer -= delta;
            if (shootingTimer <= 0)
            {
                shootingTimer = 0;
                canShoot = true;
            }
            return;
        }
        Vector3 targetPos = target.position + targetOffset;
        targetPos += Random.insideUnitSphere * inaccuracy;
        weaponManager.HandleShooting(targetPos, delta);
        Invoke(nameof(DisableShooting), shootCoolDownTime);
    }

    private void DisableShooting()
    {
        canShoot = false;
        shootingTimer = shootCoolDownTime + 3.5f;
    }

    private bool InLineOfSight(Transform potentialTarget)
    {
        Vector3 direction = (potentialTarget.position - transform.position).normalized;

        float viewAngle = Vector3.Angle(direction, transform.forward);
        if (viewAngle < detectAngle / 2)
        {
            return (Physics.Linecast(transform.position, potentialTarget.position, obstacleMask) != true);
        }
        return false;
    }
}
