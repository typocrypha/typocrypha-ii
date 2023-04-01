using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public IReadOnlyDictionary<string, SpellWord> EquippedWords => equippedWords;
    private readonly Dictionary<string, SpellWord> equippedWords = new Dictionary<string, SpellWord>();

    public IReadOnlyDictionary<string, SpellWord> UnlockedWords => unlockedWords;
    private readonly Dictionary<string, SpellWord> unlockedWords = new Dictionary<string, SpellWord>();

    [SerializeField] List<SpellWord> debugWords = new List<SpellWord>();

    private void Awake()
    {
        UnlockDebugWords();
        EquipDebugWords();
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

    public void UnlockWord(SpellWord word, bool equip = false)
    {
        if (!unlockedWords.ContainsKey(word.Key))
        {
            unlockedWords.Add(word.Key, word);
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
        if (!equippedWords.ContainsKey(word.Key))
        {
            equippedWords.Add(word.Key, word);
        }
    }

    public void EquipWords(IEnumerable<SpellWord> words)
    {
        foreach (var word in words)
        {
            EquipWord(word);
        }
    }

    public void ClearEquipment()
    {
        equippedWords.Clear();
        EquipDebugWords();
    }
}
