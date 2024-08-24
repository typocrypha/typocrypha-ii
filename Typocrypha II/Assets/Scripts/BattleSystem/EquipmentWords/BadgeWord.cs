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

    [SerializeField] [SubSO("Effect1")] private BadgeEffect effect1;
    [SerializeField] [SubSO("Effect2")] private BadgeEffect effect2;
    [SerializeField] [SubSO("Effect3")] private BadgeEffect effect3;
    [SerializeField] [SubSO("Effect4")] private BadgeEffect effect4;
    [SerializeField] [SubSO("Effect5")] private BadgeEffect effect5;

    [SerializeField] private BadgeWord[] upgrades;

    private int UpgradeLevel
    {
        get => PlayerDataManager.instance.equipment.GetUpgradeLevel(this);
        set => PlayerDataManager.instance.equipment.SetUpgradeLevel(this, value);
    }
    private bool IsUpgraded => UpgradeLevel > 0;

    public bool HasUpgrade => upgrades.Length > UpgradeLevel;
    public bool IsUnlocked => PlayerDataManager.instance.equipment.IsBadgeUnlocked(this);
    public BadgeWord NextUpgrade => HasUpgrade ? upgrades[UpgradeLevel] : null;
    private BadgeWord CurrentBadge => IsUpgraded ? upgrades[UpgradeLevel - 1] : this;

    public bool CanPurchase
    {
        get
        {
            var data = PlayerDataManager.instance;
            return !data.equipment.IsBadgeUnlocked(this) && data.ShopData.IsUnlockedInShop(this);
        }
    }

    public void Upgrade()
    {
        // Stub Implementation
        if (!HasUpgrade)
            return;
        var equipment = PlayerDataManager.instance.equipment;
        var shouldRefresh = equipment.IsBadgeEquipped(this) && Battlefield.instance != null && Battlefield.instance.Player != null;
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
        var badge = CurrentBadge;
        EquipEffect(badge.effect1, player);
        EquipEffect(badge.effect2, player);
        EquipEffect(badge.effect3, player);
        EquipEffect(badge.effect4, player);
        EquipEffect(badge.effect5, player);
    }

    private void EquipEffect(BadgeEffect effect, Player player)
    {
        if (effect == null)
            return;
        effect.Equip(player);
    }

    public void Unequip(Player player)
    {
        var badge = CurrentBadge;
        UnequipEffect(badge.effect1, player);
        UnequipEffect(badge.effect2, player);
        UnequipEffect(badge.effect3, player);
        UnequipEffect(badge.effect4, player);
        UnequipEffect(badge.effect5, player);
    }

    private void UnequipEffect(BadgeEffect effect, Player player)
    {
        if (effect == null)
            return;
        effect.Unequip(player);
    }

    public T GetEffect<T>() where T : BadgeEffect
    {
        var badge = CurrentBadge;
        if(badge.effect1 is T tEffect1)
        {
            return tEffect1;
        }
        if (badge.effect2 is T tEffect2)
        {
            return tEffect2;
        }
        if (badge.effect3 is T tEffect3)
        {
            return tEffect3;
        }
        if (badge.effect4 is T tEffect4)
        {
            return tEffect4;
        }
        if (badge.effect5 is T tEffect5)
        {
            return tEffect5;
        }
        return null;
    }

    public override string ToString()
    {
        char slotType = Slot.ToString()[0];
        return string.Format("{0} - {1}", slotType, DisplayName);
    }
}
