using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RandomUtils;

public class SpellParser : MonoBehaviour
{
    public const int maxWords = int.MaxValue;
    public const int maxRoots = int.MaxValue;
    public enum ParseResults
    {
        Valid,
        EmptySpell,
        TooManyWords,
        NoRoot,
        TooManyRoots,
        TypoFailure,
        OnCooldown,
        DuplicateWord,
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
    //public SpellWordBundle roots;
    //public SpellWordBundle modifiers;

    private readonly WeightedSet<TypoResult> typoActionWeighting = new WeightedSet<TypoResult>()
    {
        { TypoResult.CastFailure, 100 },
    };
    /// <summary> Singleton Implementation </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    /// <summary> Returns Valid if the string array represents a valid spell that exists in the given spell dictionary
    /// Returns other ParseResults to indicate different failure conditions 
    /// Returns the parsed spell in out list s.
    /// Also Checks and starts spell cooldowns. </summary>
    public ParseResults Parse(string[] spellwords, IReadOnlyDictionary<string, SpellWord> words, out Spell s, out string problemWord)
    {
        problemWord = null;
        s = new Spell();
        int roots = 0;
        foreach (string word in spellwords)
        {
            if (string.IsNullOrWhiteSpace(word))
                continue;
            if (words.ContainsKey(word)) // Spell is in the words availible for this cast
            {
                if(!TryAddWord(s, words[word], ref roots))
                {
                    problemWord = word;
                    return ParseResults.DuplicateWord;
                }
            }
            else if(freeWordBundle.words.TryGetValue(word, out var freeWord)) // Spell is in the always availible words
            {
                if (!TryAddWord(s, freeWord, ref roots))
                {
                    problemWord = word;
                    return ParseResults.DuplicateWord;
                }
            }
            else if (synonymBundle.words.TryGetValue(word, out var synonym) && (words.ContainsKey(synonym.synonymOf.Key) || freeWordBundle.words.ContainsKey(synonym.synonymOf.Key)))
            {
                if (!TryAddWord(s, synonym, ref roots))
                {
                    problemWord = word;
                    return ParseResults.DuplicateWord;
                }
            }
            else
            {
                #region Process Typo Results
                TypoResult result = RandomU.instance.Choice(typoActionWeighting);
                switch (result)
                {
                    case TypoResult.CastFailure:
                        problemWord = word;
                        return ParseResults.TypoFailure;
                    default:
                        continue;
                }
                #endregion
            }
        }

        // Keyword count checks
        if (s.Count <= 0)
            return ParseResults.EmptySpell;
        if (s.Count > maxWords)
            return ParseResults.TooManyWords;

        // Root count checks
        if (roots <= 0)
            return ParseResults.NoRoot;
        if (roots > maxRoots)
            return ParseResults.TooManyRoots;

        return ParseResults.Valid;
    }

    private bool TryAddWord(Spell s, SpellWord word, ref int roots)
    {
        if (s.Contains(word))
        {
            return false;
        }
        s.Add(word);
        if (word is RootWord)
        {
            ++roots;
        }
        return true;
    }
}
