using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the saving and loading of player data, specifically handling player name storage
/// using Unity's PlayerPrefs system. This script is typically attached to a UI manager in
/// scenes where player data needs to be persisted.
/// </summary>
public class Save : MonoBehaviour
{
    /// <summary>
    /// Input field where the player enters their name
    /// </summary>
   public TMP_InputField inputField;
   /// <summary>
    /// Text component used to display warning messages to the user
    /// </summary>
    public TMP_Text warningText;
    /// <summary>
    /// Text component that displays the current player name
    /// </summary>
    public TMP_Text playerNameText;

    /// <summary>
    /// Saves the player's name to PlayerPrefs if the input field is not empty.
    /// Displays a warning message if no name is entered.
    /// </summary>
    public void SaveData()
    {
        string playerName = inputField.text;

        // Check if the text field is empty
        if (string.IsNullOrEmpty(playerName))
        {
            // If empty, display warning and do not save
            Debug.LogWarning("The name field is empty. Please enter a name.");
            
            // Display warning message in UI if exists
            if (warningText != null)
            {
                warningText.text = "Please enter your name before saving.";
            }
            return; // Exit method without saving
        }

        // Save the name in PlayerPrefs
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        Debug.Log("Saved name: " + playerName);

        // Clear warning message if saved correctly
        if (warningText != null)
        {
            warningText.text = "";
        }
    }
   
   /// <summary>
   /// Loads the previously saved player name from PlayerPrefs and displays it
   /// in the input field. Shows a warning if no data exists.
   /// </summary>
   public void LoadData()
{
    if (PlayerPrefs.HasKey("PlayerName"))
    {
        inputField.text = PlayerPrefs.GetString("PlayerName");
        Debug.Log("Name uploaded: " + inputField.text);
    }
    else
    {
        Debug.LogWarning("No saved data to load.");
    }
}

  /// <summary>
  /// Deletes the stored player name from PlayerPrefs and clears the input field.
  /// Shows a warning if no data exists to delete.
  /// </summary>
  public void DeleteData()
{
    if (PlayerPrefs.HasKey("PlayerName"))
    {
        PlayerPrefs.DeleteKey("PlayerName");
        inputField.text = "";
        Debug.Log("Name deleted.");
    }
    else
    {
        Debug.LogWarning("There is no data saved for deletion.");
    }
}

    /// <summary>
    /// Transitions the game to the character selection scene.
    /// This method is typically called after the player's name has been saved.
    /// </summary>
    public void LoadCharacterSelectionScene()
    {
        SceneManager.LoadScene("DemoSelectionCharacter");

    }
}
