using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketingItemSlotUI : MonoBehaviour
{
    private MarketingSystemPanel marketingSystemPanel;

    public bool isTrending { get; private set; }
    public MarketItem marketItem { get; private set; }

    [Header("Item Parameters")]
    [SerializeField] public Image bg;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;

    [Header("UI")]
    [SerializeField] private Button button;

    public void Initialize(MarketingSystemPanel MSP, MarketItem MI)
    {
        marketItem = MI;
        isTrending = MI.isTrending;
        marketingSystemPanel = MSP;

        itemName.text = marketItem.itemName;
        itemImage.sprite = marketItem.ItemImage;

        button.onClick.AddListener(() => SelectItem());
    }

    private void SelectItem()
    {
        marketingSystemPanel.SetCurrentItem(this);

        for(int i = 0; i < marketingSystemPanel.slotUIList.Count; i++)
        {
            Image slotBg = marketingSystemPanel.slotUIList[i].bg;

            if(slotBg == bg)
            {
                slotBg.color = Color.green;
                continue;
            }
            slotBg.color = Color.white;
        }
    }
}
