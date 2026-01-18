using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Typocrypha;

[RequireComponent(typeof(ATB3.ATBPlayer))]
public class Player : Caster, IPausable
{
    #region IPausable
    public PauseHandle PH { get; } = new PauseHandle();

    #endregion


    [SerializeField] private AudioClip castSuccessSfx;

    private ATB3.ATBPlayer atbPlayer;

    public event System.Action OnCastFail;
    public System.Action<bool> OnPromptComplete { get; set; }

    protected override void Awake()
    {
        ui = Typocrypha.Keyboard.instance.PlayerUI;
        base.Awake();
        TargetPos = new Battlefield.Position(0, 1);
        atbPlayer = GetComponent<ATB3.ATBPlayer>();
        OnSpiritMode += BattleManager.instance.GameOver;
    }

    void OnDestroy()
    {
        OnSpiritMode -= BattleManager.instance.GameOver;
    }

    /// <summary>
    /// Cast a spell from the keyboard. Called when enter is pressed by player.
    /// Parses spell and, if spell is valid, casts it.
    /// </summary>
    private SpellParser.ParseResults CastString(string[] spellWords, Battlefield.Position targetPosition, bool insertCast)
    {
        var cooldowns = SpellCooldownManager.instance;
        var results = SpellParser.instance.Parse(spellWords, cooldowns.GetSpellsDict(), true, out var spell, out string problemWord);
        if (results == SpellParser.ParseResults.Valid) 
        {
            // Check cooldowns
            if(cooldowns.IsOnCooldown(spell, out var wordOnCooldown))
            {
                SpellFxManager.instance.CastFailFx($"{wordOnCooldown.BaseName} on Cooldown");
                OnCastFail?.Invoke();
                return SpellParser.ParseResults.OnCooldown;
            }
            cooldowns.DoCooldowns(spell);
            if (insertCast)
            {
                atbPlayer.InsertCast(targetPosition, spell, null);
            }
            else
            {
                Spell = spell;
                atbPlayer.Cast(targetPosition); // Start casting sequence
            }
            //AudioManager.instance.PlaySFX(castSuccessSfx);
        }
        else
        {
            if(results == SpellParser.ParseResults.EmptySpell)
            {
                SpellFxManager.instance.CastFailFx($"Empty Spell");
            }
            else if(results == SpellParser.ParseResults.DuplicateWord)
            {
                SpellFxManager.instance.CastFailFx($"Duplicate Word: {problemWord.ToUpper()}");
            }
            else if(results == SpellParser.ParseResults.TypoFailure)
            {
                SpellFxManager.instance.CastFailFx($"Invalid Word: {problemWord.ToUpper()}");
            }
            else if(results == SpellParser.ParseResults.TooManyRoots)
            {
                SpellFxManager.instance.CastFailFx($"Too Many Words!");
            }
            else
            {
                SpellFxManager.instance.CastFailFx($"Invalid Spell");
            }
            OnCastFail?.Invoke();
        }
        return results;
    }

    public SpellParser.ParseResults CastString(string[] spellWords)
    {
        return CastString(spellWords, TargetPos, false);
    }

    public SpellParser.ParseResults CastStringInsert(string[] spellWords, Battlefield.Position targetPosition)
    {
        return CastString(spellWords, targetPosition, true);
    }

    public void InsertCast(Spell spell, Battlefield.Position targetPosition, string messageOverride = null)
    {
        atbPlayer.InsertCast(targetPosition, spell, null, messageOverride);
    }
}
