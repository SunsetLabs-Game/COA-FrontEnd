using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CharacterSelectionMenu : MonoBehaviour
{
    int selectedCharacterIndex;

    [Header("Parameters")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private CharacterUIDisplaySlot characterUIDisplaySlot;
    [field: SerializeField] public CharacterData[] characterDatas {  get; private set; }

    [Header("Displayed Character & UI")]
    public InputFieldUI inputFieldUI;
    public CharacterData currentCharacterData;
    public CharacterAnimationController displayedCharacter;
    [field: SerializeField] public Button EditNameButton { get; private set; }
    [field: SerializeField] public Button SelectCharacterButton { get; private set; }

    [Header("Spawn Positions")]
    [SerializeField] private Transform contentDrawer;
    [SerializeField] private Transform displayedCharacterSpawnPoint;

    [Header("Character Selection Lists")]
    public List<GameObject> characterInstances = new List<GameObject>();
    public List<CharacterUIDisplaySlot> characterUIDisplaySlots = new List<CharacterUIDisplaySlot>();

    private void Awake()
    {
        characterDatas = Resources.LoadAll<CharacterData>("Character Data");
    }

    private void Start()
    {
        SpawnCharacterDisplayUI();
        inputFieldUI.Initialize(this);
        EditNameButton.onClick.AddListener(() => DisplayInputField());
        SelectCharacterButton.onClick.AddListener(() => SelectButton());
    }

    private void Update()
    {
        HandleDisplayedCharacter();
    }

    private void DisplayInputField()
    {
        string displayName = (currentCharacterData != null) ? currentCharacterData.characterName : "";
        inputFieldUI.DisplayInputField("Edit Character Name", displayName, true);

        EditNameButton.gameObject.SetActive(false);
        SelectCharacterButton.gameObject.SetActive(false);
    }

    private void HandleDisplayedCharacter()
    {
        if(currentCharacterData == null)
        {
            EditNameButton.gameObject.SetActive(false);
            inputFieldUI.DisplayInputField(null, null, false);
        }

        //HandleRotation
        displayedCharacterSpawnPoint.RotateAround(displayedCharacterSpawnPoint.position, Vector3.up, turnSpeed * Time.deltaTime);
        EditNameButton.gameObject.SetActive(true);
    }

    private void SpawnCharacterDisplayUI()
    {
        for(int i = 0; i < characterDatas.Length; i++)
        {
            CharacterUIDisplaySlot uIDisplaySlot = Instantiate(characterUIDisplaySlot, contentDrawer);

            uIDisplaySlot.Initialize(characterDatas[i], this);
            CharacterAnimationController characterInstance = Instantiate(characterDatas[i].displayedCharacter, displayedCharacterSpawnPoint);

            characterInstance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            characterInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            characterInstance.gameObject.SetActive(false);

            characterUIDisplaySlots.Add(uIDisplaySlot);
            characterInstances.Add(characterInstance.gameObject);
        }
    }

    private void SelectButton()
    {
        if(currentCharacterData == null)
        {
            return;
        }

        for(int i = 0; i < characterDatas.Length; i++)
        {
            if(currentCharacterData == characterDatas[i])
            {
                selectedCharacterIndex = i;
                break;
            }
        }

        PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
}
