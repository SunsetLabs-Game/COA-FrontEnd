using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the display of the player's name across different scenes in the game.
/// This script is responsible for retrieving the stored player name and displaying it
/// with different formats depending on the current scene.
/// </summary>
public class ShowNameOtherScene : MonoBehaviour
{
    /// <summary>
    /// Reference to the TextMeshPro component that will display the player's name.
    /// This must be assigned in the Unity Inspector.
    /// </summary>
    public TMP_Text playerNameTetx;

    /// <summary>
    /// Initializes the player name display when the script starts.
    /// Retrieves the player name from PlayerPrefs and formats it according to the current scene.
    /// </summary>
    void Start()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        playerNameTetx.text = playerName;

        if (SceneManager.GetActiveScene().name == "DemoSelectionCharacter")
        {
            playerNameTetx.text = "Welcome, " + playerName;
        }
        else
        {
            playerNameTetx.text = playerName;
        }
    }
}
