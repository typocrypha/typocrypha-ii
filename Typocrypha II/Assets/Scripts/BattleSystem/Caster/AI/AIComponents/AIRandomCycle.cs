﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRandomCycle : AIComponent
{
    [SerializeField] private Spell openingSpell;
    [SerializeField] private SpellList spells1;
    [SerializeField] private SpellList spells2;

    int ind1 = 0;
    int ind2 = 0;
    bool onSpell1 = false;

    // Start is called before the first frame update
    protected override void Awake()
    {
        // Initialize the standard refs (caster and AI)
        base.Awake();
        ind1 = RandomUtils.RandomU.instance.RandomInt(0, spells1.Count);
        if (openingSpell.Count > 0)
        {
            ChangeSpell(openingSpell);
            onSpell1 = true;
        }
        else
        {
            ChangeSpell(spells1[ind1]);
            onSpell1 = spells2.Count <= 0;
        }
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
        if (onSpell1)
        {
            CycleSpellList(spells1, ref ind1);
        }
        else
        {
            CycleSpellList(spells2, ref ind2);
        }
        onSpell1 = !onSpell1 || spells2.Count <= 0;
    }

    private void CycleSpellList(SpellList list, ref int index)
    {
        if (++index >= list.Count)
            index = 0;
        ChangeSpell(list[index]);
    }
}
