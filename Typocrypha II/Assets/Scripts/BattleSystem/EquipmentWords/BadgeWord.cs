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
    public string DisplayName => internalName.ToUpper();
    public string Key => internalName.ToLower();
    [SerializeField] private string internalName;
    public string Description => description;
    [TextArea(2, 4)] [SerializeField] private string description;
    public EquipmentSlot Slot => slot;
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private int cost;

    [SerializeField] [SubSO("Effect1")] private BadgeEffect effect1;
    [SerializeField] [SubSO("Effect2")] private BadgeEffect effect2;
    [SerializeField] [SubSO("Effect3")] private BadgeEffect effect3;
    [SerializeField] [SubSO("Effect4")] private BadgeEffect effect4;
    [SerializeField] [SubSO("Effect5")] private BadgeEffect effect5;

    [SerializeField] private BadgeWord[] upgrades;

    public void Equip(Caster player)
    {
        EquipEffect(effect1, player);
        EquipEffect(effect2, player);
        EquipEffect(effect3, player);
        EquipEffect(effect4, player);
        EquipEffect(effect5, player);
    }

    private void EquipEffect(BadgeEffect effect, Caster player)
    {
        if (effect == null)
            return;
        effect.Equip(player);
    }

    public void Unequip(Caster player)
    {
        UnequipEffect(effect1, player);
        UnequipEffect(effect2, player);
        UnequipEffect(effect3, player);
        UnequipEffect(effect4, player);
        UnequipEffect(effect5, player);
    }

    private void UnequipEffect(BadgeEffect effect, Caster player)
    {
        if (effect == null)
            return;
        effect.Unequip(player);
    }
}
