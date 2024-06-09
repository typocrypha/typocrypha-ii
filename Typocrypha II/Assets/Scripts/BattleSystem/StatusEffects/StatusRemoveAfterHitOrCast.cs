using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusRemoveAfterHitOrCast : StatusEffect
{
    public bool removeFromHits = true;
    public bool onlyCountLastHitOfCast;
    public int hitsToRemove = 1;
    public bool removeFromCasts = true;
    public int castsToRemove = 1;
    bool firstCastDone = false;
    private int casts;
    private int hits;

    public override string FailMessage(Caster caster, Caster target)
    {
        return "But it failed!";
    }

    protected virtual bool DoesHitCount(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults data)
    {
        // Only count hits that would deal damage and are not misses
        return data.WillDealDamage;
    }
    private void OnAfterHit(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults data)
    {
        if (!removeFromHits)
            return;
        // Only count damage-dealing hits
        if (!DoesHitCount(effect, caster, target, spellData, data))
            return;
        if (onlyCountLastHitOfCast && !spellData.IsLastRoot)
            return;
        if(++hits >= hitsToRemove)
            Remove();
    }

    private void OnAfterCastResolved(Spell s, Caster self, bool hitTarget)
    {
        if (!removeFromCasts)
            return;
        if(!firstCastDone)
        {
            firstCastDone = true;
            return;
        }
        if(++casts >= castsToRemove)
            Remove();
    }

    protected override void Initialize()
    {
        base.Initialize();
        affected.OnAfterHitResolved += OnAfterHit;
        affected.OnAfterSpellEffectResolved += OnAfterCastResolved;
    }

    public override void Cleanup()
    {
        base.Cleanup();
        affected.OnAfterHitResolved -= OnAfterHit;
        affected.OnAfterSpellEffectResolved -= OnAfterCastResolved;
    }
}
