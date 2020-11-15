using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ATB3.ATBPlayer))]
public class Player : Caster
{
    public char[] separator = { ' ', '-' };

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
        Spell words = new Spell();// = new List<SpellWord>();
        var results = SpellParser.instance?.Parse(spellString.Split(separator), PlayerEquipment.instance.EquippedWordsDict, out words);
        if (results == SpellParser.ParseResults.Valid) 
        {
            // Check cooldowns
            var cooldowns = SpellCooldownManager.instance;
            if(words.Any((w) => cooldowns.IsOnCooldown(w.Key)))
            {
                results = SpellParser.ParseResults.OnCooldown;
            }
            if (results != SpellParser.ParseResults.OnCooldown)
            {
                // Lower Cooldowns of all words currently on cooldown by the number of words in the successful spell
                cooldowns.ModifyAllCooldowns(-words.Count);
                foreach (var word in words)
                    cooldowns.StartCooldown(word.Key);
                GetComponent<Caster>().Spell = words;
                GetComponent<ATB3.ATBPlayer>().Cast(); // Start casting sequence
            }
        }
        Debug.Log("Spell cast:" + spellString + ":" + results);
    }
    private void Update()
    {
        if (playerActor.Pause || playerActor.isCast)
            return;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            TargetPos.Col = Mathf.Max(0, TargetPos.Col - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            TargetPos.Col = Mathf.Min(2, TargetPos.Col + 1);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            TargetPos.Row = 0;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            TargetPos.Row = 1;
    }
}
