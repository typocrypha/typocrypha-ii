using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CastParser
{
    public static readonly Dictionary<string, SpellWord> words = build();
    private static Dictionary<string, SpellWord> build()
    {
        var bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "spellword"));
        var rootWords = bundle.LoadAllAssets<SpellWord>();
        Dictionary<string, SpellWord> _words = new Dictionary<string, SpellWord>();
        foreach (var word in rootWords)
            _words.Add(word.name.ToLower(), word);
        return _words;
    }

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
    private static readonly WeightedSet<TypoResult> typoActionWeighting = new WeightedSet<TypoResult>()
    {
        { TypoResult.CastFailure, 85 },
        { TypoResult.DropoutWord, 10 },
        { TypoResult.ReplaceWord, 5 }
    };

    private const int maxWords = 5;
    private const int maxRoots = 3;

    //Returns Valid if the string array represents a valid spell that exists in the given spell dictionary
    //Returns other ParseResults to indicate different failure conditions
    public static ParseResults parse(string[] spellwords, out List<SpellWord> s)
    {
        s = new List<SpellWord>();
        int roots = 0;
        foreach (string word in spellwords)
        {
            if (words.ContainsKey(word))
            {
                s.Add(words[word]);
                if (words[word] is RootWord)
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
        if (s.Count > maxWords)
            return ParseResults.TooManyWords;
        #endregion

        #region Root Number Checks
        if (roots <= 0)
            return ParseResults.NoRoot;
        if (roots > maxRoots)
            return ParseResults.TooManyRoots;
        #endregion

        return ParseResults.Valid;
    }
    //Returns a replacement word for a typo keyword (TODO)
    private static SpellWord replaceTypo(string word)
    {
        //TODO: add actual replacement keyword option
        return words["splash"];
    }
}
