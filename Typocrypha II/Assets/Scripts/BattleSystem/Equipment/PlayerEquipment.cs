using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public static PlayerEquipment instance = null;

    public Dictionary<string, SpellWord> EquippedWordsDict => EquippedWords.Distinct().ToDictionary((s) => s.internalName.ToLower());
    public List<SpellWord> EquippedWords { get; } = new List<SpellWord>();
    [SerializeField] List<SpellWord> debugWords = new List<SpellWord>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        EquippedWords.AddRange(debugWords);
    }
}
