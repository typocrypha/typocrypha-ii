using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIMari : AIComponent
{
    [SerializeField] private Spell cycleStartSpell;
    [SerializeField] private SpellList cycle1Spells;
    [SerializeField] private SpellList cycle2Spells;
    [SerializeField] private SpellList cycle3Spells;
    private int cycleIndex = -1;
    private int spellIndex;
    private State state;

    private enum State
    {
        PreparingStorm,
        Storming,
        EyeOfTheStorm,
    }

    protected override void Awake()
    {
        base.Awake();
        EnterPreparingStormState();
    }

    private void OnEnable()
    {
        RemoveListeners();
        AddListeners();
    }

    private void AddListeners()
    {
        caster.OnAfterCastResolved += AfterCastResolved;
        caster.OnStunned += OnStunned;
        caster.OnUnstunned += OnUnStunned;
    }

    private void OnDisable()
    {
        RemoveListeners();
    }
    
    private void RemoveListeners()
    {
        caster.OnAfterCastResolved -= AfterCastResolved;
        caster.OnStunned -= OnStunned;
        caster.OnUnstunned -= OnUnStunned;
    }

    private void OnStunned()
    {
        state = State.EyeOfTheStorm;
    }

    private void OnUnStunned()
    {
        EnterPreparingStormState(); // May not need anything except buff here
    }

    private void AfterCastResolved(Spell spell, Caster self)
    {
        if(state == State.PreparingStorm)
        {
            EnterStormingState();
        }
        else if(state == State.Storming)
        {
            ChangeToCurrentStormSpell();
        }
        else if(state == State.EyeOfTheStorm)
        {
            EnterPreparingStormState();
        }
    }

    private void EnterStormingState()
    {
        state = State.Storming;
        spellIndex = -1;
        ChangeToCurrentStormSpell();
    }

    private void EnterPreparingStormState()
    {
        ++cycleIndex;
        state = State.PreparingStorm;
        ChangeSpell(cycleStartSpell);
    }

    private void ChangeToCurrentStormSpell()
    {
        SpellList spellList;
        if(cycleIndex == 0)
        {
            spellList = cycle1Spells;
        }
        else if(cycleIndex == 1)
        {
            spellList = cycle2Spells;
        }
        else
        {
            spellList = cycle3Spells;
        }
        if (++spellIndex >= spellList.Count)
            spellIndex = 0;
        ChangeSpell(spellList[spellIndex]);
    }
}
