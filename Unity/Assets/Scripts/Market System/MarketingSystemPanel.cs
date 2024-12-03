using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MarketingSystemPanel : MonoBehaviour
{
    private MarketItem[] marketItemArray;
    private MarketingItemSlotUI currentSelectedItem;
    public List<MarketingItemSlotUI> slotUIList = new List<MarketingItemSlotUI>();

    [Header("Panels")]
    [SerializeField] private GameObject trendingPanel;
    [SerializeField] private GameObject descriptionPanel;

    [Header("General Parameters")]
    [SerializeField] private Button searchButton;
    [SerializeField] private Button closeMarketButton;
    [SerializeField] private TMP_InputField searchInputField;
    [SerializeField] private Button viewDescriptionPanelButton;

    [Header("Buy Panel")]
    [SerializeField] private Button buyButton;
    [SerializeField] private Image buy_ItemImage;
    [SerializeField] private TextMeshProUGUI buy_ItemName;
    [SerializeField] private TextMeshProUGUI buy_ItemPrice;

    [Header("Description Panel")]
    [SerializeField] private Image description_ItemImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI description_ItemName;
    [SerializeField] private Button disableDescriptionPanelButton;
    [SerializeField] private TextMeshProUGUI description_ItemPrice;

    [Header("Parameters")]
    [SerializeField] private Transform trendingParent;
    [SerializeField] private Transform allItemsParent;
    [SerializeField] private MarketingItemSlotUI marketingItemSlotUI;

    private void Awake()
    {
        marketItemArray = Resources.LoadAll<MarketItem>("Market Items");
    }

    private void OnEnable()
    {
        if(Cursor.visible)
        {
            return;
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Start()
    {
        descriptionPanel.gameObject.SetActive(false);
        SpawnItemUI();

        searchButton.onClick.AddListener(() => HandleSearch());
        closeMarketButton.onClick.AddListener(() => ExitMarketPanel());
        viewDescriptionPanelButton.onClick.AddListener(() => EnableDescriptionPanel(true));
        disableDescriptionPanelButton.onClick.AddListener(() => EnableDescriptionPanel(false));
    }

    private void OnDisable()
    {
        if (Cursor.visible != true)
        {
            return;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetCurrentItem(MarketingItemSlotUI MISU)
    {
        currentSelectedItem = MISU;
        MarketItem marketItem = currentSelectedItem.marketItem;

        //Buy Panel
        buy_ItemName.text = marketItem.itemName;
        buy_ItemImage.sprite = marketItem.ItemImage;
        buy_ItemPrice.text = "Price: " + marketItem.ItemPrice.ToString();

        //Display panel
        description_ItemName.text = marketItem.itemName;
        descriptionText.text = marketItem.ItemDescription;
        description_ItemImage.sprite = marketItem.ItemImage;
        description_ItemPrice.text = "Price: " + marketItem.ItemPrice.ToString();
    }

    private void HandleSearch()
    {
        string searchedParameter = searchInputField.text;

        for(int i = 0; i < slotUIList.Count; i++)
        {
            MarketingItemSlotUI slotUI = slotUIList[i];
            MarketItem marketItem = slotUI.marketItem;

            bool hasQuery = marketItem.itemName.ToLower().Contains(searchedParameter.ToLower());
            if(hasQuery)
            {
                slotUI.gameObject.SetActive(true);
                continue;
            }
            slotUI.gameObject.SetActive(false);
        }
    }

    private void EnableDescriptionPanel(bool value)
    {
        trendingPanel.gameObject.SetActive(!value);
        descriptionPanel.gameObject.SetActive(value);
    }

    private void ExitMarketPanel()
    {
        gameObject.SetActive(false);
    }

    private void SpawnItemUI()
    {
        for(int i = 0; i < marketItemArray.Length; i++)
        {
            if (marketItemArray[i].isTrending)
            {
                MarketingItemSlotUI trendingSlotUI = Instantiate(marketingItemSlotUI, trendingParent);
                trendingSlotUI.Initialize(this, marketItemArray[i]);

                slotUIList.Add(trendingSlotUI);
            }

            MarketingItemSlotUI slotUI = Instantiate(marketingItemSlotUI, allItemsParent);
            slotUI.Initialize(this, marketItemArray[i]);

            slotUIList.Add(slotUI);
        }
    }
}
