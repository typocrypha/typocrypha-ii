using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BadgeWord", menuName = "BadgeWord")]
public class BadgeWord : ScriptableObject
{
    public enum EquipmentSlot
    {
        Active,
        Passive,
        Soldier,
    }

    [System.Flags]
    public enum ShopUnlockCodes
    {
        None = 0,
        CannotPurchase = 1,
        A = 2,
        B = 4,
        C = 8,
        All = A | B | C,
    }
    public string DisplayName => CurrentBadge.internalName.ToUpper();
    public string Key => internalName.ToLower();
    [SerializeField] private string internalName;
    public string Description => CurrentBadge.description;
    [TextArea(2, 4)] [SerializeField] private string description;
    public EquipmentSlot Slot => CurrentBadge.slot;
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private int cost;
    public ShopUnlockCodes ShopUnlockRequirements => shopUnlockRequirements;
    [SerializeField] private ShopUnlockCodes shopUnlockRequirements;
    public int Cost => CurrentBadge.cost;
    public int UpgradeCost => HasUpgrade ? NextUpgrade.cost : 0;

    public int NextCost => IsUnlocked ? UpgradeCost : Cost;

    public IEnumerable<BadgeEffect> Effects
    {
        get
        {
            var badge = CurrentBadge;
            if(effect1 != null)
            {
                yield return badge.effect1;
            }
            if (effect2 != null)
            {
                yield return badge.effect2;
            }
            if (effect3 != null)
            {
                yield return badge.effect3;
            }
            if (effect4 != null)
            {
                yield return badge.effect4;
            }
            if (effect5 != null)
            {
                yield return badge.effect5;
            }
        }
    }

    [SerializeField] [SubSO("Effect1")] private BadgeEffect effect1;
    [SerializeField] [SubSO("Effect2")] private BadgeEffect effect2;
    [SerializeField] [SubSO("Effect3")] private BadgeEffect effect3;
    [SerializeField] [SubSO("Effect4")] private BadgeEffect effect4;
    [SerializeField] [SubSO("Effect5")] private BadgeEffect effect5;

    [SerializeField] private BadgeWord[] upgrades;

    private int UpgradeLevel
    {
        get => PlayerDataManager.Equipment.GetUpgradeLevel(this);
        set => PlayerDataManager.Equipment.SetUpgradeLevel(this, value);
    }
    private bool IsUpgraded => UpgradeLevel > 0;

    public bool HasUpgrade => upgrades.Length > UpgradeLevel;
    public bool IsUnlocked => PlayerDataManager.Equipment.IsBadgeUnlocked(this);
    public BadgeWord NextUpgrade => HasUpgrade ? upgrades[UpgradeLevel] : null;
    private BadgeWord CurrentBadge => IsUpgraded ? upgrades[UpgradeLevel - 1] : this;

    public bool CanPurchase
    {
        get
        {
            var data = PlayerDataManager.instance;
            return !PlayerDataManager.Equipment.IsBadgeUnlocked(this) && data.ShopData.IsUnlockedInShop(this);
        }
    }

    public void Upgrade()
    {
        // Stub Implementation
        if (!HasUpgrade)
            return;
        var shouldRefresh = PlayerDataManager.Equipment.IsBadgeEquipped(this) && Battlefield.instance != null && Battlefield.instance.Player != null;
        if (shouldRefresh)
        {
            Unequip(Battlefield.instance.Player);
        }
        UpgradeLevel++;
        if (shouldRefresh)
        {
            Equip(Battlefield.instance.Player);
        }
    }

    public void Equip(Player player)
    {
        foreach (var effect in Effects)
        {
            effect.Equip(player);
        }
    }

    public void Unequip(Player player)
    {
        foreach(var effect in Effects)
        {
            effect.Unequip(player);
        }
    }

    public T GetEffect<T>() where T : BadgeEffect
    {
        foreach(var effect in Effects)
        {
            if(effect is T tEffect)
            {
                return tEffect;
            }
        }
        return null;
    }

    public override string ToString()
    {
        char slotType = Slot.ToString()[0];
        return string.Format("{0} - {1}", slotType, DisplayName);
    }
}
