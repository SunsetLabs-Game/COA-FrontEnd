using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public Transform player; // El jugador al que seguirá el minimapa

    private void LateUpdate()
    {
        if (player == null) return; // Asegúrate de que el jugador está asignado

        // Actualizar la posición de la cámara para que siga al jugador
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y; // Mantener la altura configurada en el Inspector

        transform.position = newPosition;
    }
}
