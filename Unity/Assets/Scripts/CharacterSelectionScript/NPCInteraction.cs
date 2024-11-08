using UnityEngine;
using TMPro;
using System.Collections;

public class NPCInteraction : MonoBehaviour
{
    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;
    public GameObject interactionText;
    public string[] greetings = { "Hola, bienvenido a Citizen of Arcanis!", "¿Qué tal el día?", "¡Es un placer verte!", "Espero que disfrutes tu aventura.", "¿Necesitas ayuda?" };
    private bool isPlayerNearby = false;
    private Coroutine typingCoroutine;

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

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TogglePanel();
        }
    }

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
