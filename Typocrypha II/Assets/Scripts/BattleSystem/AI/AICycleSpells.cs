using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICycleSpells : CasterAI
{
    public int startIndex = 0;
    public List<Spell> spells;

    int curr;

    // Start is called before the first frame update
    void Awake()
    {
        // Init targeting position
        GetComponent<Caster>().TargetPos = new Battlefield.Position(1, 1);
        curr = startIndex;
        OnAfterCast += CycleSpell;
        CurrSpell = spells[curr];
    }
    public void CycleSpell()
    {
        if (++curr >= spells.Count)
            curr = 0;
        CurrSpell = spells[curr];
    }
}
