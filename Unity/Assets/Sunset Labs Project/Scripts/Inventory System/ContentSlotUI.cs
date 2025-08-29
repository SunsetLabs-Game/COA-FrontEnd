using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ContentSlotUI : MonoBehaviour
{
    private float elapsed;
    private PickableObject selectedItem;

    private ItemClass finalReward;
    public bool hasRevealed {get; private set;}
    private List<PickableObject> itemsList = new();

    [Header("Item Properties")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private float shuffleDuration;

    [Header("Slot UI Parameters")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemCountUI;

    public void Initialize(ItemClass reward, PickableObject[] itemArray)
    {
        itemsList.Clear();
        hasRevealed = false;

        finalReward = reward;
        itemsList = new List<PickableObject>(itemArray);
    }

    public void DisplayContent(int count, PickableObject item)
    {
        itemName.text = item.ItemName;
        itemIcon.sprite = item.ItemImage;
        itemCountUI.text = count.ToString("00");
    }

    public IEnumerator RevealRandomItem()
    {
        itemIcon.color = Color.white;

        elapsed = 0f;
        while(elapsed < shuffleDuration)
        {
            int count = Random.Range(1, 26);
            selectedItem = GetRandomExcluding(selectedItem);
            DisplayContent(count, selectedItem);

            yield return new WaitForSeconds(0.05f);
            elapsed += Time.deltaTime;
        }
        DisplayContent(finalReward.itemCount, finalReward.pickedObj);
        hasRevealed = true;
    }

    private PickableObject GetRandomExcluding(PickableObject exclude)
    {
        var indexList = new List<int>();
        for (int i = 0; i < itemsList.Count; i++)
        {
            if (itemsList[i] == null || itemsList[i] == exclude)
            {
                continue;
            }
            indexList.Add(i);
        }
        if (indexList.Count == 0)
        {
            return itemsList[Random.Range(0,itemsList.Count)];
        }
        int rnd = Random.Range(0, indexList.Count);
        return itemsList[indexList[rnd]];
    }
}
