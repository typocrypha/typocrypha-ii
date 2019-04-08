using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCaster : Caster
{
    public SpellWord[] spellWords;
    public Battlefield.Position tPos = new Battlefield.Position(0,0);
    public GameObject targetRet;
    public void Cast()
    {
        SpellManager.instance.Cast(spellWords, this, tPos);
    }
    public void CastString(string spellString)
    {
        List<SpellWord> words;// = new List<SpellWord>();
        var results = CastParser.instance.Parse(spellString.Split('-'), out words);
        Debug.Log(results);
        if (results == CastParser.ParseResults.Valid)
            SpellManager.instance.Cast(words.ToArray(), this, tPos);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            tPos.Col = Mathf.Max(0, tPos.Col - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            tPos.Col = Mathf.Min(2, tPos.Col + 1);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            tPos.Row = 0;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            tPos.Row = 1;
        targetRet.transform.position = Battlefield.instance.GetSpace(tPos);
    }
}
