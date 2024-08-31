using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public IReadOnlyDictionary<string, SpellWord> EquippedSpellWords => equippedSpellWords;
    private readonly Dictionary<string, SpellWord> equippedSpellWords = new Dictionary<string, SpellWord>();

    public IReadOnlyDictionary<string, SpellWord> UnlockedSpellWords => unlockedSpellWords;
    private readonly Dictionary<string, SpellWord> unlockedSpellWords = new Dictionary<string, SpellWord>();

    public IReadOnlyDictionary<BadgeWord.EquipmentSlot, BadgeWord> EquippedBadgeWords => equippedBadgeWords;
    private readonly Dictionary<BadgeWord.EquipmentSlot, BadgeWord> equippedBadgeWords = new Dictionary<BadgeWord.EquipmentSlot, BadgeWord>();

    public IReadOnlyDictionary<string, BadgeWord> UnlockedBadgeWords => unlockedBadgeWords;
    private readonly Dictionary<string, BadgeWord> unlockedBadgeWords = new Dictionary<string, BadgeWord>();
    private readonly Dictionary<string, int> badgeUpgradeLevels = new Dictionary<string, int>();

    [SerializeField] List<SpellWord> debugWords = new List<SpellWord>();
    [SerializeField] List<BadgeWord> debugBadgeWords = new List<BadgeWord>();


    private void Awake()
    {
        UnlockDebugWords();
        EquipDebugWords();
        UnlockDebugBadges();
        //EquipDebugBadges();
    }

    [Conditional("DEBUG")]
    private void UnlockDebugWords()
    {
        UnlockWords(debugWords);
    }

    [Conditional("DEBUG")]
    private void EquipDebugWords()
    {
        EquipWords(debugWords);
    }

    [Conditional("DEBUG")]
    private void UnlockDebugBadges()
    {
        foreach(var badge in debugBadgeWords)
        {
            UnlockBadge(badge);
        }
    }

    public void UnlockWord(SpellWord word, bool equip = false)
    {
        if (!unlockedSpellWords.ContainsKey(word.Key))
        {
            unlockedSpellWords.Add(word.Key, word);
        }
        if (equip)
        {
            EquipWord(word);
        }
    }
    public void UnlockWords(IEnumerable<SpellWord> words, bool equip = false)
    {
        foreach (var word in words)
        {
            UnlockWord(word, equip);
        }
    }

    public void EquipWord(SpellWord word)
    {
        if (!equippedSpellWords.ContainsKey(word.Key))
        {
            equippedSpellWords.Add(word.Key, word);
        }
    }
    public void EquipWords(IEnumerable<SpellWord> words)
    {
        foreach (var word in words)
        {
            EquipWord(word);
        }
    }

    public void UnequipWord(SpellWord word)
    {
        if (!equippedSpellWords.ContainsKey(word.Key))
        {
            return;
        }
        equippedSpellWords.Remove(word.Key);
    }

    public bool IsBadgeUnlocked(BadgeWord word)
    {
        return unlockedBadgeWords.ContainsKey(word.Key);
    }

    public int GetUpgradeLevel(BadgeWord word)
    {
        return badgeUpgradeLevels.TryGetValue(word.Key, out int value) ? value : 0; 
    }

    public void SetUpgradeLevel(BadgeWord word, int value)
    {
        if (badgeUpgradeLevels.ContainsKey(word.Key))
        {
            badgeUpgradeLevels[word.Key] = value;
        }
        else
        {
            badgeUpgradeLevels.Add(word.Key, value);
        }
    }
    
    public void UnlockBadge(BadgeWord word)
    {
        if (!unlockedBadgeWords.ContainsKey(word.Key))
        {
            unlockedBadgeWords.Add(word.Key, word);
        }
    }
    public void EquipBadge(BadgeWord word)
    {
        if (equippedBadgeWords.ContainsKey(word.Slot))
        {
            equippedBadgeWords[word.Slot] = word;
        }
        else
        {
            equippedBadgeWords.Add(word.Slot, word);
        }
    }
    public void EquipBadgeLive(BadgeWord word, Player player)
    {
        if (equippedBadgeWords.ContainsKey(word.Slot))
        {
            equippedBadgeWords[word.Slot].Unequip(player);
            equippedBadgeWords[word.Slot] = word;
        }
        else
        {
            equippedBadgeWords.Add(word.Slot, word);
        }
        word.Equip(player);
    }
    public void UnequipBadge(BadgeWord.EquipmentSlot slot)
    {
        if (!equippedBadgeWords.ContainsKey(slot))
            return;
        equippedBadgeWords.Remove(slot);
    }
    public void UnequipBadgeLive(BadgeWord.EquipmentSlot slot, Player player)
    {
        if (!equippedBadgeWords.ContainsKey(slot))
            return;
        equippedBadgeWords[slot].Unequip(player);
        equippedBadgeWords.Remove(slot);
    }
    public void ReapplyEquippedBadgeWords(Player player)
    {
        foreach(var kvp in equippedBadgeWords)
        {
            kvp.Value.Equip(player);
        }
    }

    public bool IsBadgeEquipped(BadgeWord badge)
    {
        return EquippedBadgeWords.TryGetValue(badge.Slot, out var equippedBadge) && badge == equippedBadge;
    }

    public bool IsBadgeEquipped(string name, out BadgeWord badge)
    {
        return Lookup.TryGetBadge(name, out badge) && IsBadgeEquipped(badge);
    }

    public T GetEquippedBadgeEffect<T>() where T : BadgeEffect
    {
        foreach(var kvp in equippedBadgeWords)
        {
            var effect = kvp.Value.GetEffect<T>();
            if (effect != null)
                return effect;
        }
        return null;
    }

    public bool TryGetEquippedBadgeEffect<T>(out T effect) where T : BadgeEffect
    {
        effect = GetEquippedBadgeEffect<T>();
        return effect != null;
    }

    public void ClearEquipment()
    {
        equippedSpellWords.Clear();
        EquipDebugWords();
    }

    public void ClearUnlockedSpells()
    {
        unlockedSpellWords.Clear();
        UnlockDebugWords();
    }

    public void ClearUnlockedBadges()
    {
        unlockedBadgeWords.Clear();
        UnlockDebugBadges();
    }

    public void ClearEquippedBadges()
    {
        equippedBadgeWords.Clear();
    }
}
