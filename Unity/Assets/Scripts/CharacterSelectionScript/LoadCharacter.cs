using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class LoadCharacter : MonoBehaviour
{
    public GameObject[] characterPrefab;
    public Transform spawnPoint;
    public TMP_Text label;

    void Start() // Corrige aqu� el error de "Star"
    {
        // Obt�n el �ndice del personaje seleccionado guardado en PlayerPrefs
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", 0); // Usa 0 como valor predeterminado
        if (selectedCharacter < 0 || selectedCharacter >= characterPrefab.Length)
        {
            Debug.LogError("�ndice de personaje seleccionado est� fuera de rango. Verifica los prefabs asignados en el inspector.");
            return;
        }

        // Instancia el prefab en el punto de spawn
        GameObject prefab = characterPrefab[selectedCharacter];
        GameObject clone = Instantiate(prefab, spawnPoint.position, Quaternion.identity); // Corrige aqu� "Quanternion" a "Quaternion"

        // Actualiza el texto del nombre del personaje, si est� asignado
        if (label != null)
        {
            label.text = prefab.name;
        }
    }
}
