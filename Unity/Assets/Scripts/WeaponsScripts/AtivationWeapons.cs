using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationSword : MonoBehaviour
{
    public int weaponNumber; // Número de la espada que este objeto representa
    private CollectionWeapons collectionWeapons;

    // Al inicio, buscar el componente CollectionWeapons en el jugador
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            collectionWeapons = player.GetComponentInChildren<CollectionWeapons>();

            if (collectionWeapons == null)
            {
                Debug.LogError("CollectionWeapons component not found on the Player or its children.");
            }
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found.");
        }
    }

    // Método para detectar la colisión y activar el arma
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (collectionWeapons != null)
            {
                collectionWeapons.ActivationWeapon(weaponNumber); // Activar el arma correspondiente
                Destroy(gameObject); // Destruir el objeto que representa la espada en el suelo
            }
            else
            {
                Debug.LogError("CollectionWeapons is null in OnTriggerEnter.");
            }
        }
    }
}
