using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ATB3.ATBPlayer))]
public class Player : Caster, IPausable
{
    public static readonly char[] separator = { ' ', '-' };

    #region IPausable
    public PauseHandle PH { get; private set; }

    public void OnPause(bool pause)
    {
        // Stop input (target control)
        enabled = !pause;
    }
    #endregion

    protected override void Awake()
    {
        ui = Typocrypha.Keyboard.instance.PlayerUI;
        base.Awake();
        TargetPos = new Battlefield.Position(0, 1);
        PH = new PauseHandle(OnPause);
    }

    private void Start()
    {
        PH.SetParent(BattleManager.instance.PH);
    }

    /// <summary>
    /// Cast a spell from the keyboard. Called when enter is pressed by player.
    /// Parses spell and, if spell is valid, casts it.
    /// </summary>
    public void CastString(string spellString)
    {
        var results = SpellParser.instance.Parse(spellString.TrimEnd().Split(separator), PlayerDataManager.instance.equipment.EquippedWords, out var spell);
        if (results == SpellParser.ParseResults.Valid) 
        {
            // Check cooldowns
            var cooldowns = SpellCooldownManager.instance;
            if(cooldowns.IsOnCooldown(spell, out _))
            {
                results = SpellParser.ParseResults.OnCooldown;
            }
            if (results != SpellParser.ParseResults.OnCooldown)
            {
                cooldowns.DoCooldowns(spell);
                GetComponent<Caster>().Spell = spell;
                GetComponent<ATB3.ATBPlayer>().Cast(TargetPos); // Start casting sequence
            }
        }
        Debug.Log("Spell cast:" + spellString + ":" + results);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            TargetPos.Col = Mathf.Max(0, TargetPos.Col - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            TargetPos.Col = Mathf.Min(2, TargetPos.Col + 1);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            TargetPos.Row = 0;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            TargetPos.Row = 1;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            var field = Battlefield.instance;
            var newPos = new Battlefield.Position(TargetPos);
            do
            {
                ++newPos.Col;
                if (newPos.Col >= field.Columns)
                    newPos.Col = 0;
                var caster = field.GetCaster(newPos);
                if (caster != null && caster.BStatus != BattleStatus.Dead && caster.BStatus != BattleStatus.Fled)
                    break;
            }
            while (newPos.Col != TargetPos.Col);
            TargetPos.Col = newPos.Col;
        }
    }
}
