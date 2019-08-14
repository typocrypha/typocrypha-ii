using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caster : FieldObject
{
    public enum Class
    {
        None = -1,
        Other,
        Player,
        PartyMember,
    }
    public enum State
    {
        None = -1,
        Player,
        Hostile,
        Neutral,
        Ally,
    }
    public enum BattleStatus
    {
        Alive,
        Dead,
        Fled,
        Banished,
        Hiding,
    }

    #region Delegate Declarations
    public delegate void ApplyToEffectFn(RootWordEffect effect, Caster caster, Caster target);
    public delegate void HitFn(RootWordEffect effect, Caster caster, Caster target, CastResults data);
    public delegate void AfterCastFn(Spell s, Caster caster); // Add targets and results?
    #endregion

    public ApplyToEffectFn OnBeforeCastResolved { get; set; }
    public AfterCastFn OnAfterCastResolved { get; set; }
    public HitFn OnAfterHitResolved { get; set; }

    #region State, Status, and Class
    [SerializeField] private State _type;
    public State CasterState { get => _type; set => _type = value; }
    [SerializeField] private Class _casterClass;
    public Class CasterClass { get => _casterClass; set => _casterClass = value; }
    public BattleStatus BStatus { get; }
    #endregion

    #region Health properties and UI functionality
    int health;
    public int Health
    {
        get => health;
        set
        {
            health = value;
            ui?.onHealthChanged.Invoke((float)health/Stats.MaxHP);
        }
    }
    public int Armor { get; set; }
    public int SP { get; set; }
    int stagger;
    public int Stagger
    {
        get => stagger;
        set
        {
            stagger = value;
            ui?.onStaggerChanged.Invoke(stagger.ToString());
            if (stagger <= 0)
            {
                Stunned = true;
            }

        }
    }
    private bool stunned = false;
    public bool Stunned
    {
        get => stunned;
        set
        {
            stunned = value;
            // If stunned display stun UI
            if (stunned)
            {
                ui?.onStun.Invoke();
            }
            else
            {
                // If unstunned reset stagger to full
                Stagger = Stats.MaxStagger;
            }
        }
    }
    Spell spell;
    public Spell Spell
    {
        get => spell;
        set
        {
            spell = value;
            ui?.onSpellChanged.Invoke(spell.ToDisplayString());
        }
    }
    public float ChargeTime { get; set; } // Total time it takes to charge
    float charge; // Charge amount (seconds) for enemies
    public float Charge
    {
        get => charge;
        set
        {
            charge = value;
            ui?.onChargeChanged.Invoke(charge/ChargeTime); 
        }
    }
    #endregion

    #region Caster Tags and Caster Stats
    [SerializeField] private CasterTagDictionary tags;
    public bool HasTag(CasterTag tag)
    {
        return tags.ContainsTag(tag);
    }
    public void RemoveTag(CasterTag tag)
    {
        tags.Remove(tag);
    }
    public void AddTag(CasterTag tag)
    {
        tags.Add(tag, this);
    }
    public CasterTagDictionary.ReactionMultiSet GetReactions(SpellTag tag)
    {
        return tags.GetReactions(tag);
    }
    public CasterStats Stats { get => tags.statMod; }

    #if UNITY_EDITOR
    public CasterTagDictionary Tags => tags;
    #endif
    #endregion

    public Battlefield.Position TargetPos { get; set; } = new Battlefield.Position(0, 0);
    public CasterUI ui = null;

    protected void Awake()
    {
        tags.RecalculateAggregate();
        tags.SpawnAllStatusEffects(this);
        Health = Stats.MaxHP;
        Armor = Stats.MaxArmor;
        SP = Stats.MaxSP;
        Stagger = Stats.MaxStagger;
        if (ui == null) ui = GetComponentInChildren<CasterUI>();
    }
}
