using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;


public class Save : MonoBehaviour
{
   public TMP_InputField inputField;
    public TMP_Text warningText; // Optional: Warning text in the UI
    public TMP_Text playerNameText;

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
    // Method for loading data
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

      // Method for loading the character selection scene
    public void LoadCharacterSelectionScene()
    {
        SceneManager.LoadScene("DemoSelectionCharacter");

    }
}
