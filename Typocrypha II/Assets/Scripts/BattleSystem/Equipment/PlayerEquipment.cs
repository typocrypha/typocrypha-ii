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

    public IReadOnlyDictionary<EquipmentWord.EquipmentSlot, EquipmentWord> EquippedBadgeWords => equippedBadgeWords;
    private readonly Dictionary<EquipmentWord.EquipmentSlot, EquipmentWord> equippedBadgeWords = new Dictionary<EquipmentWord.EquipmentSlot, EquipmentWord>();

    public IReadOnlyDictionary<string, EquipmentWord> UnlockedBadgeWords => unlockedBadgeWords;
    private readonly Dictionary<string, EquipmentWord> unlockedBadgeWords = new Dictionary<string, EquipmentWord>();

    [SerializeField] List<SpellWord> debugWords = new List<SpellWord>();
    [SerializeField] List<EquipmentWord> debugBadgeWords = new List<EquipmentWord>();

    private void Awake()
    {
        UnlockDebugWords();
        EquipDebugWords();
        UnlockDebugBadges();
        EquipDebugBadges();
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

    [Conditional("DEBUG")]
    private void EquipDebugBadges()
    {
        foreach (var badge in debugBadgeWords)
        {
            EquipBadge(badge);
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

    public void UnlockBadge(EquipmentWord word)
    {
        if (!unlockedBadgeWords.ContainsKey(word.Key))
        {
            unlockedBadgeWords.Add(word.Key, word);
        }
    }
    public void EquipBadge(EquipmentWord word)
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
    public void EquipBadgeLive(EquipmentWord word, Caster player)
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
    public void UnequipBadge(EquipmentWord.EquipmentSlot slot)
    {
        if (!equippedBadgeWords.ContainsKey(slot))
            return;
        equippedBadgeWords.Remove(slot);
    }
    public void UnequipBadgeLive(EquipmentWord.EquipmentSlot slot, Caster player)
    {
        if (!equippedBadgeWords.ContainsKey(slot))
            return;
        equippedBadgeWords[slot].Unequip(player);
        equippedBadgeWords.Remove(slot);
    }
    public void ReapplyEquippedBadgeWords(Caster player)
    {
        foreach(var kvp in equippedBadgeWords)
        {
            kvp.Value.Equip(player);
        }
    }

    public void ClearEquipment()
    {
        equippedSpellWords.Clear();
        EquipDebugWords();
    }
}
