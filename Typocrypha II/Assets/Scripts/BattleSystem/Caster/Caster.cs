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
        Normal,
        SpiritMode,
        Dead,
        Fled,
    }

    #region Delegate Declarations
    public delegate void ApplyToEffectFn(RootWordEffect effect, Caster caster, Caster target);
    public delegate void HitFn(RootWordEffect effect, Caster caster, Caster target, CastResults data);
    public delegate CasterTagDictionary.ReactionMultiSet GetReactionsFn(SpellTag tag);
    public delegate void AfterCastFn(Spell s, Caster caster); // Add targets and results?
    #endregion

    public ApplyToEffectFn OnBeforeCastResolved { get; set; }
    public AfterCastFn OnAfterCastResolved { get; set; }
    public GetReactionsFn ExtraReactions { get; set; }
    public HitFn OnBeforeEffectApplied { get; set; }
    public HitFn OnAfterHitResolved { get; set; }
    public System.Action OnSpiritMode { get; set; }

    #region State, Status, and Class
    [SerializeField] private State _type;
    public State CasterState { get => _type; set => _type = value; }
    [SerializeField] private Class _casterClass;
    public Class CasterClass { get => _casterClass; set => _casterClass = value; }
    public BattleStatus BStatus
    {
        get => status;
        set
        {
            switch (value)
            {
                case BattleStatus.Normal:
                    break;
                case BattleStatus.SpiritMode:
                    ui?.onSpiritForm.Invoke();
                    ui?.onSpChanged.Invoke((float)sp / Stats.MaxSP);
                    if(tags.ContainsTag("Demon"))
                        ui?.onNameChanged.Invoke(DisplayName + "(WISP)");
                    OnSpiritMode?.Invoke();
                    break;
                case BattleStatus.Dead:
                    ui?.gameObject.SetActive(false);
                    break;
                case BattleStatus.Fled:
                    ui?.gameObject.SetActive(false);
                    GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);
                    break;
            }
            status = value;
        }
    }
    private BattleStatus status = BattleStatus.Normal;
    #endregion

    #region Health properties and UI functionality    
    public int Health
    {
        get => health;
        protected set
        {
            health = value;
            if (value <= 0 && BStatus == BattleStatus.Normal)
            {
                BStatus = BattleStatus.SpiritMode;
            }
            else
            {
                ui?.onHealthChanged.Invoke((float)health / Stats.MaxHP);
            }         
        }
    }
    int health;
    public int SP
    {
        get => sp;
        protected set
        {
            sp = value;
            if (value <= 0 && BStatus == BattleStatus.SpiritMode)
            {
                BStatus = BattleStatus.Dead;
            }
            ui?.onSpChanged.Invoke((float)sp / Stats.MaxSP);
        }
    }
    int sp;
    public int Stagger
    {
        get => stagger;
        set
        {
            if (Stunned)
                return;
            stagger = Mathf.Max(0, value);
            ui?.onStaggerChanged.Invoke(stagger.ToString());
            if (stagger <= 0)
            {
                Stunned = true;
            }

        }
    }
    int stagger; 
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
    private bool stunned = false;   
    public Spell Spell
    {
        get => spell;
        set
        {
            spell = value;
            if (value == null)
                return;
            // Set spell word (DEBUG)
            ui?.onSpellChanged.Invoke(spell.ToDisplayString());
            // Set spell icon (gets first rootword)
            foreach(var spellword in spell)
                if (spellword is RootWord)
                    ui?.onSpellIconChanged.Invoke((spellword as RootWord).icon);
            
        }
    }
    Spell spell;
    public float ChargeTime { get; set; } // Total time it takes to charge 
    public float Charge
    {
        get => charge;
        set
        {
            charge = value;
            ui?.onChargeChanged.Invoke(charge/ChargeTime); 
        }
    }
    float charge; // Charge amount (seconds) for enemies
    #endregion

    public void Damage(int amount)
    {
        if (BStatus == BattleStatus.SpiritMode)
            SP -= amount;
        else
            Health -= amount;
    }

    public void Heal(int amount)
    {
        if (BStatus == BattleStatus.SpiritMode)
            SP += amount;
        else
            Health += amount;
    }

    private StatusEffectDict statusEffects = new StatusEffectDict();

    #region Caster Tags and Caster Stats
    [SerializeField] private CasterTagDictionary tags;
    public bool HasTag(CasterTag tag)
    {
        return tags.ContainsTag(tag);
    }
    public bool HasTag(string tagName)
    {
        return tags.ContainsTag(tagName);
    }
    public void RemoveTag(CasterTag tag)
    {
        tags.Remove(tag);
        if(statusEffects.ContainsKey(tag))
        {
            var effect = statusEffects[tag];
            Destroy(effect);
            statusEffects.Remove(tag);
        }
    }
    public void AddTag(CasterTag tag)
    {
        tags.Add(tag);
    }
    public void AddTag(string tagName)
    {
        tags.Add(tagName);
    }
    public void AddTagWithStatusEffect(StatusEffect effect, CasterTag tag)
    {
        if (statusEffects.ContainsKey(tag))
            return;
        statusEffects.Add(tag, effect);
        tags.Add(tag);
    }
    public StatusEffect GetStatusEffect(CasterTag tag)
    {
        if (statusEffects.ContainsKey(tag))
            return statusEffects[tag];
        return null;
    }
    public CasterTagDictionary.ReactionMultiSet GetReactions(SpellTag tag)
    {
        return tags.GetReactions(tag);
    }
    public CasterStats Stats { get => tags.statMod; }

    /// <summary>
    /// The dictionary containing all of this caster's CasterTags.
    /// </summary>
    public CasterTagDictionary TagDict => tags;
    #endregion

    public Battlefield.Position TargetPos { get; set; } = new Battlefield.Position(0, 0);
    public CasterUI ui = null;

    protected void Awake()
    {
        if (ui == null) ui = GetComponentInChildren<CasterUI>();
        tags.RecalculateAggregate();
        Health = Stats.MaxHP;
        SP = Stats.MaxSP;
        Stagger = Stats.MaxStagger;
        ui?.onNameChanged.Invoke(DisplayName);
    }

    [System.Serializable] private class StatusEffectDict : SerializableDictionary<CasterTag, StatusEffect> { }

    public override ScouterInfo GetScouterInfo()
    {
        switch (CasterState)
        {
            case State.Player:
                return new ScouterInfo_Player(this);
            case State.Ally:
                return new ScouterInfo_Ally(this);
            case State.Hostile:
                return new ScouterInfo_Enemy(this);
            default:
                return null;
        }
    }
}
