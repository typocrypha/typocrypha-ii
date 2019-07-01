using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ATB3.ATBPlayer))]
public class DummyCaster : Caster
{
    public GameObject targetRet;
    public char[] separator = { ' ', '-' };

    ATB3.ATBPlayer playerActor;

    new public void Awake()
    {
        base.Awake();
        Stats.MaxHP = 100; // DEBUG
        Health = Stats.MaxHP;
        playerActor = GetComponent<ATB3.ATBPlayer>();
        DisplayName = "Ayin";
    }

    /// <summary>
    /// Cast a spell from the keyboard. Called when enter is pressed by player.
    /// Parses spell and, if spell is valid, casts it.
    /// </summary>
    public void CastString(string spellString)
    {
        Spell words = new Spell();// = new List<SpellWord>();
        var results = SpellParser.instance?.Parse(spellString.Split(separator), out words);
        Debug.Log("Spell cast:" + spellString + ":" + results);
        if (results == SpellParser.ParseResults.Valid) 
        {
            //    foreach (var word in words)
            //    {
            //        if (word is RootWord)
            //        {
            //            if (!SpellCooldownManager.instance.cooldowns.ContainsKey(word.displayName))
            //                SpellCooldownManager.instance.cooldowns[word.displayName] = 0f;
            //            else if (SpellCooldownManager.instance.cooldowns[word.displayName] > 0f)
            //            {
            //                results = CastParser.ParseResults.OnCooldown;
            //                return;
            //            }
            //        }
            //    }
            //    if (results != CastParser.ParseResults.OnCooldown)
            //    {
            //        foreach (RootWord word in words)
            //        {
            //            if (SpellCooldownManager.instance.cooldowns[word.displayName] <= 0f)
            //                SpellCooldownManager.instance.StartCooldown(word.displayName, word.cooldown);
            //        }

            //        FaderManager.instance.FadeAll(0.5f, Color.black);
            //        foreach (var target in words.AllTargets(FieldPos, TargetPos).Where( (a) => a != null)) 
            //        {
            //            target.GetComponent<FaderGroup>().FadeAmount = 0f;
            //            target.GetComponent<FaderGroup>().FadeColor = Color.black;
            //        }
            //        GetComponent<Caster>().Spell = words;
            //        GetComponent<ATB3.ATBPlayer>().Cast(); // Start casting sequence
            //    }
            GetComponent<Caster>().Spell = words;
            GetComponent<ATB3.ATBPlayer>().Cast(); // Start casting sequence
        }
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
        if (targetRet != null)
            targetRet.transform.position = Battlefield.instance.GetSpace(TargetPos);
    }
}
