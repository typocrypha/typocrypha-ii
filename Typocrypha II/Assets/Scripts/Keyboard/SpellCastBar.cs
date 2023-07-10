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
        var results = player.CastString(Text.TrimEnd(KeywordDelimiters).Split(KeywordDelimiters));
        Clear(results != SpellParser.ParseResults.Valid);
    }
}
