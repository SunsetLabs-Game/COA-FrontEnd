using UnityEngine;
using System.Collections.Generic;
using DevionGames.InventorySystem;
using DevionGames.UIWidgets;

public class WeaponManager : MonoBehaviour
{
    private bool hasBeenEquiped;

    [Header("Parameters")]
    private Transform parent;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Transform weaponGrip;
    [SerializeField] private Transform weaponRest;
    [SerializeField] private string inventoryWindowName = "Inventory";

    [Header("Devion Parameters")]
    public bool saveable = true;
    [SerializeField] private List<int> amount = new();
    [SerializeField] private List<ItemModifierList> weaponModifiers = new();
    [ItemPicker(true)][SerializeField] private List<Item> weaponItems = new();

    [Header("Weapon Colliders")]
    [SerializeField] private MeshCollider weaponMeshCollider;
    [SerializeField] private BoxCollider weaponTriggerCollider;

    private void Awake()
    {
        parent = transform.Find("Transforms");
        weaponGrip = parent.Find("Grip");
        weaponRest = parent.Find("Rest");
        rigidBody = GetComponent<Rigidbody>();
        
        weaponMeshCollider = GetComponent<MeshCollider>();
        weaponTriggerCollider = GetComponentInChildren<BoxCollider>();
    }

    public void HandleRigidBodySettings(bool hasPicked)
    {
        weaponMeshCollider.enabled = !hasPicked;
        weaponTriggerCollider.enabled = !hasPicked;
        hasBeenEquiped = rigidBody.isKinematic = hasPicked;

        if(hasPicked)
        {
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
            return;
        }
        rigidBody.constraints = RigidbodyConstraints.None;
    }

    public void EquipWeapon(WeaponHandler weaponHandler)
    {
        weaponHandler.equippedWeapon = this;
        weaponHandler.HandleIKPositioning(weaponRest, weaponGrip);

        HandleRigidBodySettings(true);
        transform.SetParent(weaponHandler.WeaponHolder);
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void AddToInventoryManager()
    {
        Item existingItem = ItemContainer.GetItem(inventoryWindowName, weaponItems[0].Id);

        if(existingItem != null)
        {
            existingItem.Stack += amount[0];
            Destroy(gameObject);
        }
        else
        {
            bool success = ItemContainer.AddItem(inventoryWindowName, weaponItems[0]);
            if (success)
            {
                Destroy(gameObject);
            }
        }

        ItemContainer itemContainer = WidgetUtility.Find<ItemContainer>(inventoryWindowName);
        if(itemContainer != null)
        {
            itemContainer.RefreshSlots();
        }
    }

    public void DropFromInventoryManager()
    {

    }
}