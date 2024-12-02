using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadCharacter : MonoBehaviour
{
    public GameObject[] characterPrefab;
<<<<<<< HEAD
    /// <summary>
    /// Array of drone prefabs that can be instantiated.
    /// These should be assigned in the Unity Inspector.
    /// </summary>
    public GameObject[] dronePrefab;
    /// <summary>
    /// Transform reference for the position where the character will be spawned.
    /// Should be set in the Unity Inspector.
    /// </summary>
=======
>>>>>>> 919593f928df30540e00d5255638fe2318c32e57
    public Transform spawnPoint;
    public TMP_Text label;

<<<<<<< HEAD
    /// <summary>
    /// Initializes the character and its drone loading process when the script starts.
    /// Retrieves the selected character index from PlayerPrefs and instantiates the corresponding prefab.
    /// Also updates the character name label if one is assigned.
    /// </summary>
    void Start()
=======
    void Start() // Corrige aquÌ el error de "Star"
>>>>>>> 919593f928df30540e00d5255638fe2318c32e57
    {
        // ObtÈn el Ìndice del personaje seleccionado guardado en PlayerPrefs
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", 0); // Usa 0 como valor predeterminado
        if (selectedCharacter < 0 || selectedCharacter >= characterPrefab.Length)
        {
            Debug.LogError("Õndice de personaje seleccionado est· fuera de rango. Verifica los prefabs asignados en el inspector.");
            return;
        }

        // Instancia el prefab en el punto de spawn
        GameObject prefab = characterPrefab[selectedCharacter];
        GameObject clone = Instantiate(prefab, spawnPoint.position, Quaternion.identity); // Corrige aquÌ "Quanternion" a "Quaternion"

        // Actualiza el texto del nombre del personaje, si est· asignado
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
                Debug.LogWarning("No se encontr√≥ la c√°mara principal en la escena.");
            }

        }
        else
        {
            Debug.LogWarning("No se encontr√≥ un drone asignado para este personaje.");
        }
    }
}
