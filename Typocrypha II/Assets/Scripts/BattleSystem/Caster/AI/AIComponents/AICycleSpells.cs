using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICycleSpells : AIComponent
{
    public int startIndex = 0;
    public int loopIndex = 0;
    public SpellList spells;

    int curr;

    // Start is called before the first frame update
    void Awake()
    {
        // Initialize the standard refs (caster and AI)
        InitializeBase();
        curr = startIndex;
        caster.Spell = spells[curr];
    }

    private void OnEnable()
    {
        caster.OnAfterCastResolved += (spell, self) => CycleSpell();
        caster.OnCounter += (self) => CycleSpell();
    }

    private void OnDisable()
    {
        caster.OnAfterCastResolved -= (spell, self) => CycleSpell();
        caster.OnCounter -= (self) => CycleSpell();
    }

    public void CycleSpell()
    {
        if (++curr >= spells.Count)
            curr = loopIndex;
        caster.Spell = spells[curr];
    }
}
