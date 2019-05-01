using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Caster))]
public  class CasterAI : MonoBehaviour
{
    public delegate void AfterCast();
    public delegate void AfterHit(Spell spell, RootWordEffect effect, Caster caster, PopupData data);
    public delegate void UpdateFn();

    #region Delegate Handles
    public AfterCast OnAfterCast { get; set; } = null;
    public AfterHit OnAfterHit { get; set; } = null;
    public UpdateFn OnUpdate { get; set; } = null;
    #endregion

    private void LateUpdate()
    {
        OnUpdate?.Invoke();
    }

    public Spell CurrSpell { get; set; }
}
