using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICycleSpells : AIComponent
{
    public int startIndex = 0;
    public List<Spell> spells;

    int curr;

    // Start is called before the first frame update
    void Awake()
    {
        // Initialize the standard refs (caster and AI)
        InitializeBase();
        curr = startIndex;
        AI.OnAfterCast += CycleSpell;
        caster.Spell = spells[curr];
    }
    public void CycleSpell()
    {
        if (++curr >= spells.Count)
            curr = 0;
        caster.Spell = spells[curr];
    }
}
