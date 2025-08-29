using UnityEngine;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private WaitForSeconds exitPanelSeconds;
    public CharacterManager character { get; private set; }

    // Tags
    private const string PLAYER_TAG = "Player";
    private const string DUEL_TAG = "Mode: Duel";

    private GameObject player;
    private GameObject currentSpeaker;
    private bool duelTriggered = false; // Flag to track if a duel should start

    // Status
    public bool canContinue;

    public bool skipDialogue;
    public bool dialogueIsPlaying { get; private set; }
    public Story currentDialogueStory { get; private set; }

    [Header("Dialogue Parameters")]
    [SerializeField] private DialoguePanel dialogueUIPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("A duplicate DialogueManager was found and destroyed.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialogueUIPanel.ExitPanel();
        exitPanelSeconds = new WaitForSeconds(0.2f);
    }

    private void Update()
    {
        if(!dialogueIsPlaying)
        {
            return;
        }

        if (skipDialogue)
        {
            skipDialogue = false;
            StartCoroutine(ExitDialogueMode());
        }

        if (InputManager.Instance.jumpInput && canContinue && currentDialogueStory.currentChoices.Count == 0)
        {
            ContinueDialogueStory();
        }
    }

    public void HandleDialogue(TextAsset inkJsonStory, CharacterManager npc = null)
    {
        if (npc != null)
        {
            character = npc;
        }

        if (NPCController.Instance != null && NPCController.Instance.Mercenary != null)
        {
            NPCController.Instance.Mercenary.hasAssignment = false;
        }
        dialogueIsPlaying = true;
        dialogueUIPanel.gameObject.SetActive(true);
        dialogueUIPanel.DisableUIChoices();
        currentDialogueStory = new Story(inkJsonStory.text);

        duelTriggered = false; // Reset duel flag
        ContinueDialogueStory();
    }

    private void ContinueDialogueStory()
    {
        if (currentDialogueStory.canContinue)
        {
            dialogueUIPanel.StopDisplayCoroutine();

            string text = currentDialogueStory.Continue();
            CheckWhoIsSpeaking(currentDialogueStory.currentTags);
            CheckForDuel(currentDialogueStory.currentTags); // Check if a duel should be triggered

            if (text.Equals("") && !currentDialogueStory.canContinue)
            {
                StartCoroutine(ExitDialogueMode());
            }

            if (currentSpeaker != null)
            {
                dialogueUIPanel.DisplayChoicesUI(currentDialogueStory);
                dialogueUIPanel.DisplayText(currentSpeaker, text);
            }
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return exitPanelSeconds;
        
        dialogueIsPlaying = false;
        character.dontMove = false;
        currentDialogueStory = null;
        character.isTalking = false;

        dialogueUIPanel.ExitPanel();
        character.Agent.enabled = true;

        // If a duel was triggered, inform CombatManager and load the combat scene
        if (duelTriggered)
        {
            CombatManager.Instance.StartDuel(NPCController.Instance.Player, character);
        }
    }

    private void CheckWhoIsSpeaking(List<string> currentTag)
    {
        foreach (string tag in currentTag)
        {
            string[] splitTag = tag.Split(':');

            if (splitTag.Length != 2)
            {
                Debug.LogError("Error Parsing Tag: " + tag);
                return;
            }
            string tagValue = splitTag[1].Trim();
            SetCharacter(tagValue);
        }
    }

    private void CheckForDuel(List<string> currentTag)
    {
        foreach (string tag in currentTag)
        {
            if (tag.Trim().Equals(DUEL_TAG))
            {
                duelTriggered = true;
                return;
            }
        }
    }

    private void SetCharacter(string tagValue)
    {
        if (tagValue.Equals(PLAYER_TAG))
        {
            currentSpeaker = player;
            return;
        }
        currentSpeaker = character.gameObject;
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        currentDialogueStory.ChooseChoiceIndex(choiceIndex);
        ContinueDialogueStory();
    }
}
