using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAnimatorRigController : MonoBehaviour
{
    private Rig[] rigLayers;
    protected CharacterManager characterManager;

    [Header("Stats")]
    [SerializeField] protected float aimDuration;

    [field: Header("Rigs")]
    public RigBuilder rigBuilder;
    [field: SerializeField] public Rig HandIKConstraints { get; protected set; }
    [field: SerializeField] public Rig BodyAimConstraints { get; protected set; }
    [field: SerializeField] public Rig WeaponAimConstraint { get; protected set; }

    [field: Header("Hand_IK Parameters")]
    [field: SerializeField] public TwoBoneIKConstraint LeftHandIKConstraint { get; protected set; }
    [field: SerializeField] public TwoBoneIKConstraint RightHandIKConstraint { get; protected set; }

    [field: Header("Aiming Constraints")]
    [field: SerializeField] public MultiAimConstraint[] MultiAimConstraintArray { get; protected set; }

    protected virtual void Awake()
    {
        rigLayers = GetComponentsInChildren<Rig>();
        rigBuilder = GetComponentInParent<RigBuilder>();
        characterManager = GetComponentInParent<CharacterManager>();
    }

    public virtual void CharacterAnimationRig_Updater(float delta)
    {
        Lock_In(delta);
    }

    public void SetTwoBoneIKConstraint(Transform weaponGrip, Transform weaponRest)
    {
        LeftHandIKConstraint.data.target = weaponRest;
        RightHandIKConstraint.data.target = weaponGrip;

        LeftHandIKConstraint.weight = (weaponRest == null) ? 0.0f : 0.55f;
        RightHandIKConstraint.weight = (weaponGrip == null) ? 0.0f : 1.0f;
        rigBuilder.Build();
    }

    public void SetAimTarget(Transform aimedTarget)
    {
        foreach (MultiAimConstraint multiAimConstraint in MultiAimConstraintArray)
        {
            var sources = multiAimConstraint.data.sourceObjects;
            WeightedTransform weightedTransform = new(aimedTarget, 1f);

            sources.Clear();
            sources.Add(weightedTransform);
            multiAimConstraint.data.sourceObjects = sources;
        }
        rigBuilder.Build();
    }

    public void SetRigs(bool status)
    {
        int weight = (status)? 1 : 0;
        foreach(var rig in rigLayers)
        {
            rig.weight = weight;
        }
        rigBuilder.Build();
    }

    private void Lock_In(float delta)
    {
        float moveDuration = delta / aimDuration;
        WeaponManager currentWeapon = characterManager.CombatManager.CurrentWeapon;

        if (currentWeapon == null || currentWeapon.type == WeaponType.Melee)
        {
            return;
        }

        if (characterManager.isLockedIn)
        {
            WeaponAimConstraint.weight += moveDuration;
            LeftHandIKConstraint.weight += moveDuration;
            return;
        }

        //Instantly Set Aim if isAttacking and not locked in
        if (characterManager.isAttacking)
        {
            WeaponAimConstraint.weight = 1.0f;
            LeftHandIKConstraint.weight = 1.0f;
            return;
        }
        WeaponAimConstraint.weight -= moveDuration;
        LeftHandIKConstraint.weight -= Mathf.Max(moveDuration, 0.55f);
    }
}
