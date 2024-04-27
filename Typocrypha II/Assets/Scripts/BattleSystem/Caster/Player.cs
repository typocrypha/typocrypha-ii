using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Typocrypha;

[RequireComponent(typeof(ATB3.ATBPlayer))]
public class Player : Caster, IPausable
{
    private const float castFailTextTime = 0.6f;
    #region IPausable
    public PauseHandle PH { get; } = new PauseHandle();

    #endregion

    [SerializeField] private AudioClip castFailureSfx;
    [SerializeField] private AudioClip castSuccessSfx;

    private ATB3.ATBPlayer atbPlayer;

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
    public SpellParser.ParseResults CastString(string[] spellWords)
    {
        var results = SpellParser.instance.Parse(spellWords, PlayerDataManager.instance.equipment.EquippedSpellWords, out var spell, out string problemWord);
        if (results == SpellParser.ParseResults.Valid) 
        {
            // Check cooldowns
            var cooldowns = SpellCooldownManager.instance;
            if(cooldowns.IsOnCooldown(spell, out var wordOnCooldown))
            {
                results = SpellParser.ParseResults.OnCooldown;
                AudioManager.instance.PlaySFX(castFailureSfx);
                SpellFxManager.instance.PlayText(new Vector2(0f, -2f), false, $"{wordOnCooldown.BaseName} on Cooldown", Color.red, castFailTextTime);
            }
            if (results != SpellParser.ParseResults.OnCooldown)
            {
                cooldowns.DoCooldowns(spell);
                Spell = spell;
                atbPlayer.Cast(TargetPos); // Start casting sequence
                //AudioManager.instance.PlaySFX(castSuccessSfx);
            }
        }
        else
        {
            AudioManager.instance.PlaySFX(castFailureSfx);
            if(results == SpellParser.ParseResults.EmptySpell)
            {
                SpellFxManager.instance.PlayText(new Vector2(0f, -2f), false, $"Empty Spell", Color.red, castFailTextTime);
            }
            else if(results == SpellParser.ParseResults.DuplicateWord)
            {
                SpellFxManager.instance.PlayText(new Vector2(0f, -2f), false, $"Duplicate Word: {problemWord.ToUpper()}", Color.red, castFailTextTime);
            }
            else if(results == SpellParser.ParseResults.TypoFailure)
            {
                SpellFxManager.instance.PlayText(new Vector2(0f, -2f), false, $"Invalid Word: {problemWord.ToUpper()}", Color.red, castFailTextTime);
            }
            else
            {
                SpellFxManager.instance.PlayText(new Vector2(0f, -2f), false, $"Invalid Spell", Color.red, castFailTextTime);
            }
        }
        return results;
    }
}
