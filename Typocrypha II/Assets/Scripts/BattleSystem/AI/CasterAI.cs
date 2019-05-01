using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Caster))]
public abstract class CasterAI : MonoBehaviour
{
    public delegate void AfterCast();
    public delegate void AfterHit(Spell spell, RootWordEffect effect, Caster caster, PopupData data);

    #region Delegate Handles
    public AfterCast OnAfterCast { get; set; } = null;
    public AfterHit OnAfterHit { get; set; } = null;
    #endregion

    public Spell CurrSpell { get; set; }
}
