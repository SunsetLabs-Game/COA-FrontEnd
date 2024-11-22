using UnityEngine;

/// <summary>
/// Controls the minimap camera behavior by following the player's position.
/// This script should be attached to the minimap camera object in the scene.
/// </summary>
public class MinimapController : MonoBehaviour
{
    /// <summary>
    /// Reference to the player's transform that the minimap camera will follow.
    /// Must be assigned through the Unity Inspector.
    /// </summary>
    public Transform player; // El jugador al que seguir� el minimapa

    /// <summary>
    /// Updates the minimap camera position to follow the player.
    /// Called after all Update functions have been called.
    /// </summary>
    private void LateUpdate()
    {
        if (player == null) return; // Ensure player reference is assigned

        // Update camera position to follow player while maintaining its height
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y; // Keep height set in Inspector

        transform.position = newPosition;
    }
}
