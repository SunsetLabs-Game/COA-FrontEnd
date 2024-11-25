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
    public Transform player;

    /// <summary>
    /// Updates the minimap camera position to follow the player.
    /// Called after all Update functions have been called.
    /// </summary>
    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;

        transform.position = newPosition;
    }
}
