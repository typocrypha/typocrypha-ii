using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public Dictionary<string, SpellWord> EquippedWordsDict => EquippedWords.Distinct().ToDictionary((s) => s.Key);
    public List<SpellWord> EquippedWords { get; } = new List<SpellWord>();
    [SerializeField] List<SpellWord> debugWords = new List<SpellWord>();

    private void Awake()
    {
        Initialize();
    }

    [Conditional("DEBUG")]
    private void Initialize()
    {
        EquippedWords.AddRange(debugWords);
    }
}
