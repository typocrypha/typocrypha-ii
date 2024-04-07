using UnityEngine;

public abstract class EquipmentWord : ScriptableObject
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

    public abstract void Equip(Caster player);
    public abstract void Unequip(Caster player);
}
