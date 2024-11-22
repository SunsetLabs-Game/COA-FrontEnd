using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Controls NPC dialogue interactions and display of interaction prompts.
/// This script should be attached to NPC GameObjects to enable player-NPC dialogue interactions.
/// </summary>
public class NPCInteraction : MonoBehaviour
{
    /// <summary>
    /// UI panel that contains the dialogue elements
    /// </summary>
    public GameObject dialogueUI;
    /// <summary>
    /// Text component that displays the NPC's dialogue
    /// </summary>
    public TextMeshProUGUI dialogueText;
    /// <summary>
    /// UI element showing the interaction prompt (e.g., "Press E to interact")
    /// </summary>    
    public GameObject interactionText;
    /// <summary>
    /// Array of possible greeting messages that the NPC can display
    /// </summary>
    public string[] greetings = { "Hola, bienvenido a Citizen of Arcanis!", "�Qu� tal el d�a?", "�Es un placer verte!", "Espero que disfrutes tu aventura.", "�Necesitas ayuda?" };
    private bool isPlayerNearby = false;
    private Coroutine typingCoroutine;

    /// <summary>
    /// Initializes the dialogue UI elements to their default hidden state
    /// </summary>
    void Start()
    {
        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
        }
    }

    /// <summary>
    /// Handles player entering the NPC's interaction zone
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactionText != null)
            {
                interactionText.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Handles player leaving the NPC's interaction zone
    /// </summary>
    /// <param name="other">The collider that exited the trigger zone</param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactionText != null)
            {
                interactionText.SetActive(false);
            }
            if (dialogueUI != null)
            {
                dialogueUI.SetActive(false);
            }
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
        }
    }

    /// <summary>
    /// Checks for player input to trigger dialogue interaction
    /// </summary>
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TogglePanel();
        }
    }

    /// <summary>
    /// Toggles the dialogue panel visibility and manages the typing animation
    /// </summary>
    void TogglePanel()
    {
        if (dialogueUI != null)
        {
            bool isActive = dialogueUI.activeSelf;
            dialogueUI.SetActive(!isActive);

            if (!isActive)
            {
                if (dialogueText != null && greetings.Length > 0)
                {
                    int randomIndex = Random.Range(0, greetings.Length);
                    string message = greetings[randomIndex];

                    if (typingCoroutine != null)
                    {
                        StopCoroutine(typingCoroutine);
                    }

                    typingCoroutine = StartCoroutine(TypeText(message));
                }
            }
            else
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }
                dialogueText.text = "";
            }

            if (interactionText != null)
            {
                interactionText.SetActive(!dialogueUI.activeSelf);
            }
        }
    }

    /// <summary>
    /// Creates a typing animation effect for displaying dialogue text
    /// </summary>
    /// <param name="message">The message to be displayed character by character</param>
    /// <returns>IEnumerator for the coroutine system</returns>
    IEnumerator TypeText(string message)
    {
        dialogueText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.10f);
        }
    }
}
