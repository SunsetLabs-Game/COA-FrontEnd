using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ShowNameOtherScene : MonoBehaviour
{
    public TMP_Text playerNameTetx;

    void Start()
    {
        // Load player name from PlayerPrefs
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        playerNameTetx.text = playerName;

         // Verificar si estamos en la escena de selección de personaje
        if (SceneManager.GetActiveScene().name == "DemoSelectionCharacter")
        {
            // Mostrar "Welcome, [Nombre]" solo en la escena de selección de personaje
            playerNameTetx.text = "Welcome, " + playerName;
        }
        else
        {
            // Mostrar solo el nombre en otras escenas
            playerNameTetx.text = playerName;
        }
    }
}
