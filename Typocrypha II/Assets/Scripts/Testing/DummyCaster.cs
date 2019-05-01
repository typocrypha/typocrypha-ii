using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ATB3.ATBPlayer))]
public class DummyCaster : Caster
{
    public Spell currSpell;
    public GameObject targetRet;
    public char[] separator = { ' ', '-' };

    ATB3.ATBPlayer playerActor;

    public void Awake()
    {
        playerActor = GetComponent<ATB3.ATBPlayer>();
    }

    public void Cast()
    {
        SpellManager.instance.Cast(currSpell, this, TargetPos);
    }
    public void CastString(string spellString)
    {
        Spell words;// = new List<SpellWord>();
        var results = CastParser.instance.Parse(spellString.Split(separator), out words);
        Debug.Log(results);
        if (results == CastParser.ParseResults.Valid)
        {
            foreach (var word in words)
            {
                if (word is RootWord)
                {
                    if (!SpellCooldownManager.instance.cooldowns.ContainsKey(word.displayName))
                        SpellCooldownManager.instance.cooldowns[word.displayName] = 0f;
                    else if (SpellCooldownManager.instance.cooldowns[word.displayName] > 0f)
                    {
                        results = CastParser.ParseResults.OnCooldown;
                        return;
                    }
                }
            }
            if (results != CastParser.ParseResults.OnCooldown)
            {
                foreach (RootWord word in words)
                {
                    if (SpellCooldownManager.instance.cooldowns[word.displayName] <= 0f)
                        SpellCooldownManager.instance.StartCooldown(word.displayName, word.cooldown);
                }
                
                playerActor.currSpell = words;
                //SpellManager.instance.Cast(words.ToArray(), this, tPos);
                GetComponent<ATB3.ATBPlayer>().cast();
            }
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
        targetRet.transform.position = Battlefield.instance.GetSpace(TargetPos);
    }
}
