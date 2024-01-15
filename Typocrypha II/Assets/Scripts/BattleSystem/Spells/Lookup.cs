using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lookup : MonoBehaviour
{
    public static bool Ready => instance != null;
    private static Lookup instance = null;
    [SerializeField] private SpellTagBundle spellTagBundle;
    [SerializeField] private CasterTagBundle casterTagBundle;
    [SerializeField] private SpellWordBundle allWordsBundle;
    [SerializeField] private BadgeBundle allBadges;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    /// <summary>
    /// If the string is a valid spell tag, returns it (else returns null)
    /// </summary>
    public static SpellTag GetSpellTag(string name)
    {
        var dict = instance.spellTagBundle.tags;
        if (dict.ContainsKey(name))
            return dict[name];
        return null;
    }
    /// <summary>
    /// If the string is a valid caster tag, returns it (else returns null)
    /// </summary>
    public static CasterTag GetCasterTag(string name)
    {
        var dict = instance.casterTagBundle.tags;
        if (dict.ContainsKey(name))
            return dict[name];
        return null;
    }

    public static SpellWord GetSpellWord(string name)
    {
        return instance.allWordsBundle.words.TryGetValue(name.ToLower(), out var word) ? word : null;
    }

    public static bool TryGetSpellWord(string name, out SpellWord word)
    {
        return instance.allWordsBundle.words.TryGetValue(name.ToLower(), out word);
    }

    public static EquipmentWord GetBadge(string name)
    {
        return instance.allBadges.badges.TryGetValue(name.ToLower(), out var badge) ? badge : null;
    }

    public static bool TryGetBadge(string name, out EquipmentWord badge)
    {
        return instance.allBadges.badges.TryGetValue(name.ToLower(), out badge);
    }
}
