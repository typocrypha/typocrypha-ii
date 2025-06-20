using Gameflow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDialogEffect : BasePlayDialogEffect
{
    [SerializeField] private DialogCanvas dialog;
    [SerializeField] private bool auto = true;
    protected override Data GetDialogData(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults)
    {
        return new Data() { dialog = dialog, auto = auto };
    }
}