using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionWeapons : MonoBehaviour
{
    public GameObject[] weapons; // Array of weapons (swords, etc.)

    // Method to activate a specific sword
    public void ActivationWeapon(int number)
    {
        // Validate that the index is within the weapon range
        if (number < 0 || number >= weapons.Length)
        {
            Debug.LogError($"Invalid weapon number: {number}. Must be between 0 and {weapons.Length - 1}.");
            return;
        }

        // Deactivate all weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        // Activate only the desired sword
        weapons[number].SetActive(true);
        Debug.Log($"Weapon {number} activated.");
    }
}