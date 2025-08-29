using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManagerPanel_UI : MonoBehaviour
{
    private bool hasInitialized;
    
    private float inactivityTimer;
    public bool IsMouseOverPanel { get; private set; }
    
    public List<InventorySlotUI> currencySlotList { get;  private set; }
    public List<InventorySlotUI> collectiblesSlotList { get; private set; }

    public Camera MainCamera { get; private set; }
    public CanvasGroup CanvasGrp { get; private set; }

    //public static InventoryManagerPanel_UI Instance { get; private set; }

    [Header("Parameters")]
    [SerializeField] private Transform currencySlotParent;
    [SerializeField] private Transform collectiblesSlotParent;
    [Range(1f, 10f)] [SerializeField] private float idleTime;

    [Header("Panel UI Parameters")]
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private List<NotificationPanel> notifPanels;

    private void Awake()
    {
        IsMouseOverPanel = false;
        MainCamera = Camera.main;
        CanvasGrp = GetComponent<CanvasGroup>();

        notifPanels = new(GetComponentsInChildren<NotificationPanel>());
        currencySlotList = new(currencySlotParent.GetComponentsInChildren<InventorySlotUI>());
        collectiblesSlotList = new(collectiblesSlotParent.GetComponentsInChildren<InventorySlotUI>());
    }

    private void OnEnable()
    {
        if(hasInitialized)
        {
            return;
        }
        hasInitialized = true;

        DisablePanel();
        closeButton.onClick.AddListener(DisablePanel);
        notifPanels.ForEach(x => x.gameObject.SetActive(false));
    }

    private void Update()
    {
        if(IsMouseOverPanel)
        {
            inactivityTimer = 0.0f;
            return;
        }

        if(inventoryPanel.activeSelf)
        {
            inactivityTimer += Time.deltaTime;
            if(inactivityTimer >= idleTime)
            {
                DisablePanel();
            }
        }
    }

    public void HandleSlotInitialization(ItemClass itemClass)
    {
        PickableObject pickedObj = itemClass.pickedObj;

        if(pickedObj.ItemType == ItemType.Currency)
        {
            InitializeSlotUI(itemClass, currencySlotList);
            return;
        }
        InitializeSlotUI(itemClass, collectiblesSlotList);
    }

    public void HandSlotUpdate(float alphaValue, InventorySlotUI slotUI)
    {
        EnablePanel();
        slotUI.UpdateSlotUI(alphaValue);
    }

    private void InitializeSlotUI(ItemClass itemClass, List<InventorySlotUI> slotList)
    {
        InventorySlotUI slotUI = FindInactiveSlot(slotList);

        if(slotUI == null)
        {
            DisplayNotification("No More Collectible Space, Remove An Item");
            return;
        }

        itemClass.SetSlotUI(slotUI);
        slotUI.Initialize(itemClass);

	    EnablePanel();
        DisplayNotification($"{itemClass.pickedObj.ItemName} Has Been Added To Inventory");
    }

    public void DisplayNotification(string text)
    {
        NotificationPanel notif = notifPanels.Find(x => x.gameObject.activeSelf != true);

        if(notif != null)
        {
            notif.ShowNotification(text);
            return;
        }
        notifPanels[0].ShowNotification(text);
    }

    public void ResetInactiveTime()
    {
        inactivityTimer = 0.0f;
    }

    public void EnablePanel()
    {
        inventoryPanel.SetActive(true);
    }

    public void DisablePanel()
    {
        ResetInactiveTime();
        inventoryPanel.SetActive(false);
    }

    public InventorySlotUI FindInactiveSlot(List<InventorySlotUI> slotList)
    {
        return slotList.Find(x => x.IsActive != true);
    }
}
