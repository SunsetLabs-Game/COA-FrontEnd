using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class MarketTrigger : MonoBehaviour
{
    public GameObject tradeText;         
    public GameObject marketUI;          
    public TextMeshProUGUI statsText;    
    private bool isPlayerNearby = false; 

    // This method is called once at the start of the game. It hides the trade text and the market UI and clears the stats text.
    void Start()
    {
        tradeText.SetActive(false);  
        marketUI.SetActive(false);  
        statsText.text = "";         
    }

    // This method is triggered when another object enters the trigger collider attached to the object that this script is on.
    // It checks if the player has entered the area, shows the "Trade" text, and sets a flag to indicate that the player is nearby.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  
        {
            tradeText.SetActive(true);   
            isPlayerNearby = true;       
        }
    }

    // This method is triggered when another object exits the trigger collider attached to the object that this script is on.
    // It checks if the player has left the area, hides the "Trade" text, and sets a flag to indicate that the player is no longer nearby.
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  
        {
            tradeText.SetActive(false);  
            isPlayerNearby = false;      
            CloseMarketWindow();         
        }
    }

    // This method is called once per frame. It checks if the player is nearby and presses the "E" key.
    // If the market UI is not visible, it opens the market window. If it's already visible, it closes the market window.
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

    // This method opens the market UI and logs a message to confirm that the market window has opened.
    void OpenMarketWindow()
    {
        marketUI.SetActive(true); 
    }

    // This method closes the market UI and logs a message to confirm that the market window has closed.
    public void CloseMarketWindow()
    {
        marketUI.SetActive(false);   
    }

    // This method updates the statsText to display the attributes of the Sword weapon. It will be called when the sword button is clicked.
    public void ShowSwordStats()
    {
        statsText.text = "Weapon: Sword\n\n\nAttack: 4\n\nDefense: 0\n\nSpeed: 3\n\nStrength: 6";
    }

    // This method updates the statsText to display the attributes of the Katana weapon. It will be called when the katana button is clicked.
    public void ShowKatanaStats()
    {
        statsText.text = "Weapon: Katana\n\n\nAttack: 3\n\nDefense: 0\n\nSpeed: 7\n\nStrength: 2";
    }
}
