using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICycleSpells : AIComponent
{
    public int startIndex = 0;
    public int loopIndex = 0;
    public SpellList spells;

    int curr;

    protected override void Awake()
    {
        // Initialize the standard refs (caster and AI)
        base.Awake();
        curr = startIndex;
        ChangeSpell(spells[curr]);
    }

    private void OnEnable()
    {
        caster.OnAfterCastResolved += CycleSpell;
    }

    private void OnDisable()
    {
        caster.OnAfterCastResolved -= CycleSpell;
    }

    public void CycleSpell(Spell spell, Caster self)
    {
        if (++curr >= spells.Count)
            curr = loopIndex;
        ChangeSpell(spells[curr]);
    }
}
