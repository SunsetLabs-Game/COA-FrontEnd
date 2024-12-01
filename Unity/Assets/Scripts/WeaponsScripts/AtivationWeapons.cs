using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationSword : MonoBehaviour
{
    public int weaponNumber; // N�mero de la espada que este objeto representa
    private CollectionWeapons collectionWeapons;
   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Intentar obtener CollectionWeapons en el momento de la colisi�n
            CollectionWeapons collectionWeapons = other.GetComponentInChildren<CollectionWeapons>();

            if (collectionWeapons == null)
            {
                Debug.LogError("CollectionWeapons component not found on the Player or its children.");
                return; // Salir del m�todo si CollectionWeapons es nulo
            }

            // Activar el arma correspondiente
            collectionWeapons.ActivationWeapon(weaponNumber);
            Debug.Log($"Weapon {weaponNumber} activated.");

            // Destruir el objeto que representa la espada en el suelo
            Destroy(gameObject);
        }
    }
}
