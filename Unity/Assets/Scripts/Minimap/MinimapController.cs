using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public Transform player; // El jugador al que seguir� el minimapa

    private void LateUpdate()
    {
        if (player == null) return; // Aseg�rate de que el jugador est� asignado

        // Actualizar la posici�n de la c�mara para que siga al jugador
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y; // Mantener la altura configurada en el Inspector

        transform.position = newPosition;
    }
}
