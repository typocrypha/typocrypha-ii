using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using TMPro;

public class Thesaurus : MonoBehaviour
{

    [SerializeField] private SpellWordBundle SynonymBundle;
    [SerializeField] private TMPro.TextMeshProUGUI DisplayText;

    const int SPELL_DISPLAY_LINES = 7;
    const int LINE_LENGTH = 24;

    List<SpellWord> GetSynonymsOfBase(SpellWord word)
    {
        return SynonymBundle.words.Select(p => p.Value).Where(s => s.synonymOf == word).ToList();
    }

    string FormatWordList(SpellWord baseWord, List<SpellWord> synonyms, int page)
    {
        int indexLastPage = (synonyms.Count / SPELL_DISPLAY_LINES);
        int indexFirstSpell = SPELL_DISPLAY_LINES * page;
        int itemsToDisplay = page == indexLastPage ? synonyms.Count % SPELL_DISPLAY_LINES : SPELL_DISPLAY_LINES;
        var paginatedWords = synonyms.GetRange(indexFirstSpell, itemsToDisplay);
        int padLength = LINE_LENGTH - baseWord.DisplayName.Length;

        StringBuilder sb = new StringBuilder(LINE_LENGTH);
        sb.Append(baseWord.DisplayName).Append($"page {page+1}/{indexLastPage+1}".PadLeft(padLength));
        foreach (var word in paginatedWords) sb.Append($"\n-{word.DisplayName}");
        return sb.ToString();
    }

    /// <summary>
    /// Print a page of synonyms to thesaurus display.
    /// </summary>
    /// <param name="word">Baseword to search synonyms for.</param>
    /// <param name="page">Page to display. 0 to length - 1. If page out of range, does not attempt to print.</param>
    /// <returns> Number of pages.</returns>
    public int DisplaySynonyms(SpellWord word, int page)
    {
        var synonyms = GetSynonymsOfBase(word);
        int pageCount = (synonyms.Count / SPELL_DISPLAY_LINES) + 1;
        if (page > -1 && page < pageCount) DisplayText.text = FormatWordList(word, synonyms, page);
        return pageCount;
    }
}
