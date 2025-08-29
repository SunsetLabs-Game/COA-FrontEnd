using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;

[RequireComponent(typeof(PickableObject))]
public class WeaponManager : MonoBehaviour
{
    //Private Variables
    private Ray ray;
    private int bulletLeft;
    private float accumulatedTime;

    private CinemachineCamera shooterCam;
    private CinemachineImpulseSource impulseSource;

    [Header("Gun Status")]
    [SerializeField] private int fireRate = 25;
    [SerializeField] private float gunRange = 15.0f;
    [SerializeField] private float simulationSpeed = 3.0f;

    [field: Header("Parameters")]
    [field: SerializeField] public WeaponType type { get; private set; }
    [field: SerializeField] public PickableObject pickableObject { get; private set; }

    [Header("Gun Parameters")]
    [SerializeField] private int maxBullets;
    [SerializeField] private bool isShooting;
    [SerializeField] private Transform grip, rest;

    [field: Header("Layer Masks")]
    [field: SerializeField] public LayerMask EnemyMask { get; private set; }
    [field: SerializeField] public LayerMask DamageableMask { get; private set; }

    [Header("Gun Parameters (FX)")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private ParticleSystem[] muzzleFlash;

    [field: Header("Melee Parameters")]
    [field: SerializeField] public CharacterDamageCollider DamageCollider { get; private set; }

    [Header("Weapon Recoil")]
    [SerializeField] private WeaponRecoil weaponRecoil = new();

    private void Awake()
    {
        pickableObject = GetComponent<PickableObject>();

        impulseSource = GetComponent<CinemachineImpulseSource>();
        DamageCollider = GetComponentInChildren<CharacterDamageCollider>(); 
    }

    public void Initialize(Transform cameraObject, CharacterManager character)
    {
        bulletLeft = maxBullets;
        character.CombatManager.AssignWeapon(this);

        if (DamageCollider != null)
        {
            DamageCollider.SetCharacter(character);
        }

        if(type == WeaponType.Gun)
        {
            if (character.characterType == CharacterType.Player)
            {
                shooterCam = character.CameraController.ShooterVirtualCamera;
            }
            character.RigController.SetRigs(true);
            weaponRecoil.Initialize(cameraObject, impulseSource);
        }
        if (character != null) character.RigController.SetTwoBoneIKConstraint(grip, rest);
    }

    private void HandleReload()
    {
        if (bulletLeft >= maxBullets)
        {
            return;
        }
        int difference = maxBullets - bulletLeft;
        bulletLeft += difference;
    }

    public void WeaponManager_Update(Vector3 targetPosition, CharacterManager character, float delta)
    {
        weaponRecoil.HandleRecoil(delta);
        Vector3 direction = targetPosition - muzzlePoint.position;

        ray = new(muzzlePoint.position, direction);

        if(character.characterType == CharacterType.Player)
        {
            Image crossHairImage = character.CameraController.CrossHairImg;
            if (Physics.Raycast(ray, gunRange, EnemyMask))
            {
                crossHairImage.color = Color.green;
                return;
            }
            crossHairImage.color = Color.white;
        }
    }

    public void HandleAction(Vector3 targetPosition, CharacterManager characterManager)
    {
        if(type == WeaponType.Gun)
        {
            HandleAction_Gun(targetPosition, characterManager);
        }
    }

    private void HandleAction_Gun(Vector3 targetPosition, CharacterManager characterManager)
    {
        if(characterManager != null)
        characterManager.CombatManager.currentAction = null;

        HandleVFX();
        HandleShooting(targetPosition, characterManager);
    }

    #region Gun Parameters
    private void HandleVFX()
    {
        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1);
        }
    }

    private void HandleShooting(Vector3 targetPosition, CharacterManager characterManager)
    {
        accumulatedTime += Time.deltaTime;
        float fireInterval = 1.0f / fireRate;

        while (accumulatedTime > 0.0f)
        {
            FireGun(targetPosition, characterManager);
            accumulatedTime -= fireInterval;
        }
    }

    private void FireGun(Vector3 targetPosition, CharacterManager character)
    {
        weaponRecoil.GenerateRecoilPattern();
        if(Physics.Raycast(ray, out RaycastHit hitInfo, gunRange, DamageableMask))
        {
            CharacterManager shotCharacter = hitInfo.collider.GetComponentInParent<CharacterManager>();
            StartCoroutine(HandleTrailFX(muzzlePoint.position, hitInfo.point));
            
            if (shotCharacter == null)
            {
                InstantiateBulletHoles(hitInfo);
                return;
            }

            bool sameTeam = (shotCharacter.currentTeam != character.currentTeam);
            if(sameTeam)
            {
                shotCharacter.StatsManager.TakeDamage(2, AttackType.Heavy);
            }
            return;
        }
        StartCoroutine(HandleTrailFX(muzzlePoint.position, targetPosition));
    }

    private void InstantiateBulletHoles(RaycastHit raycastHit)
    {
        float positionMultiplier = 0.5f;
        float spawnX = raycastHit.point.x - ray.direction.x * positionMultiplier;
        float spawnY = raycastHit.point.y - ray.direction.y * positionMultiplier;
        float spawnZ = raycastHit.point.z - ray.direction.z * positionMultiplier;
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

        GameObject bulletHoles = ImpactHole(raycastHit.collider);
        Quaternion targetRotation = Quaternion.LookRotation(ray.direction);
        bulletHoles.transform.SetPositionAndRotation(spawnPosition, targetRotation);
    }

    private GameObject ImpactHole(Collider damagedCollider)
    {
        if (damagedCollider.CompareTag("Wood"))
        {
            return GameObjectManager.woodBulletHolesPool.Get();
        }
        else if (damagedCollider.CompareTag("Metal"))
        {
            return GameObjectManager.metalBulletHolesPool.Get();
        }
        return GameObjectManager.cementBulletHolesPool.Get();
    }

    private IEnumerator HandleTrailFX(Vector3 start, Vector3 end)
    {
        TrailRenderer trail = GameObjectManager.bulletTrailPool.Get();
        trail.transform.position = start;
        yield return null;

        trail.emitting = true;
        float distance = Vector3.Distance(start, end);
        float remainingDistance = distance;
        while (remainingDistance > 0f)
        {
            trail.transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(1 - (remainingDistance / distance)));

            remainingDistance -= simulationSpeed * Time.deltaTime;
            yield return null;
        }

        trail.transform.position = end;
        yield return new WaitForSeconds(trail.time);
        yield return null;

        trail.emitting = false;
        trail.transform.position = end;

        GameObjectManager.bulletTrailPool.Release(trail);
    }

    #endregion
}
