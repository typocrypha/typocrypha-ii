using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellRestrictions : MonoBehaviour
{
    public delegate bool Restriction(Spell spell, Caster caster, Battlefield.Position target, bool showGUI = false);
    public static SpellRestrictions instance;

    public Restriction restrictions;

    /// <summary> Singleton implementation </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public bool IsRestricted(Spell spell, Caster caster, Battlefield.Position target, bool showGUI = false)
    {
        if (restrictions == null)
            return false;
        var restrictionList = restrictions.GetInvocationList();
        bool isRestricted = false;
        foreach(var restriction in restrictionList)
            if ((restriction as Restriction).Invoke(spell, caster, target, showGUI))
                isRestricted = true;
        return isRestricted;
    }
}
