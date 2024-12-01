using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles the loading and instantiation of character prefabs based on player selection.
/// This script is used in game scenes where the selected character needs to be spawned.
/// It reads the player's character selection from PlayerPrefs and instantiates the corresponding prefab.
/// </summary>
public class LoadCharacter : MonoBehaviour
{
    /// <summary>
    /// Array of character prefabs that can be instantiated.
    /// These should be assigned in the Unity Inspector.
    /// </summary>
    public GameObject[] characterPrefab;
    /// <summary>
    /// Array of drone prefabs that can be instantiated.
    /// These should be assigned in the Unity Inspector.
    /// </summary>
    public GameObject[] dronePrefab;
    /// <summary>
    /// Transform reference for the position where the character will be spawned.
    /// Should be set in the Unity Inspector.
    /// </summary>
    public Transform spawnPoint;
    /// <summary>
    /// TextMeshPro UI component to display the selected character's name.
    /// Optional - can be left unassigned if character name display is not needed.
    /// </summary>
    public TMP_Text label;

    /// <summary>
    /// Initializes the character and its drone loading process when the script starts.
    /// Retrieves the selected character index from PlayerPrefs and instantiates the corresponding prefab.
    /// Also updates the character name label if one is assigned.
    /// </summary>
    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", 0); 
        if (selectedCharacter < 0 || selectedCharacter >= characterPrefab.Length)
        {
            Debug.LogError("�ndice de personaje seleccionado est� fuera de rango. Verifica los prefabs asignados en el inspector.");
            return;
        }

        GameObject prefab = characterPrefab[selectedCharacter];
        GameObject clone = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        MinimapController minimapController = FindObjectOfType<MinimapController>();
        if (minimapController != null)
        {
            minimapController.player = clone.transform;
        }

        if (label != null)
        {
            label.text = prefab.name;
        }

        // Instantiate the corresponding drone if available
        if (selectedCharacter < dronePrefab.Length)
        {
            GameObject drone = Instantiate(dronePrefab[selectedCharacter], spawnPoint.position + new Vector3(0, 2, -1), Quaternion.identity);

            // Set up the drone to follow the character
            DroneFollower droneFollower = drone.AddComponent<DroneFollower>();
            droneFollower.target = clone.transform;

            // Assign the main camera to the drone 
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                droneFollower.cameraTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogWarning("No se encontró la cámara principal en la escena.");
            }

        }
        else
        {
            Debug.LogWarning("No se encontró un drone asignado para este personaje.");
        }
    }
}
