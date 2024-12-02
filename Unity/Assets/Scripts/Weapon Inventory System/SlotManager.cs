using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DevionGames.InventorySystem;

public class SlotClass
{
    public Slot itemSlot;
    public Button itemButton;

    public SlotClass(Slot slot, Button button)
    {
        itemSlot = slot;
        itemButton = button;
    }
}
public class SlotManager : MonoBehaviour
{
    private bool hasInitialized;
    private PlayerWeaponHandler weaponHandler;

    private Button[] buttons;
    private List<SlotClass> slotClasses = new List<SlotClass>();

    private void Update()
    {
        if(hasInitialized == true)
        {
            return;
        }

        weaponHandler = FindFirstObjectByType<PlayerWeaponHandler>();
        if(weaponHandler != null )
        {
            InitializeButton();
            hasInitialized = true;
        }
    }

    private void InitializeButton()
    {
        buttons = GetComponentsInChildren<Button>();

        for(int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            Slot slot = button.GetComponent<Slot>();

            SlotClass slotClass = new SlotClass(slot, button);

            slotClasses.Add(slotClass);
            slotClass.itemButton.onClick.AddListener(() => EquipWeapon(slotClass));
        }
    }

    private void EquipWeapon(SlotClass slotClass)
    {
        Item equippedItem = slotClass.itemSlot.ObservedItem;
        if(weaponHandler.equippedWeaponItem == equippedItem)
        {
            return;
        }

        weaponHandler.equippedWeaponItem = equippedItem;
        if (weaponHandler.equippedWeapon != null)
        {
            weaponHandler.HandleIKPositioning(null, null);
            Destroy(weaponHandler.equippedWeapon);
            weaponHandler.equippedWeapon = null;
        }

        WeaponManager equippedWeapon = Instantiate(equippedItem.Prefab).GetComponent<WeaponManager>();
        if(equippedWeapon == null)
        {
            return;
        }
        equippedWeapon.EquipWeapon(weaponHandler);
    }
}
