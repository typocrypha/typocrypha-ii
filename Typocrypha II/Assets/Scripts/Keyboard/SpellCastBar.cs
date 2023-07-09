using System.Collections;
using System.Collections.Generic;
using Typocrypha;
using UnityEngine;

public class SpellCastBar : CastBar
{
    public override void Submit()
    {
        if (Battlefield.instance.Player is Player player)
        {
            player.CastString(Text.TrimEnd(KeywordDelimiters).Split(KeywordDelimiters));
        }
        else
        {
            Debug.LogError("Player is not valid. Cannot cast");
        }
        Clear(false);
    }
}
