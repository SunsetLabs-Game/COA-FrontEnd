using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Para usar TextMeshPro

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject tradeText;         // Object that holds the "Trade" text
    public GameObject marketUI;          // The Market window UI
    public TextMeshProUGUI statsText;    // Referencia al área de texto donde se mostrarán los stats
    private bool isPlayerNearby = false; // Track if the player is near

    void Start()
    {
        tradeText.SetActive(false);  // Hide the text at the start
        marketUI.SetActive(false);   // Hide the market UI at the start
        statsText.text = "";         // Clear the text at the start
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Check if the player enters the trigger zone
        {
            tradeText.SetActive(true);   // Show the "Trade" text
            isPlayerNearby = true;       // Flag that the player is nearby
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // Check if the player leaves the trigger zone
        {
            tradeText.SetActive(false);  // Hide the "Trade" text
            isPlayerNearby = false;      // Flag that the player is no longer nearby
            CloseMarketWindow();         // Close the market UI when the player leaves the zone
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))  // Check if the player presses the "E" key
        {
            if (!marketUI.activeSelf)   // If the market UI is not active
            {
                OpenMarketWindow();     // Show the market window
            }
            else
            {
                CloseMarketWindow();    // Hide the market window if it's already open
            }
        }
    }

    void OpenMarketWindow()
    {
        marketUI.SetActive(true);  // Activate the market UI
        Debug.Log("Market window opened");  // Debug message for confirmation
    }

    void CloseMarketWindow()
    {
        marketUI.SetActive(false);  // Deactivate the market UI
        Debug.Log("Market window closed");  // Debug message for confirmation
    }

    // Funciones para mostrar los stats de la espada y katana
    public void ShowSwordStats()
    {
        statsText.text = "Weapon: Sword\n\n\nAttack: 4\n\nDefense: 0\n\nSpeed: 3\n\nStrength: 6";
    }

    public void ShowKatanaStats()
    {
        statsText.text = "Weapon: Katana\n\n\nAttack: 3\n\nDefense: 0\n\nSpeed: 7\n\nStrength: 2";
    }
}
