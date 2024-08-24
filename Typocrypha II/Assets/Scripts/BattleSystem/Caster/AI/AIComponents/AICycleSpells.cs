using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICycleSpells : AIComponent
{
    [SerializeField] private int startIndex = 0;
    [SerializeField] private int loopIndex = 0;
    [SerializeField] private SpellList spells;

    protected virtual SpellList Spells => spells;

    private int curr;

    protected override void Awake()
    {
        // Initialize the standard refs (caster and AI)
        base.Awake();
        curr = startIndex;
        ChangeSpell(Spells[curr]);
    }

    private void OnEnable()
    {
        caster.OnAfterCastResolved += CycleSpell;
    }

    private void OnDisable()
    {
        caster.OnAfterCastResolved -= CycleSpell;
    }

    public void CycleSpell(Spell spell, Caster self, bool hitTarget)
    {
        if (++curr >= Spells.Count)
            curr = loopIndex;
        ChangeSpell(Spells[curr]);
    }
}
