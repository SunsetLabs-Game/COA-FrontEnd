using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RewardSystem : MonoBehaviour
{
    public static RewardSystem Instance { get; private set; }

    private LootBox lootBox;
    public bool HasFinished { get; private set; }
    public RewardBox Reward {  get; private set; }

    [Header("Reward Parameters")]
    [SerializeField] private LootBox[] lootBoxes;
    [SerializeField] private Transform spawnPoint;

    [Header("Reward UI")]
    [SerializeField] private NotificationPanel notifPanel;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        HasFinished = false;
        Reward = new RewardBox();

        int rate = Random.Range(3, 8);
        lootBox = Instantiate(GetRandomBox(rate), spawnPoint);
    }

    public LootBox GetRandomBox(int maxRate)
    {
        int random;
        List<int> unExcludedIndex = new();
        for (int i = 0; i < lootBoxes.Length; i++)
        {
            LootBox box = lootBoxes[i];
            if (box == null || box.Rate > maxRate)
            {
                continue;
            }
            unExcludedIndex.Add(i);
        }
        random = Random.Range(0, unExcludedIndex.Count);
        LootBox selectedItem = lootBoxes[unExcludedIndex[random]];
        return selectedItem;
    }

    public IEnumerator HandleReward(CharacterManager character, DuelState state)
    {
        if (state != DuelState.Lost)
        {
            yield return new WaitForSeconds(3.5f);
            FillUpBox(character, state);
            yield return new WaitUntil(() => HasFinished);

            Reward.HandleRewarding();
            yield return new WaitUntil(() => Reward.FinishedCleaning);
        }

        state = DuelState.None;
        if (character.mentalState == CombatMentalState.Friendly)
        {
            HasFinished = false;
            LevelLoader.HandleLoadLevel("Main Scene", null, this);
        }
    }

    private void FillUpBox(CharacterManager character, DuelState state)
    {
        if (character.mentalState != CombatMentalState.Friendly)
        {
            FillLootBoxWithCharacter();
        }
        else
        {
            lootBox.FillLootBoxRandomly();
        }
        StartCoroutine(HandleFillUpRewardBox(state));
    }

    private void FillLootBoxWithCharacter()
    {
        lootBox.ItemClasses.Clear();
        CombatManager combat = CombatManager.Instance;

        int itemCount = combat.MercenaryWeapons.Length;
        for (int i = 0; i < itemCount; i++)
        {
            PickableObject pickObj = combat.MercenaryWeapons[i];
            ItemClass item = new(1, pickObj);
            lootBox.ItemClasses.Add(item);
        }
        lootBox.AddCurrencies(itemCount + 1);
    }

    private IEnumerator HandleFillUpRewardBox(DuelState duelState)
    {
        HasFinished = false;
        bool isDraw = (duelState == DuelState.Draw);

        Reward.CleanBox();
        foreach(ItemClass item in lootBox.ItemClasses)
        {
            Reward.AddItemToBox(item);
        }
        yield return new WaitUntil(() => Reward.itemsList.Count >= lootBox.ItemClasses.Count);

        ItemBox selectedBox = lootBox.SelectedBox;
        string notification = $"Congratulations You Have Won {selectedBox.name}";
        notifPanel.ShowNotification(notification, selectedBox, Reward);

        yield return new WaitUntil(() => notifPanel.gameObject.activeSelf != true);
        HasFinished = true;
        Destroy(lootBox.gameObject);
        int rate = Random.Range(3, 8);
        lootBox = Instantiate(GetRandomBox(rate), spawnPoint);
    }
}
