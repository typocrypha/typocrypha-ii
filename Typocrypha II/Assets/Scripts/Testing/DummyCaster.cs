using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCaster : Caster
{
    public SpellWord[] spellWords;
    public Battlefield.Position tPos = new Battlefield.Position(0,0);
    public string spellString;
    public void Cast()
    {
        SpellManager.instance.Cast(spellWords, this, tPos);
    }
    public void CastString()
    {
        List<SpellWord> words;// = new List<SpellWord>();
        var results = CastParser.instance.parse(spellString.Split('-'), out words);
        Debug.Log(results);
        if (results == CastParser.ParseResults.Valid)
            SpellManager.instance.Cast(words.ToArray(), this, tPos);
    }
}
