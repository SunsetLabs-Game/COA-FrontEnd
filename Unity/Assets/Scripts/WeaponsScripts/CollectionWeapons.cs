using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a collection of weapon GameObjects in the game.
/// This script handles the activation and deactivation of different weapons
/// typically attached to a player or weapon manager object.
/// </summary>
public class CollectionWeapons : MonoBehaviour
{
    /// <summary>
    /// Array containing all available weapon GameObjects.
    /// Each element represents a different weapon (sword, axe, etc.) that can be activated.
    /// </summary>
    public GameObject[] weapons;

    /// <summary>
    /// Activates a specific weapon and deactivates all others.
    /// Used when switching between different weapons in the game.
    /// </summary>
    /// <param name="number">The index of the weapon to activate (0-based index)</param>
    public void ActivationWeapon(int number)
    {
        if (number < 0 || number >= weapons.Length)
        {
            Debug.LogError($"Invalid weapon number: {number}. Must be between 0 and {weapons.Length - 1}.");
            return;
        }

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        weapons[number].SetActive(true);
        Debug.Log($"Weapon {number} activated.");
    }
}