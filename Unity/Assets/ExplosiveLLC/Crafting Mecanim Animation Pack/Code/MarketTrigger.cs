using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Necesario para TextMeshPro

public class MarketTrigger : MonoBehaviour
{
    public GameObject tradeText;         // Texto de "Trade" que aparecerá al entrar en la zona
    public GameObject marketUI;          // La UI del Market
    public TextMeshProUGUI statsText;    // Área de texto para los stats de las armas
    private bool isPlayerNearby = false; // Flag para verificar si el jugador está en la zona

    // Inicialización
    void Start()
    {
        tradeText.SetActive(false);  // Esconde el texto "Trade" al principio
        marketUI.SetActive(false);   // Esconde el UI del market al principio
        statsText.text = "";         // Limpia el área de stats
    }

    // Detecta si el jugador entra en la zona de colisión (trigger)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Verifica si el objeto que entra es el jugador
        {
            tradeText.SetActive(true);   // Muestra el texto "Trade"
            isPlayerNearby = true;       // Marca que el jugador está cerca
        }
    }

    // Detecta si el jugador sale de la zona de colisión (trigger)
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // Verifica si el objeto que sale es el jugador
        {
            tradeText.SetActive(false);  // Esconde el texto "Trade"
            isPlayerNearby = false;      // Marca que el jugador ya no está cerca
            CloseMarketWindow();         // Cierra el market si está abierto
        }
    }

    // Actualiza cada frame
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))  // Si el jugador está cerca y presiona "E"
        {
            if (!marketUI.activeSelf)   // Si el market no está activo
            {
                OpenMarketWindow();     // Abre el market
            }
            else
            {
                CloseMarketWindow();    // Cierra el market si ya está abierto
            }
        }
    }

    // Función para abrir la ventana del market
    void OpenMarketWindow()
    {
        marketUI.SetActive(true);  // Muestra la UI del market
    }

    // Función para cerrar la ventana del market
    public void CloseMarketWindow()
    {
        marketUI.SetActive(false);  // Esconde la UI del market
    }

    // Muestra los stats de la espada
    public void ShowSwordStats()
    {
        statsText.text = "Weapon: Sword\n\nAttack: 4\nDefense: 0\nSpeed: 3\nStrength: 6";
    }

    // Muestra los stats de la katana
    public void ShowKatanaStats()
    {
        statsText.text = "Weapon: Katana\n\nAttack: 3\nDefense: 0\nSpeed: 7\nStrength: 2";
    }
}
