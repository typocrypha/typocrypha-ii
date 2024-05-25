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
    public string DisplayName => CurrentBadge.internalName.ToUpper();
    public string Key => internalName.ToLower();
    [SerializeField] private string internalName;
    public string Description => CurrentBadge.description;
    [TextArea(2, 4)] [SerializeField] private string description;
    public EquipmentSlot Slot => CurrentBadge.slot;
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private int cost;

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
    public BadgeWord NextUpgrade => HasUpgrade ? upgrades[UpgradeLevel] : null;
    private BadgeWord CurrentBadge => IsUpgraded ? upgrades[UpgradeLevel - 1] : this;

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

    public void Equip(Caster player)
    {
        var badge = CurrentBadge;
        EquipEffect(badge.effect1, player);
        EquipEffect(badge.effect2, player);
        EquipEffect(badge.effect3, player);
        EquipEffect(badge.effect4, player);
        EquipEffect(badge.effect5, player);
    }

    private void EquipEffect(BadgeEffect effect, Caster player)
    {
        if (effect == null)
            return;
        effect.Equip(player);
    }

    public void Unequip(Caster player)
    {
        var badge = CurrentBadge;
        UnequipEffect(badge.effect1, player);
        UnequipEffect(badge.effect2, player);
        UnequipEffect(badge.effect3, player);
        UnequipEffect(badge.effect4, player);
        UnequipEffect(badge.effect5, player);
    }

    private void UnequipEffect(BadgeEffect effect, Caster player)
    {
        if (effect == null)
            return;
        effect.Unequip(player);
    }

    public override string ToString()
    {
        char slotType = Slot.ToString()[0];
        return string.Format("{0} - {1} {2}", slotType, DisplayName, cost.ToString("C0"));
    }
}
