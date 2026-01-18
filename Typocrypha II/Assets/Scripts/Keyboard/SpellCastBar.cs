using System.Collections;
using System.Collections.Generic;
using Typocrypha;
using UnityEngine;

public class SpellCastBar : CastBar
{
    public override void Submit()
    {
        if (!(Battlefield.instance.Player is Player player))
        {
            Debug.LogError("Player is not valid. Cannot cast");
            return;
        }
        string text = Text;
        if(!string.IsNullOrEmpty(Prompt) && text != Prompt.ToLower())
        {
            SpellFxManager.instance.CastFailFx($"Cast {Prompt.ToUpper()}!");
            Clear();
            return;
        }
        var results = player.CastString(text.TrimEnd(KeywordDelimiters).Split(KeywordDelimiters));
        Clear(results != SpellParser.ParseResults.Valid);
    }
}
