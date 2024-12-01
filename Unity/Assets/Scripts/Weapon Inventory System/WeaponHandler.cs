using UnityEngine;
using DevionGames.InventorySystem;
using UnityEngine.Animations.Rigging;

public class WeaponHandler : MonoBehaviour
{
    private GameObject weaponDetectorObject;
    private BoxCollider weaponDetectorCollider;

    [HideInInspector] public Item equippedWeaponItem;
    [HideInInspector] public WeaponManager equippedWeapon;

    [Header("Parameters")]
    [SerializeField] private string itemContainername;
    [SerializeField] private GameObject weaponDetectorPrefab;
    [field: SerializeField] public Transform WeaponHolder { get; private set; }

    [Header("Animation Rigging")]
    [SerializeField] private RigBuilder rigBuilder;
    [SerializeField] private Rig HandIKConstraints;
    [SerializeField] private TwoBoneIKConstraint leftHandIKConstraint;
    [SerializeField] private TwoBoneIKConstraint rightHandIKConstraint;

    private void Awake()
    {
        weaponDetectorObject = transform.Find("Weapon Detector Collider").gameObject;

        if(weaponDetectorObject == null)
        {
            weaponDetectorObject = Instantiate(weaponDetectorPrefab);
            weaponDetectorCollider = weaponDetectorObject.AddComponent<BoxCollider>();

            weaponDetectorObject.transform.SetParent(transform);
            weaponDetectorObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        else
        {
            weaponDetectorCollider = weaponDetectorObject.GetComponent<BoxCollider>();
            if(weaponDetectorCollider == null)
            {
                weaponDetectorObject.AddComponent<BoxCollider>();
            }
        }

        weaponDetectorCollider.isTrigger = true;
        weaponDetectorCollider.size = new Vector3(1.0f, 0.35f, 1.0f);
        weaponDetectorCollider.center = new Vector3(0.0f, 0.2f, 0.0f);
        weaponDetectorObject.layer = LayerMask.GetMask("WeaponHandler");

        rigBuilder = GetComponent<RigBuilder>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetKeyDown(KeyCode.F) != true)
        {
            return;
        }

        HandleWeaponPickup(other);
    }

    public void HandleIKPositioning(Transform left, Transform right)
    {
        leftHandIKConstraint.weight = 1.0f;
        rightHandIKConstraint.weight = 1.0f;

        leftHandIKConstraint.data.target = left;
        rightHandIKConstraint.data.target = right;

        rigBuilder.Build();
    }

    private void HandleWeaponPickup(Collider other)
    {
        WeaponManager weaponManager = other.GetComponentInParent<WeaponManager>();

        if(weaponManager != null)
        {
            weaponManager.AddToInventoryManager();
        }
    }
}
