using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCaster : Caster
{
    public SpellWord[] spellWords;
    public Battlefield.Position tPos = new Battlefield.Position(0,0);
    public void Cast()
    {
        Spell s = new Spell(spellWords);
        s.Cast(this, tPos);
    }
}
