using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the activation of weapons when the player collects them in the game world.
/// This script should be attached to weapon pickup objects in the scene.
/// </summary>
public class ActivationSword : MonoBehaviour
{
    /// <summary>
    /// Identifier number for the weapon this object represents.
    /// This number should match the weapon index in the CollectionWeapons component.
    /// </summary>
    public int weaponNumber;
    /// <summary>
    /// Reference to the CollectionWeapons component that manages the player's weapon inventory.
    /// </summary>
    private CollectionWeapons collectionWeapons;
   
    /// <summary>
    /// Handles the collision between the player and the weapon pickup object.
    /// When triggered, activates the corresponding weapon in the player's inventory
    /// and destroys the pickup object.
    /// </summary>
    /// <param name="other">The Collider that entered this object's trigger zone</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectionWeapons collectionWeapons = other.GetComponentInChildren<CollectionWeapons>();

            if (collectionWeapons == null)
            {
                Debug.LogError("CollectionWeapons component not found on the Player or its children.");
                return;
            }

            collectionWeapons.ActivationWeapon(weaponNumber);
            Debug.Log($"Weapon {weaponNumber} activated.");

            Destroy(gameObject);
        }
    }
}
