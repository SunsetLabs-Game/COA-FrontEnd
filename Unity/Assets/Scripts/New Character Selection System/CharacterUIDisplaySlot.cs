using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIDisplaySlot : MonoBehaviour
{
    private CharacterSelectionMenu characterSelectionMenu;
    public CharacterData characterData { get; private set; }

    [Header("UI Objects")]
    public Button uiButton;

    [Header("Parameters")]
    [SerializeField] private Image displayImage;
    [field: SerializeField] public TextMeshProUGUI DisplayName { get; private set; }

    public void Initialize(CharacterData CD, CharacterSelectionMenu CSM)
    {
        characterData = CD;
        characterSelectionMenu = CSM;

        UpdateDisplaySlot(characterData.characterName);
        uiButton.onClick.AddListener(() => DisplayCharacter());
    }

    public void UpdateDisplaySlot(string name)
    {
        characterData.characterName = name;

        DisplayName.text = characterData.characterName;
        displayImage.sprite = characterData.displayImage;
    }

    public void DisplayCharacter()
    {
        for (int i = 0; i < characterSelectionMenu.characterUIDisplaySlots.Count; i++) 
        {
            GameObject characterInstance = characterSelectionMenu.characterInstances[i];
            CharacterUIDisplaySlot uIDisplaySlot = characterSelectionMenu.characterUIDisplaySlots[i];

            if(uIDisplaySlot == this)
            {
                characterSelectionMenu.inputFieldUI.currentUIDisplaySlot = this;
                characterSelectionMenu.currentCharacterData = uIDisplaySlot.characterData;
                characterSelectionMenu.displayedCharacter = uIDisplaySlot.characterData.displayedCharacter;
            }
            characterInstance.SetActive(uIDisplaySlot == this);
            uIDisplaySlot.uiButton.image.color = (uIDisplaySlot == this) ? Color.green : Color.blue;
        }
    }
}
