using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCaster : Caster
{
    public SpellWord[] spellWords;
    public Battlefield.Position tPos = new Battlefield.Position(0,0);
    public void Cast()
    {
        SpellManager.instance.Cast(spellWords, this, tPos);
    }
}
