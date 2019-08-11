using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICycleSpells : AIComponent
{
    public int startIndex = 0;
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
        caster.AfterCastResolved += (spell, self) => CycleSpell();
    }

    private void OnDisable()
    {
        caster.AfterCastResolved -= (spell, self) => CycleSpell();
    }

    public void CycleSpell()
    {
        if (++curr >= spells.Count)
            curr = 0;
        caster.Spell = spells[curr];
    }
}
