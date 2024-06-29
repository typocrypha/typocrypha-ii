using Gameflow;
using System;
//using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#region Data Containers

[System.Serializable]
public struct TallyEntry
{
    public string label; //value description
    public int value; //contributes to the total
    public bool isPercentage; //whether value affects base total or final multiplier

    public TallyEntry(string label, int value, bool isPercentage)
    {
        this.label = label;
        this.value = value;
        this.isPercentage = isPercentage;
    }
}

public struct BonusEntry : IComparable<BonusEntry>
{
    public const int criticalPriority = 100;
    public const int criticalPriority2 = 99;
    public const int criticalPriority3 = 98;
    public const int highPriority = 50;
    public const int highPriority2 = 49;
    public const int highPriority3 = 48;
    public const int lowPriority = 3;
    public const int lowPriority2 = 2;
    public const int lowPriority3 = 1;
    public string badgeName; //badge name
    public string unlockReason; //how the badge was earned
    public string description; //describes the effects of wearing the badge
    public int priority; //for order of appearance (higher comes first)
    public string clarkeText; //what clarke says about this item
    public Sprite iconSprite;

    public BonusEntry(string badgeName, string unlockReason, string description, Sprite icon, int priority = 0, string clarkeText = "")
    {
        this.badgeName = badgeName;
        this.unlockReason = unlockReason;
        this.description = description;
        this.priority = priority;
        this.clarkeText = clarkeText;
        this.iconSprite = icon;
    }

    public int CompareTo(BonusEntry other)
    {
        if (priority == other.priority)
        {
            return string.Compare(badgeName, other.badgeName, StringComparison.OrdinalIgnoreCase);
        }
        return other.priority.CompareTo(priority);
    }
}

#endregion Data Containers

public class RewardsManager : MonoBehaviour
{
    private const int DEFEAT_REWARD = 10;
    private const int KILL_REWARD = 20;

    public static RewardsManager Instance;

    public int DemonCasualties => casualtyCount;
    public int DemonsDefeated => casualtyCount - killCount;
    public int DemonsKilled => killCount;

    private int casualtyCount;
    private int killCount;

    private bool rewardCasualties;
    private TallyEntry[] fixedRewards;

    public IReadOnlyList<BonusEntry> BonusEntries => bonusEntries;
    private readonly List<BonusEntry> bonusEntries = new List<BonusEntry>();

    private void Awake()
    {
        if (Instance != null) Destroy(this);
        Instance = this;
    }

    public void Initialize()
    {
        ClearUnlockEntries();
        ClearCasualtyCounters();
    }

    public void ParseNode(VictoryScreenNode victoryNode)
    {
        fixedRewards = victoryNode.Entries;
        rewardCasualties = victoryNode.RewardCasualties;
    }

    public void ClearUnlockEntries() => bonusEntries.Clear();

    public void AddBonusEntry(string name, string reason, string description, Sprite icon, int priority = 0, string clarkeMessage = "")
    {
        bonusEntries.Add(new BonusEntry(name, reason, description, icon, priority, clarkeMessage));
        bonusEntries.Sort();
    }

    public void ClearCasualtyCounters() => casualtyCount = killCount = 0;
    public void IncrementCasualty() => ++casualtyCount;
    public void IncrementKill() => ++killCount;

    public int GetDefeatReward() => rewardCasualties ? DEFEAT_REWARD * DemonsDefeated : 0;
    public int GetKillReward() => rewardCasualties ? KILL_REWARD * DemonsKilled : 0;

    public static int CalculateReward(TallyEntry[] entries)
    {
        float total = 0, percentMultiplier = 0;
        for (int i = 0; i < entries.Length; ++i)
        {
            if (!entries[i].isPercentage) total += entries[i].value;
            else percentMultiplier += entries[i].value;
        }
        total *= 1 + percentMultiplier / 100;
        return Mathf.FloorToInt(total);
    }

    private TallyEntry[] CreateDynamicEntries()
    {
        return new TallyEntry[]
        {
            new TallyEntry("Demons Defeated x" + DemonsDefeated, GetDefeatReward(), false),
            new TallyEntry("Demons Killed x" + DemonsKilled, GetKillReward(), false)
        };
    }

    public TallyEntry[] GetAllTallies()
    {
        var dynamicTallies = CreateDynamicEntries();
        var fixedTallies = fixedRewards;
        return dynamicTallies.Concat(fixedTallies).ToArray();
    }

    public int GetTotalReward() => CalculateReward(GetAllTallies());
}
