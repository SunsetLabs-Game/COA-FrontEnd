using TMPro;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class CharacterDataJSON
{
    public string characterName;
}

[System.Serializable]
public class CharacterDataList
{
    public List<CharacterDataJSON> characterDataJsonList;
}

public class InputFieldUI : MonoBehaviour
{
    CharacterSelectionMenu characterSelectionMenu;

    private string filePath;
    [HideInInspector] public CharacterUIDisplaySlot currentUIDisplaySlot;

    [Header("UI Buttons")]
    [SerializeField] private Button submitButton;
    [SerializeField] private Button cancelButton;

    [Header("Text Parameters")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TMP_InputField inputField;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "CharacterData.json");
    }

    public void Initialize(CharacterSelectionMenu CSM)
    {
        characterSelectionMenu = CSM;

        submitButton.onClick.AddListener(() => SubmitButton());
        cancelButton.onClick.AddListener(() => CancelButton());

        LoadCharacterData();
        DisplayInputField(null, null, false);
    }

    private void SubmitButton()
    {
        string inputtedText = inputField.text;

        characterSelectionMenu.currentCharacterData.characterName = inputtedText;
        currentUIDisplaySlot.UpdateDisplaySlot(inputtedText);
        SaveCharacterData();


        characterSelectionMenu.EditNameButton.gameObject.SetActive(true);
        characterSelectionMenu.SelectCharacterButton.gameObject.SetActive(true);
        gameObject.SetActive(false);

        CharacterAnimationController displayedCharacter = characterSelectionMenu.displayedCharacter;
        if(displayedCharacter.performingAction != true)
        {
            displayedCharacter.PlayTargetAnimation("Excited", true);
        }
    }

    private void CancelButton()
    {
        characterSelectionMenu.EditNameButton.gameObject.SetActive(true);
        characterSelectionMenu.SelectCharacterButton.gameObject.SetActive(true);
        gameObject.SetActive(false);

        CharacterAnimationController displayedCharacter = characterSelectionMenu.displayedCharacter;
        if (displayedCharacter.performingAction != true)
        {
            displayedCharacter.PlayTargetAnimation("Downcast", true);
        }
    }

    private void LoadCharacterData()
    {
        if(File.Exists(filePath) != true)
        {
            return;
        }

        string jsonData = File.ReadAllText(filePath);
        CharacterDataList loadedData = JsonUtility.FromJson<CharacterDataList>(jsonData);

        if(loadedData == null)
        {
            return;
        }

        for(int i = 0; i < loadedData.characterDataJsonList.Count; i++)
        {
            string name = loadedData.characterDataJsonList[i].characterName;
            characterSelectionMenu.characterUIDisplaySlots[i].UpdateDisplaySlot(name);
        }
    }

    private void SaveCharacterData()
    {
        List<CharacterDataJSON> dataToSave = new List<CharacterDataJSON>();

        for(int i = 0; i < characterSelectionMenu.characterDatas.Length; i++)
        {
            CharacterData characterData = characterSelectionMenu.characterDatas[i];

            dataToSave.Add(new CharacterDataJSON{ characterName = characterData.characterName } );
            string jsonString = JsonUtility.ToJson(new CharacterDataList { characterDataJsonList = dataToSave }, true);
            File.WriteAllText(filePath, jsonString);
        }
    }

    public void DisplayInputField(string titleText, string currentInputText, bool shouldDisplayInput)
    {
        gameObject.SetActive(shouldDisplayInput);

        title.text = titleText;
        inputField.text = currentInputText;
    }
}
