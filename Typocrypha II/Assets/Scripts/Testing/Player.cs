using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ATB3.ATBPlayer))]
public class Player : Caster
{
    public static readonly char[] separator = { ' ', '-' };

    ATB3.ATBPlayer playerActor;

    new public void Awake()
    {
        base.Awake();
        Stats.MaxHP = 100; // DEBUG
        Health = Stats.MaxHP;
        playerActor = GetComponent<ATB3.ATBPlayer>();
        DisplayName = "Ayin";
        TargetPos = new Battlefield.Position(0, 1);
    }

    /// <summary>
    /// Cast a spell from the keyboard. Called when enter is pressed by player.
    /// Parses spell and, if spell is valid, casts it.
    /// </summary>
    public void CastString(string spellString)
    {
        Spell spell = new Spell();// = new List<SpellWord>();
        var results = SpellParser.instance?.Parse(spellString.TrimEnd().Split(separator), PlayerDataManager.instance.equipment.EquippedWordsDict, out spell);
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
        //if (playerActor.Pause || playerActor.isCast)
        //    return;
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
