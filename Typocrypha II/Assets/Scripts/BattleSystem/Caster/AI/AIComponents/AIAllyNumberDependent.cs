using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAllyNumberDependent : AIComponent
{
    [SerializeField] private Caster selfCaster;
    [SerializeField] private Spell startingSpell;

    [SerializeField] private SpellList aloneSpellList;
    [SerializeField] private SpellList oneAllySpellList;
    [SerializeField] private SpellList multipleAlliesSpellList;

    [SerializeField] private int aloneLoopIndex;
    [SerializeField] private int oneAllyLoopIndex;
    [SerializeField] private int multipleAlliesLoopIndex;

    SpellList lastSpellList = null;
    int spellIndex = -1;

    // Start is called before the first frame update
    void Awake()
    {
        // Initialize the standard refs (caster and AI)
        InitializeBase();
        if(startingSpell != null && startingSpell.Count > 0)
        {
            ChangeSpell(startingSpell);
        }
        else
        {
            SetNextSpell(null, selfCaster);
        }
    }

    private void OnEnable()
    {
        caster.OnAfterCastResolved -= SetNextSpell;
        caster.OnAfterCastResolved += SetNextSpell;
    }

    private void OnDisable()
    {
        caster.OnAfterCastResolved -= SetNextSpell;
    }

    private void SetNextSpell(Spell spell, Caster self)
    {
        int numAllies = GetNumAllies(self);
        var spellList = GetSpellList(numAllies, out int loopIndex);
        if(spellList != lastSpellList)
        {
            spellIndex = 0;
        }
        else if(++spellIndex >= spellList.Count)
        {
            spellIndex = loopIndex;
        }
        lastSpellList = spellList;
        ChangeSpell(spellList[spellIndex]);
    }

    private int GetNumAllies(Caster self)
    {
        int allyCount = 0;
        foreach(var caster in Battlefield.instance.Casters)
        {
            if(caster != self && caster.CasterState == Caster.State.Hostile && caster.BStatus == Caster.BattleStatus.Normal)
            {
                allyCount++;
            }
        }
        return allyCount;
    }

    private SpellList GetSpellList(int numAllies, out int loopIndex)
    {
        if(numAllies <= 0)
        {
            loopIndex = aloneLoopIndex;
            return aloneSpellList;
        }
        if(numAllies == 1 && oneAllySpellList != null && oneAllySpellList.Count > 0)
        {
            loopIndex = oneAllyLoopIndex;
            return oneAllySpellList;
        }
        loopIndex = multipleAlliesLoopIndex;
        return multipleAlliesSpellList;
    }
}
