using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the character selection screen functionality, allowing players to browse and select different characters
/// before starting the game.
/// </summary>
public class CharacterSelection : MonoBehaviour
{
    /// <summary>
    /// Array containing references to all available character GameObjects that can be selected.
    /// Only one character should be active at a time.
    /// </summary>
    public GameObject[] characters;
    /// <summary>
    /// Index of the currently selected character in the characters array.
    /// </summary>
    public int selectedCharacter = 0;

    /// <summary>
    /// Switches to the next character in the selection screen.
    /// Deactivates the current character and activates the next one in a circular manner.
    /// </summary>
    public void NextCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter = (selectedCharacter + 1) % characters.Length;
        characters[selectedCharacter].SetActive(true);
    }

    /// <summary>
    /// Switches to the previous character in the selection screen.
    /// Deactivates the current character and activates the previous one in a circular manner.
    /// </summary>
    public void PreviusCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if (selectedCharacter < 0)
        {
            selectedCharacter += characters.Length;
        }
        characters[selectedCharacter].SetActive(true);
    }

    /// <summary>
    /// Starts the game with the selected character.
    /// Saves the selected character index to PlayerPrefs and loads the game scene.
    /// </summary>
    public void StartGame()
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }
}
