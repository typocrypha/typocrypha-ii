using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caster : MonoBehaviour
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
    [System.Flags]
    public enum ActiveAbilities
    {
        None = 0,
        Critical = 1,
        CriticalBlock = 2,
        Combo = 8,
    }

    #region Delegate Declarations
    public delegate void ApplyToEffectFn(RootWordEffect effect, Caster caster, Caster target);
    public delegate void HitFn(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults data);
    public delegate CasterTagDictionary.ReactionMultiSet GetReactionsFn(SpellTag tag);
    public delegate void AfterCastFn(Spell s, Caster caster, bool hitTarget); // Add targets and results?
    public delegate void OnCounterOtherFn(Caster caster, Caster countered, bool fullCounter);
    #endregion

    /// <summary>
    /// Callbacks applied before a spell effect resolves (before they resolve)
    /// </summary>
    public ApplyToEffectFn OnBeforeSpellEffectCast { get; set; }

    public HitFn OnAfterSpellEffectCast { get; set; }
    /// <summary>
    /// Callbacks applied after a spell effect resolves
    /// </summary>
    public AfterCastFn OnAfterSpellEffectResolved { get; set; }
    /// <summary>
    /// Callbacks applied after a cast resolves
    /// </summary>
    public AfterCastFn OnAfterCastResolved { get; set; }
    public System.Action<Caster, bool> OnCountered { get; set; }
    public OnCounterOtherFn OnCounterOther { get; set; }
    /// <summary>
    /// Callbacks the calculate extra tag reactions
    /// </summary>
    public GetReactionsFn ExtraReactions { get; set; }
    /// <summary>
    /// Callbacks applied by an effect target before being hit
    /// </summary>
    public HitFn OnBeforeHitResolved { get; set; }
    /// <summary>
    /// Callbacks applied by an effect target after being hit
    /// </summary>
    public HitFn OnAfterHitResolved { get; set; }
    public System.Action OnSpiritMode { get; set; }
    public System.Action<Battlefield.Position> OnNoTargetHit { get; set; }
    public System.Action OnStunned { get; set; }
    public System.Action OnUnstunned { get; set; }
    public System.Action<Caster, Spell> OnSpellChanged { get; set; }

    private ActiveAbilities CurrentActiveAbiltiies { get; set; }
    public void AddActiveAbilities(ActiveAbilities abilities)
    {
        CurrentActiveAbiltiies |= abilities;
    }
    public void RemoveActiveAbilities(ActiveAbilities abilities)
    {
        CurrentActiveAbiltiies &= ~abilities;
    }
    public bool HasActiveAbilities(ActiveAbilities abilities)
    {
        return (CurrentActiveAbiltiies & abilities) == abilities;
    }

    #region Field Object Properties

    [SerializeField] private string _displayName;
    public string DisplayName { get => _displayName; set => _displayName = value; }
    public Battlefield.Position FieldPos { get; set; } = new Battlefield.Position(0, 0);
    [SerializeField] private bool _moveable = true;
    public bool IsMoveable { get => _moveable; set => _moveable = value; }

    #endregion

    #region State, Status, and Class

    public bool IsPlayer => CasterState == State.Player;
    public bool IsDemon => HasTag(Lookup.GetCasterTag("Demon"));
    public State CasterState { get => _type; set => _type = value; }
    [SerializeField] private State _type;
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
                    //if(tags.ContainsTag("Demon"))
                    //    ui?.onNameChanged.Invoke(DisplayName + "(WISP)");
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

    public bool IsDeadOrFled => status == BattleStatus.Dead || status == BattleStatus.Fled;

    #endregion

    #region Health properties and UI functionality    
    public int Health
    {
        get => health;
        protected set
        {
            health = Mathf.Clamp(value, 0, Stats.MaxHP);
            if (value <= 0 && BStatus == BattleStatus.Normal)
            {
                if(SP > 0)
                {
                    EnterSpiritMode();
                }
                else
                {
                    Kill();
                }
            }
            ui?.onHealthChanged.Invoke((float)health / Stats.MaxHP);
            ui?.onHealthChangedNumber.Invoke(health + "/" + Stats.MaxHP);
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
                Kill();
            }
            ui?.onSpChanged.Invoke((float)sp / Stats.MaxSP);
            ui?.onHealthChangedNumber.Invoke(sp + "/" + Stats.MaxSP);
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
            if (value == stunned)
                return;
            stunned = value;
            // If stunned display stun UI
            if (stunned)
            {
                ui?.onStun.Invoke();
                OnStunned?.Invoke();
            }
            else
            {
                // If unstunned reset stagger to full
                ui?.onUnstun.Invoke();
                OnUnstunned?.Invoke();
                Stagger = Stats.MaxStagger;
            }
        }
    }
    private bool stunned = false; 
    public float StunProgress 
    {
        set
        {
            ui?.onStunProgressChanged.Invoke(value);
        } 
    }
    public bool Countered => Spell.Countered;
    public Spell Spell
    {
        get => spell;
        set
        {
            spell = value;
            OnSpellChanged?.Invoke(this, spell);
            if (spell == null || ui == null)
                return;
            // Set spell word
            ui.onSpellChanged.Invoke(spell.ToDisplayString());
            // Set spell icon (gets first rootword)
            ui.onSpellIconChanged.Invoke(spell.Icon);
            // Set countered
            ui.onCounterStateChanged.Invoke(Countered);
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
            ui?.onChargeChangedNumber.Invoke(((int)Charge) + "/" + ((int)ChargeTime));
        }
    }
    float charge; // Charge amount (seconds) for enemies
    #endregion

    #region Research + Scouter

    public string ResearchKey => string.IsNullOrWhiteSpace(researchKeyOverride) ? DisplayName : researchKeyOverride;
    [SerializeField] private string researchKeyOverride;

    public float ResearchAmount => researchAmount;
    [SerializeField] private float researchAmount = 0.1f;

    private ScouterData scouterData;
    public ScouterData ScouterData => scouterData ?? (scouterData = GetComponent<ScouterData>());

    #endregion

    private void EnterSpiritMode()
    {
        BStatus = BattleStatus.SpiritMode;
        if (IsDemon) RewardsManager.Instance?.IncrementCasualty();
    }
    private void Kill()
    {
        BStatus = BattleStatus.Dead;
        if (IsDemon) RewardsManager.Instance?.IncrementKill();
    }

    public void Damage(int amount)
    {
        if (amount <= 0) return;
        if (BStatus == BattleStatus.SpiritMode)
            SP -= amount;
        else
            Health -= amount;
        ui?.onDamageReceived.Invoke();
    }

    public void Heal(int amount)
    {
        if (BStatus == BattleStatus.SpiritMode)
            SP += amount;
        else
            Health += amount;
    }

    public void RecalculateMaxHP()
    {
        if (BattleManager.instance != null && !BattleManager.instance.FirstWaveStarted) 
        {
            Health = Stats.MaxHP;
        }
        else
        {
            Health = System.Math.Min(Health, Stats.MaxHP);
        }
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
            Destroy(effect.gameObject);
            statusEffects.Remove(tag);
        }
        RemoveAbilities(tag);

    }
    public void AddTag(CasterTag tag)
    {
        if(tag == null)
        {
            return;
        }
        AddAbilities(tag);
        tags.Add(tag);
    }
    public void AddTag(string tagName)
    {
        AddTag(Lookup.GetCasterTag(tagName));
    }
    public void AddTagWithStatusEffect(StatusEffect effect, CasterTag tag)
    {
        if (statusEffects.ContainsKey(tag))
            return;
        statusEffects.Add(tag, effect);
        AddTag(tag);
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
    private void AddAbilities(CasterTag tag)
    {
        AddAbility(tag.ability1);
        AddAbility(tag.ability2);
        foreach (var subtag in tag.subTags)
        {
            if (!HasTag(subtag))
            {
                AddAbilities(subtag);
            }
        }
    }
    private void AddAbility(CasterAbility ability)
    {
        if (ability == null)
        {
            return;
        }
        ability.AddTo(this);
    }
    private void RemoveAbilities(CasterTag tag)
    {
        RemoveAbility(tag.ability1);
        RemoveAbility(tag.ability2);
        foreach (var subtag in tag.subTags)
        {
            if (!HasTag(subtag))
            {
                RemoveAbilities(subtag);
            }
        }
    }
    private void RemoveAbility(CasterAbility ability)
    {
        if(ability == null)
        {
            return;
        }
        ability.RemoveFrom(this);
    }
    public CasterStats Stats { get => tags.statMod; }

    /// <summary>
    /// The dictionary containing all of this caster's CasterTags.
    /// </summary>
    public CasterTagDictionary TagDict => tags;
    #endregion

    public Battlefield.Position TargetPos { get; set; } = new Battlefield.Position(0, 0);
    public CasterUI ui = null;

    protected virtual void Awake()
    {
        if (ui == null) 
        {
            ui = GetComponentInChildren<CasterUI>();
        }
        tags.RecalculateAggregate();
        foreach (var tag in tags)
        {
            AddAbilities(tag);
        }
        sp = Stats.MaxSP;
        Health = Stats.MaxHP;
        Stagger = Stats.MaxStagger;
        if (ui != null)
        {
            ui.onNameChanged.Invoke(DisplayName);
        }
    }

    [System.Serializable] private class StatusEffectDict : SerializableDictionary<CasterTag, StatusEffect> { }
}
