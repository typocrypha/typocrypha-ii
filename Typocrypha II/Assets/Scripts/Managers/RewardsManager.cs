using System;
//using System.Collections;
using System.Collections.Generic;
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
    public string badgeName; //badge name
    public string unlockReason; //how the badge was earned
    public string description; //describes the effects of wearing the badge
    public int priority; //for order of appearance (higher comes first)
    public string clarkeText; //what clarke says about this item

    public BonusEntry(string badgeName, string unlockReason, string description, int priority = 0, string clarkeText = "")
    {
        this.badgeName = badgeName;
        this.unlockReason = unlockReason;
        this.description = description;
        this.priority = priority;
        this.clarkeText = clarkeText;
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
    public static RewardsManager Instance;

    private void Awake()
    {
        if (Instance != null) Destroy(this);
        Instance = this;
    }

    private readonly List<BonusEntry> bonusEntries = new List<BonusEntry>();
    public IReadOnlyList<BonusEntry> BonusEntries => bonusEntries;

    public void ClearUnlockEntries() => bonusEntries.Clear();

    public void AddBonusEntry(string name, string reason, string description, int priority = 0, string clarkeMessage = "")
    {
        bonusEntries.Add(new BonusEntry(name, reason, description, priority, clarkeMessage));
        bonusEntries.Sort();
    }

}
