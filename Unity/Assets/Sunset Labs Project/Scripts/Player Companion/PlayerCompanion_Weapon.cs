using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class PlayerCompanion_Weapon : MonoBehaviour
{
    //Private Parameters
    private float accumulatedTime;
    private ObjectPool<BulletFX> bulletFXPool;

    private PlayerCompanion manager;
    private CharacterManager characterManager;

    //Private Bullet Parameters
    private readonly List<Bullet> bulletList = new();
    private ObjectPool<TrailRenderer> bulletTrailPool;

    [Header("Gun Status")]
    [SerializeField] private int fireRate = 25;
    [SerializeField] private float bulletDrop = 300f;
    [SerializeField] private float bulletSpeed = 1000f;
    [SerializeField] private float maxBulletTime = 3.0f;

    [Header("FX")]
    [SerializeField] private GameObject decal;
    [SerializeField] private ParticleSystem impact;
    [SerializeField] private float simulationSpeed;
    [SerializeField] private ParticleSystem[] muzzleFlash;
    [SerializeField] private TrailRenderer bulletTrailPrefab;

    [Header("Parameters")]
    [SerializeField] private int damageValue;
    [SerializeField] private Transform muzzlePoint;

    private void Awake()
    {
        manager = GetComponent<PlayerCompanion>();

        bulletTrailPool = ObjectPooler.TrailPool(bulletTrailPrefab);
        bulletFXPool = ObjectPooler.BulletFXPool(decal, impact, this);
    }

    private void Start()
    {
        characterManager = manager.FollowCharacter;
    }

    private void HandleVFX()
    {
        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1);
        }
    }

    public void HandleShooting(Vector3 targetPosition, float delta)
    {
        accumulatedTime += delta;
        float fireInterval = 1.0f / fireRate;

        while (accumulatedTime > 0.0f)
        {
            FireBullet(targetPosition);
            accumulatedTime -= fireInterval;
        }
    }

    public void UpdateBullet(float delta)
    {
        SimulateBullet(delta);
        bulletList.RemoveAll(x => x.time >= maxBulletTime);
    }

    #region Bullet Functions

    private Bullet CreateBullet(Vector3 pos, Vector3 vel)
    {
        Bullet bullet = new(pos, vel);
        return bullet;
    }

    private Vector3 GetBulletPosition(Bullet bullet)
    {
        //Pos = bPos + bVel * bTime + 0.5 * grv * bTime^2
        Vector3 gravity = Vector3.down * bulletDrop;
        return bullet.initialPosition + (bullet.initialVelocity * bullet.time) + (0.5f * bullet.time * bullet.time * gravity);
    }

    private void SimulateBullet(float delta)
    {
        bulletList.ForEach
        (
            bullet =>
            {
                Vector3 p0 = GetBulletPosition(bullet);
                bullet.time += delta;
                Vector3 p1 = GetBulletPosition(bullet);
                HandleRaycastSegment(p0, p1, bullet);
            }
        );
    }

    private void HandleRaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 dir = end - start;
        float distance = dir.magnitude;

        Ray ray = new(start, dir);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, distance))
        {
            Collider collider = raycastHit.collider;
            TrailFX.HandleTrailFX(simulationSpeed, start, raycastHit.point, bulletTrailPool, this);
            CharacterStatistic shotCharacter = collider.GetComponentInParent<CharacterStatistic>();

            BulletFX bulletFX = bulletFXPool.Get();
            InstantiateBulletHoles(bulletFX, raycastHit, shotCharacter);
            if(shotCharacter != null && shotCharacter.Character != characterManager)
            {
                shotCharacter.TakeDamage(damageValue, AttackType.Heavy);
            }
            bullet.time = maxBulletTime;
            return;
        }
        TrailFX.HandleTrailFX(simulationSpeed, start, raycastHit.point, bulletTrailPool, this);
    }

    private void FireBullet(Vector3 targetPosition)
    {
        HandleVFX();
        Vector3 velocity = (targetPosition - muzzlePoint.position).normalized * bulletSpeed;
        Bullet bullet = CreateBullet(muzzlePoint.position, velocity);
        bulletList.Add(bullet);
    }

    private void InstantiateBulletHoles(BulletFX bulletFX, RaycastHit raycastHit, CharacterStatistic shotCharacter)
    {
        if (shotCharacter != null)
            return;

        float decalOffset = 0.05f;
        Quaternion spawnRotation = Quaternion.LookRotation(raycastHit.normal);
        Vector3 spawnPosition = raycastHit.point + raycastHit.normal * decalOffset;

        bulletFX.HandleBulletImpact(spawnPosition, spawnRotation);
    }
    #endregion
}
