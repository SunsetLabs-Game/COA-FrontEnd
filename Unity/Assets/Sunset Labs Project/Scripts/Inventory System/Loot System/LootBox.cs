using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootBox : MonoBehaviour, IInteractable
{
    private ItemBox itemBox;
    private readonly List<int> unExcludedIndex = new();
    private readonly HashSet<PickableObject> excludedObjects = new();

    private WaitForSeconds rewardDelay;
    private WaitForSeconds destroyDelay;
    public ItemBox SelectedBox => itemBox;

    [Header("Parameters")]
    [SerializeField] private float f_Rdelay;
    [SerializeField] private float f_Ddelay;
    [field: SerializeField] public int Rate { get; private set; }
    [SerializeField] private ItemBox[] itemBoxes;
    [SerializeField] public List<ItemClass> ItemClasses { get; private set; } = new List<ItemClass>();

    [Header("UI Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private ContentSlotUI[] contentArray;

    public void Start()
    {
        panel.SetActive(false);
        rewardDelay = new WaitForSeconds(f_Rdelay);
        destroyDelay = new WaitForSeconds(f_Ddelay);

        int rate = Random.Range(6, 12);
        itemBox = GetRandomBox(rate, itemBoxes);
    }

    public string GetInteractText()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        StartCoroutine(DisplayLoot());
    }

    private IEnumerator DisplayLoot()
    {
        DisplayUIContent();
        yield return rewardDelay;

        panel.SetActive(false);
        ItemClasses.ForEach(x => CharacterInventoryManager.Instance.HandleItemAddition(x));
        yield return destroyDelay;
        Destroy(gameObject);
    }

    private void DisplayUIContent()
    {
        FillLootBox();
        for(int i = 0; i < ItemClasses.Count; i++)
        {
            ItemClass item = ItemClasses[i];
            contentArray[i].DisplayContent(item.itemCount, item.pickedObj);
        }
        panel.SetActive(true);
    }

    private int RandomItemCount(ItemType itemType)
    {
        BoundInt bound;
        bound = (itemType == ItemType.Currency) ?
            itemBox.CurrencyCountSize : itemBox.CollectibleCountSize;
        return Random.Range(bound.minValue, bound.maxValue);
    }

    public void AddCurrencies(int itemCount)
    {
        excludedObjects.Clear();
        int c = Random.Range(0, itemBox.CurrencyItems.Length);
        int maxCount = Mathf.Max(itemCount + c, 5);

        while(ItemClasses.Count < maxCount)
        {
            int count = RandomItemCount(ItemType.Currency);
            PickableObject pickObj = GetRandomItem(itemBox.CurrencyItems, itemBox);

            if(pickObj == null)
            {
                break;
            }
            ItemClass currencyItem = new(count, pickObj);
            ItemClasses.Add(currencyItem);
        }
    }

    public void FillLootBoxRandomly()
    {
        ItemClasses.Clear();
        excludedObjects.Clear();
        int itemCount = Random.Range(0, itemBox.CollectibleItems.Length);
        
        while(ItemClasses.Count < (itemCount + 1))
        {
            int count = RandomItemCount(ItemType.Collectible);
            PickableObject pickObj = GetRandomItem(itemBox.CollectibleItems, itemBox);

            ItemClass item = new(count, pickObj);
            ItemClasses.Add(item);
        }
        AddCurrencies(itemCount + 1);
    }

    private void FillLootBox()
    {
        ItemClasses.Clear();
        excludedObjects.Clear();
        int itemCount = Random.Range(0, 4);

        while(ItemClasses.Count < itemCount)
        {
            ItemType itemType;
            PickableObject[] objs;
            bool pickCollectible = (Random.Range(0, 3) < 2);

            if(pickCollectible)
            {
                itemType = ItemType.Collectible;
                objs = itemBox.CollectibleItems;
            }
            else
            {
                itemType = ItemType.Currency;
                objs = itemBox.CurrencyItems;
            }
            int count = RandomItemCount(itemType);
            PickableObject pickObj = GetRandomItem(objs, itemBox);

            ItemClass item = new(count, pickObj);
            ItemClasses.Add(item);
        }

        while (ItemClasses.Count < 3)
        {
            int count = RandomItemCount(ItemType.Currency);
            PickableObject pickObj = GetRandomItem(itemBox.CurrencyItems, itemBox);

            ItemClass currencyItem = new(count, pickObj);
            ItemClasses.Add(currencyItem);
        }
    }

    public ItemBox GetRandomBox(int maxRate, ItemBox[] itemBoxes)
    {
        int random;
        unExcludedIndex.Clear();

        for (int i = 0; i < itemBoxes.Length; i++)
        {
            ItemBox box = itemBoxes[i];
            if (box == null || box.MaxRate > maxRate)
            { 
                continue;
            }
            unExcludedIndex.Add(i);
        }
        random = Random.Range(0, unExcludedIndex.Count);
        ItemBox selectedItem = itemBoxes[unExcludedIndex[random]];
        return selectedItem;
    }

    private PickableObject GetRandomItem(PickableObject[] itemsArray, ItemBox itemBox)
    {
        int random;
        unExcludedIndex.Clear();

        for (int i = 0; i < itemsArray.Length; i++)
        {
            PickableObject obj = itemsArray[i];
            if (obj == null || excludedObjects.Contains(obj))
            {
                continue;
            }

            random = Random.Range(2, itemBox.MaxRate);
            if (obj.rewardRate > random)
            {
                continue;
            }
            unExcludedIndex.Add(i);
        }

        if (unExcludedIndex.Count == 0)
        {
            return null;
        }
        random = Random.Range(0, unExcludedIndex.Count);
        PickableObject wonItem = itemsArray[unExcludedIndex[random]];
        excludedObjects.Add(wonItem);
        return wonItem;
    }
}
