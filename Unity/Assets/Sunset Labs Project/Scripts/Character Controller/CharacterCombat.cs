using UnityEngine;
using System.Collections.Generic;

public enum AttackType
{
    Light,
    Heavy
}

public enum ComboStatus
{
    Can,
    Cannot
}

public class CharacterCombat : MonoBehaviour
{
    CharacterManager characterManager;

    private bool hasHashed;
    private int shouldMirrorHash;
    private Vector3 targetPosition;
    public WeaponManager CurrentWeapon { get; private set; }

    [Header("Combat Status")]
    public bool canCombo;
    public AttackType attackType;
    [SerializeField] private bool isSciFi;
    public List<WeaponManager> enemyPossibleWeapons = new();

    [Header("Parameters")]
    public int damageModifier;
    public float currentRecovery;
    [SerializeField] private bool mirrorAttack;
    [SerializeField] private Transform crossHairTransform;

    [Header("Gun Parameters")]
    [SerializeField] private float inaccuracy;
    [SerializeField] private Vector3 targetOffset;

    [Header("Melee Parameters")]
    [SerializeField] private AttackActions[] lightActions;
    [SerializeField] private AttackActions[] heavyActions;
    [SerializeField] private CharacterDamageCollider[] damageColliders;

    [field: Header("Combat Character")]
    public AttackActions currentAction;
    [SerializeField] private Transform GunWeaponHolder;
    [SerializeField] private Transform MeleeWeaponHolder;
    [field: SerializeField] public CharacterCombatData CombatCharacter { get; private set; }

    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
    }

    private void OnEnable()
    {
        if (hasHashed == true)
        {
            return;
        }

        InitializeAttackActions();
        foreach (var c in damageColliders)
        {
            c.SetCharacter(characterManager);
        }
        shouldMirrorHash = Animator.StringToHash("shouldMirror");
    }

    private void OnDisable()
    {
        if (hasHashed == true)
        {
            hasHashed = false;
        }
    }

    public void AssignWeapon(WeaponManager weapon)
    {
        CurrentWeapon = weapon;
    }

    public Transform WeaponHolder(WeaponManager weapon)
    {
        if(weapon == null || weapon.type == WeaponType.Melee)
        {
            return MeleeWeaponHolder;
        }
        return GunWeaponHolder;
    }

    public Vector3 GetTargetPosition()
    {
        if(characterManager.characterType == CharacterType.AI)
        {
            Vector3 target = characterManager.PositionOfTarget + targetOffset;
            target += Random.insideUnitSphere * inaccuracy;
            return target;
        }
        return crossHairTransform.position;
    }

    public void SetCrossHair(Transform crossHair)
    {
        crossHairTransform = crossHair;
    }

    public void Combat_Update(float delta)
    {
        bool hasGun = HasGun();
        if (crossHairTransform != null)
        {
            targetPosition = GetTargetPosition();
        }
        CharacterType type = characterManager.characterType;

        if (hasGun)
        {
            CurrentWeapon.WeaponManager_Update(targetPosition, characterManager, delta);
        }
        if (type == CharacterType.AI)
        {
            HandleRecoveryTimer(delta);
        }
        else if (type == CharacterType.Player)
        {
            Attack(characterManager.PlayerInput);
            characterManager.CameraController.EnableShooterGraphics(hasGun, characterManager.isLockedIn, delta);
        }    
    }

    private void HandleRecoveryTimer(float delta)
    {
        if (currentRecovery <= 0.0f)
        {
            currentRecovery = 0.0f;
            return;
        }

        if (characterManager.performingAction)
        {
            return;
        }
        currentRecovery -= delta;
    }

    public void SetComboStatus(ComboStatus status)
    {
        canCombo = (status == ComboStatus.Can);
    }

    private bool DoNotAttack()
    {
        if (DialogueManager.Instance != null)
        {
            return DialogueManager.Instance.dialogueIsPlaying;
        }
        if (CharacterInventoryManager.Instance != null)
        {
            return CharacterInventoryManager.Instance.Panel.IsMouseOverPanel;
        }
        return false;
    }

    private void Attack(InputManager input)
    {
        characterManager.isAttacking = (input.lightAttackInput == true || input.heavyAttackInput == true);

        bool cantAttack = DoNotAttack();
        if(characterManager.isAttacking != true || cantAttack)
        {
            return;
        }

        bool noWeapon = (CurrentWeapon == null);
        if (noWeapon || CurrentWeapon.type == WeaponType.Melee)
        {
            if (input.lightAttackInput)
            {
                int random = Random.Range(0, lightActions.Length);
                currentAction = lightActions[random];
            }
            else
            {
                int random = Random.Range(0, heavyActions.Length);
                currentAction = heavyActions[random];
            }
            currentAction.PerformAction(noWeapon, characterManager);
            return;
        }
        HandleWeaponAction();
    }

    public void HandleWeaponAction()
    {
        CurrentWeapon.HandleAction(targetPosition, characterManager);
    }    

    public bool HasGun()
    {
        return CurrentWeapon != null && CurrentWeapon.type == WeaponType.Gun;
    }

    public void SetMirrorStatus(bool status, Animator animator)
    {
        mirrorAttack = status;
        animator.SetBool(shouldMirrorHash, mirrorAttack);
    }

    public void EnableCollider(int colliderIndex)
    {
        if (HasGun())
        {
            return;
        }
        int index = (mirrorAttack) ? colliderIndex + 1 : colliderIndex;
        var damage = (CurrentWeapon == null) ? damageColliders[index] : CurrentWeapon.DamageCollider;
        damage.SetColliderStatus(true);
    }

    public void DisableCollider(int colliderIndex)
    {
        if (HasGun())
        {
            return;
        }

        CharacterDamageCollider damageCollider;
        if(CurrentWeapon == null)
        {
            int index = (mirrorAttack) ? colliderIndex + 1 : colliderIndex;
            damageCollider = damageColliders[index];
        }
        else
        {
            damageCollider = CurrentWeapon.DamageCollider;
        }
        damageCollider.SetColliderStatus(false);
    }

    public void ResetPerformAttack()
    {
        characterManager.Attack.ResetPerformAttack();
    }

    public void SetDuellingCharacter()
    {
        if(CombatManager.Instance == null)
        {
            return;
        }
        CombatManager.Instance.AssignPlayer(CombatCharacter.characterManager);
    }

    public void SetDamageColliders()
    {
        GameObject newObject = new();

        CharacterDamageCollider leftLeg = GetDamageCollider(GameObjectName(true, "L"), newObject);
        CharacterDamageCollider rightLeg = GetDamageCollider(GameObjectName(true, "R"), newObject);
        CharacterDamageCollider leftHand = GetDamageCollider(GameObjectName(false, "L"), newObject);
        CharacterDamageCollider rightHand = GetDamageCollider(GameObjectName(false, "R"), newObject);

        CreateHurtBox(newObject, 11);
        damageColliders = new[]{ leftHand, rightHand, leftLeg, rightLeg };

        DestroyImmediate(newObject);
    }

    private string GameObjectName(bool isLeg, string suffix)
    {
        string objectName;
        if (isLeg)
        {
            objectName = (isSciFi) ? "Ball_" : "Foot.";
        }
        else
        {
            objectName = (isSciFi) ? "Hand_" : "Hand.";
        }
        return objectName + suffix;
    }

    private CharacterDamageCollider GetDamageCollider(string name, GameObject go)
    {
        LayerMask layer = 11;
        string colliderName = name + " Damage Collider";
        Transform parent = GameObjectTool.FindChildRecursively(transform, name);

        if(GameObjectTool.TryFindChildRecursively(parent, colliderName, out Transform t))
        {
            DestroyImmediate(t.gameObject);
        }
        GameObject gameObject = Instantiate(go, parent);

        gameObject.layer = layer;
        gameObject.name = colliderName;
        gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        CharacterDamageCollider damageCollider = gameObject.AddComponent<CharacterDamageCollider>();

        damageCollider.SetParameters(0.175f, LayerMask.GetMask("Damage Layer"));
        return damageCollider;
    }

    private void CreateHurtBox(GameObject go, LayerMask layer)
    {
        string objectName = "Body Damage Collider";
        if (GameObjectTool.TryFindChildRecursively(transform, objectName, out Transform t))
        {
            DestroyImmediate(t.gameObject);
        }
        GameObject body = Instantiate(go, transform);

        body.name = objectName;
        CapsuleCollider capsule = body.AddComponent<CapsuleCollider>();
        capsule.height = 1.50f;
        capsule.radius = 0.425f;
        capsule.center = new Vector3(0, 0.75f, 0);

        objectName = "Head Damage Collider";
        Transform parent = GameObjectTool.FindChildRecursively(transform, "Head");
        if (GameObjectTool.TryFindChildRecursively(transform, objectName, out t))
        {
            DestroyImmediate(t.gameObject);
        }
        GameObject head = Instantiate(go, parent);

        head.name = objectName;
        SphereCollider sphere = head.AddComponent<SphereCollider>();
        sphere.radius = 0.03f;
        sphere.center = new Vector3(0, 0.005f, 0.003f);

        head.layer = body.layer = layer;
        body.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        head.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if(TryGetComponent(out CharacterController controller))
        {
            controller.radius = 0.3f;
            controller.height = 1.6f;
            controller.center = new Vector3(0,0.925f,0);
        }
    }

    private void InitializeAttackActions()
    {
        for (int i = 0; i < lightActions.Length; i++)
        {
            lightActions[i] = Instantiate(lightActions[i]);
            lightActions[i].Initialize();
        }

        for (int i = 0; i < heavyActions.Length; i++)
        {
            heavyActions[i] = Instantiate(heavyActions[i]);
            heavyActions[i].Initialize();
        }
    }

    internal void CreateEnemyWeapons(WeaponManager exclude, WeaponManager[] potentialWeapons)
    {
        while (enemyPossibleWeapons.Count < 3)
        {
            exclude = GameObjectTool.GetRandomExcluding(exclude, potentialWeapons);
            if (exclude == null)
            {
                Debug.LogError("No weapon found");
                break;
            }
            Transform holder = WeaponHolder(exclude);
            WeaponManager spawnedItem = Instantiate(exclude, holder);

            spawnedItem.gameObject.SetActive(false);
            spawnedItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            spawnedItem.pickableObject.SetPhysicsSystem(false);
            if (enemyPossibleWeapons.Contains(spawnedItem) != true)
            {
                enemyPossibleWeapons.Add(spawnedItem);
            }
        }
    }
}
