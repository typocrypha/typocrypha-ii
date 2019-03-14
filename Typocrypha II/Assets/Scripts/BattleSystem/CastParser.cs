﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CastParser : MonoBehaviour
{
    public enum ParseResults
    {
        Valid,
        EmptySpell,
        TooManyWords,
        NoRoot,
        TooManyRoots,
        TypoFailure,
    }
    private enum TypoResult
    {
        CastFailure,
        DropoutWord,
        ReplaceWord,
    }
    public static CastParser instance = null;

    private readonly WeightedSet<TypoResult> typoActionWeighting = new WeightedSet<TypoResult>()
    {
        { TypoResult.CastFailure, 85 },
        { TypoResult.DropoutWord, 10 },
        { TypoResult.ReplaceWord, 5 }
    };
    public int MaxWords { get; } = 5;
    public int MaxRoots { get; } = 3;
    public Dictionary<string, SpellWord> Words { get; private set; }
    /// <summary> Singleton Implementation </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            buildDictionary();
        }
        else
            Destroy(gameObject);
    }
    /// <summary> Returns Valid if the string array represents a valid spell that exists in the given spell dictionary
    /// Returns other ParseResults to indicate different failure conditions 
    /// Returns the parsed spell in out list s </summary>
    public ParseResults parse(string[] spellwords, out List<SpellWord> s)
    {
        s = new List<SpellWord>();
        int roots = 0;
        foreach (string word in spellwords)
        {
            if (Words.ContainsKey(word))
            {
                s.Add(Words[word]);
                if (Words[word] is RootWord)
                    ++roots;
            }
            else
            {
                #region Process Typo Results
                TypoResult result = RandomC.RandomChoice(typoActionWeighting, true);
                switch (result)
                {
                    case TypoResult.CastFailure:
                        return ParseResults.TypoFailure;
                    case TypoResult.ReplaceWord:
                        SpellWord replacement = replaceTypo(word);
                        s.Add(replacement);
                        if (replacement is RootWord)
                            ++roots;
                        break;
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
    /// <summary> Returns a replacement word for a misspelled keyword (WIP) </summary>
    private SpellWord replaceTypo(string word)
    {
        //TODO: add actual replacement keyword option
        return Words["splash"];
    }
    /// <summary> Build the wor dictionary from the "spellword" assetbundle </summary>
    private void buildDictionary()
    {
        var bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "spellword"));
        var rootWords = bundle.LoadAllAssets<SpellWord>();
        Words = new Dictionary<string, SpellWord>();
        foreach (var word in rootWords)
            Words.Add(word.name.ToLower(), word);
    }
}
