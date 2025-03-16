using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEFunctionMariEvent : BattleEventFunction
{
    public enum EventID
    {
        ChangeForm,
        SpawnWords,
        TransitionUI,
    }
    [SerializeField] private EventID id;
    public override void Run()
    {
        var mari = Battlefield.instance.GetCaster(new Battlefield.Position(0, 1));
        if (mari == null)
            return;
        var spiritFormAI = mari.GetComponent<AIMariSpritForm>();
        if (spiritFormAI == null)
            return;
        if(id == EventID.ChangeForm)
        {
            spiritFormAI.ChangeToSpiritForm();
        }
        else if(id == EventID.SpawnWords)
        {
            spiritFormAI.SpawnBattleWords();
        }
        else if(id == EventID.TransitionUI)
        {
            spiritFormAI.TransitionUI();
        }
    }
}
