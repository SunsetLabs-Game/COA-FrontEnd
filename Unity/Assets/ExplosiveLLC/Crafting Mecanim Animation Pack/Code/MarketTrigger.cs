using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class MarketTrigger : MonoBehaviour
{
    public GameObject tradeText;         // "Trade" text that appears when entering the zone
    public GameObject marketUI;          // Market UI
    public TextMeshProUGUI statsText;    // Text area for displaying weapon stats
    private bool isPlayerNearby = false; // Flag to check if the player is in the zone

    // Initializes the market system, hiding the UI and trade text, and clearing the stats text
    void Start()
    {
        tradeText.SetActive(false);
        marketUI.SetActive(false);
        statsText.text = "";
    }

    // Detects if the player enters the trigger zone and shows the "Trade" text
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tradeText.SetActive(true);
            isPlayerNearby = true;
        }
    }

    // Detects if the player exits the trigger zone and hides the "Trade" text and market UI
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tradeText.SetActive(false);
            isPlayerNearby = false;
            CloseMarketWindow();
        }
    }

    // Updates every frame to check if the player presses the "E" key to open/close the market
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!marketUI.activeSelf)
            {
                OpenMarketWindow();
            }
            else
            {
                CloseMarketWindow();
            }
        }
    }

    // Opens the market UI
    void OpenMarketWindow()
    {
        marketUI.SetActive(true);
    }

    // Closes the market UI
    public void CloseMarketWindow()
    {
        marketUI.SetActive(false);
    }

    // Displays the stats of the sword in the statsText area
    public void ShowSwordStats()
    {
        statsText.text = "Weapon: Sword\n\nAttack: 4\nDefense: 0\nSpeed: 3\nStrength: 6";
    }

    // Displays the stats of the katana in the statsText area
    public void ShowKatanaStats()
    {
        statsText.text = "Weapon: Katana\n\nAttack: 3\nDefense: 0\nSpeed: 7\nStrength: 2";
    }
}
