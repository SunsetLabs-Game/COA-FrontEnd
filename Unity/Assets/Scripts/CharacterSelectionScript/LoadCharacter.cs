using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class LoadCharacter : MonoBehaviour
{
    public GameObject[] characterPrefab;
    public Transform spawnPoint;
    public TMP_Text label;

    void Start() // Corrige aquí el error de "Star"
    {
        // Obtén el índice del personaje seleccionado guardado en PlayerPrefs
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", 0); // Usa 0 como valor predeterminado
        if (selectedCharacter < 0 || selectedCharacter >= characterPrefab.Length)
        {
            Debug.LogError("Índice de personaje seleccionado está fuera de rango. Verifica los prefabs asignados en el inspector.");
            return;
        }

        // Instancia el prefab en el punto de spawn
        GameObject prefab = characterPrefab[selectedCharacter];
        GameObject clone = Instantiate(prefab, spawnPoint.position, Quaternion.identity); // Corrige aquí "Quanternion" a "Quaternion"

        // Actualiza el texto del nombre del personaje, si está asignado
        if (label != null)
        {
            label.text = prefab.name;
        }
    }
}
