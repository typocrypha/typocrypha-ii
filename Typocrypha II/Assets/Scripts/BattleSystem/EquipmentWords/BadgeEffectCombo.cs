using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectCombo : BadgeEffect
{
    public const float baseCritChance = 0.03f;
    private const int comboThreshold = 10;
    private int combo;
    public override void Equip(Player player)
    {
        ResetCombo();
        player.OnAfterCastResolved -= AfterCastResolved;
        player.OnAfterCastResolved += AfterCastResolved;
        player.OnCastFail -= ResetCombo;
        player.OnCastFail += ResetCombo;
        player.OnPromptComplete -= OnPromptComplete;
        player.OnPromptComplete += OnPromptComplete;
        player.OnAfterHitResolved -= OnAfterHitResolved;
        player.OnAfterHitResolved += OnAfterHitResolved;
        player.AddActiveAbilities(Caster.ActiveAbilities.Combo);
    }

    private void OnAfterHitResolved(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults data)
    {
        if((caster.CasterState == Caster.State.Hostile || caster.CasterState == Caster.State.Neutral) && data.WillDealDamage)
        {
            ResetCombo();
        }
    }

    public override void Unequip(Player player)
    {
        ResetCombo();
        player.OnAfterCastResolved -= AfterCastResolved;
        player.OnCastFail -= ResetCombo;
        player.OnPromptComplete -= OnPromptComplete;
        player.OnAfterHitResolved -= OnAfterHitResolved;
        player.RemoveActiveAbilities(Caster.ActiveAbilities.Combo);
    }

    private void AfterCastResolved(Spell s, Caster caster, bool hitTarget)
    {
        if (hitTarget)
        {
            IncreaseCombo();
        }
        else
        {
            ResetCombo();
        }
    }

    private void OnPromptComplete(bool success)
    {
        if (success)
        {
            IncreaseCombo();
        }
        else
        {
            ResetCombo();
        }
    }

    private void IncreaseCombo()
    {
        ++combo;
        SpellFxManager.instance.PlayText(Battlefield.instance.Player.FieldPos, $"Combo x{combo}!", Color.green);
    }

    private void ResetCombo()
    {
        if (combo == 0)
            return;
        combo = 0;
        var pos = Battlefield.instance.GetSpaceScreenSpace(Battlefield.instance.Player.FieldPos) + (Vector2.down * 75);
        SpellFxManager.instance.PlayText(pos, true, $"Combo Broken", Color.red);
    }

    public bool CanFollowUp(Caster player)
    {
        if (combo < comboThreshold)
            return false;
        int comboScore = combo - comboThreshold;
        return UnityEngine.Random.Range(0, 1f) <= Mathf.Max(0, baseCritChance + (comboScore * ((player.Stats.Luck + 1) * 0.01f)));
    }
}
