using Gameflow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayDialogEffect : RootWordEffect
{
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        SpellManager.instance.LogDelay(PlayDialog(GetDialogData(caster, target, spellData, mod, prevResults)));
        return InitializeCastResults(caster, target, mod);
    }

    protected abstract Data GetDialogData(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults);

    private IEnumerator PlayDialog(Data data)
    {
        bool complete = false;
        void OnComplete() => complete = true;
        DialogManager.instance.StartDialog(data.dialog, true, data.auto, OnComplete);
        bool IsComplete() => complete;
        yield return new WaitUntil(IsComplete);
    }

    protected class Data
    {
        public DialogCanvas dialog;
        public bool auto;
    }
}
