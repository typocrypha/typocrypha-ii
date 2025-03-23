using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIMari : AIComponent, IBattleWordProvider
{
    [SerializeField] private Spell battleStartSpell;
    [SerializeField] private Spell cycleStartSpell;
    [SerializeField] private SpellList cycle1Spells;
    [SerializeField] private SpellList cycle2Spells;
    [SerializeField] private SpellList cycle3Spells;

    [SerializeField] private List<BattleWord.SequenceData> cycle1Prompts;
    [SerializeField] private List<BattleWord.SequenceData> cycle2Prompts;
    [SerializeField] private List<BattleWord.SequenceData> cycle3Prompts;
    [SerializeField] private GameObject battleWordPrefab;

    private int cycleIndex;
    private int spellIndex;
    private State state = State.BattleStart;

    private enum State
    {
        PreparingStorm,
        Storming,
        EyeOfTheStorm,
        BattleStart,
    }

    protected override void Awake()
    {
        base.Awake();
        state = State.BattleStart;
        ChangeSpell(cycleStartSpell);
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
        Rule.ActiveRule = null;
    }

    private void OnUnStunned()
    {
        EnterPreparingStormState(); // May not need anything except buff here
    }

    private void AfterCastResolved(Spell spell, Caster self, bool hitTarget)
    {
        if(spell.Count > 0 && spell[0].Key == battleStartSpell[0].Key)
        {
            state = State.PreparingStorm;
        }
        if (state == State.BattleStart)
            return;
        if(state == State.PreparingStorm)
        {
            EnterStormingState();
        }
        else if(state == State.Storming)
        {
            ChangeToCurrentStormSpell();
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

    public IReadOnlyList<BattleWord.SequenceData> GetData(string _, out GameObject defaultPrefab)
    {
        defaultPrefab = battleWordPrefab;
        if (cycleIndex <= 0)
        {
            return cycle1Prompts;
        }
        if (cycleIndex == 1)
        {
            return cycle2Prompts;
        }
        return cycle3Prompts;
    }

    [System.Serializable]
    public class PromptData
    {
        public string Prompt;
        public float Time;
    }
}
