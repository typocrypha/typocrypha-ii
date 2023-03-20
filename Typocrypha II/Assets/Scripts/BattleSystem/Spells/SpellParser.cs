using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RandomUtils;

public class SpellParser : MonoBehaviour
{
    public enum ParseResults
    {
        Valid,
        EmptySpell,
        TooManyWords,
        NoRoot,
        TooManyRoots,
        TypoFailure,
        OnCooldown,
    }
    private enum TypoResult
    {
        CastFailure,
        DropoutWord,
        ReplaceWord,
    }
    public static SpellParser instance = null;
    // Words that are availible even if not equipped
    [SerializeField] private SpellWordBundle freeWordBundle;
    [SerializeField] private SpellWordBundle synonymBundle;
    private readonly Dictionary<string, SpellWord> freeWords = new Dictionary<string, SpellWord>();
    //public SpellWordBundle roots;
    //public SpellWordBundle modifiers;

    private readonly WeightedSet<TypoResult> typoActionWeighting = new WeightedSet<TypoResult>()
    {
        { TypoResult.CastFailure, 100 },
    };
    public int MaxWords { get; } = 5;
    public int MaxRoots { get; } = 3;
    /// <summary> Singleton Implementation </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            BuildDictionary();
        }
        else
            Destroy(gameObject);
    }

    /// <summary> Build the free word dictionary from the free word assetbundle </summary>
    private void BuildDictionary()
    {
        freeWords.Clear();
        foreach (var word in freeWordBundle.words)
            freeWords.Add(word.Key.ToLower(), word.Value);
    }

    /// <summary> Returns Valid if the string array represents a valid spell that exists in the given spell dictionary
    /// Returns other ParseResults to indicate different failure conditions 
    /// Returns the parsed spell in out list s.
    /// Also Checks and starts spell cooldowns. </summary>
    public ParseResults Parse(string[] spellwords, IReadOnlyDictionary<string, SpellWord> words, out Spell s)
    {
        s = new Spell();
        int roots = 0;
        foreach (string word in spellwords)
        {
            if (words.ContainsKey(word)) // Spell is in the words availible for this cast
            {
                s.Add(words[word]);
                if (words[word] is RootWord)
                {
                    ++roots;
                }
            }
            else if(freeWords.ContainsKey(word)) // Spell is in the always availible words
            {
                s.Add(freeWords[word]);
                if (freeWords[word] is RootWord)
                {
                    ++roots;
                }
            }
            else if (synonymBundle.words.TryGetValue(word, out var synonym) && (words.ContainsKey(synonym.synonymOf.Key) || freeWords.ContainsKey(synonym.synonymOf.Key)))
            {
                s.Add(synonym);
                if(synonym is RootWord)
                {
                    ++roots;
                }
            }
            else
            {
                #region Process Typo Results
                TypoResult result = RandomU.instance.Choice(typoActionWeighting);
                switch (result)
                {
                    case TypoResult.CastFailure:
                        return ParseResults.TypoFailure;
                    default:
                        continue;
                }
                #endregion
            }
        }

        #region Keyword Number Checks
        if (s.Count <= 0)
            return ParseResults.EmptySpell;
        if (s.Count > MaxWords)
            return ParseResults.TooManyWords;
        #endregion

        #region Root Number Checks
        if (roots <= 0)
            return ParseResults.NoRoot;
        if (roots > MaxRoots)
            return ParseResults.TooManyRoots;
        #endregion

        return ParseResults.Valid;
    }
}
