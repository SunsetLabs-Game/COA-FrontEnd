using UnityEngine;

[CreateAssetMenu(fileName = "Market Item", menuName = "Market Item")]
public class MarketItem : ScriptableObject
{
    [Header("Parameters")]
    public string itemName;
    [TextArea(3, 6)] public string ItemDescription;
    [field: SerializeField] public bool isTrending { get; private set; }
    [field: SerializeField] public float ItemPrice { get; private set; }
    [field: SerializeField] public Sprite ItemImage { get; private set; }
    [field: SerializeField] public GameObject ItemObject { get; private set; }
}
