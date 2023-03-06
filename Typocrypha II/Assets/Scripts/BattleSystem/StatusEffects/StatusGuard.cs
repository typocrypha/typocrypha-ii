using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusGuard : StatusRemoveAfterHitOrCast
{
    private SpellTag.TagSet resistTags = new SpellTag.TagSet();
    private Reaction resistReaction;

    protected override bool DoesHitCount(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults data)
    {
        return base.DoesHitCount(effect, caster, target, spellData, data)
            || (!data.Miss && resistReaction == Reaction.Block && data.Effectiveness == Reaction.Block);
    }
    public override void Apply(ApplyStatusEffect effect, Caster caster, Caster target, CastResults data)
    {
        resistTags.Add(effect.tags);
        resistReaction = data.IsCrit ? Reaction.Block : Reaction.Resist;
        SpellFxManager.instance.LogMessage($"{target.DisplayName} put its shield up!");
    }

    public CasterTagDictionary.ReactionMultiSet GetReactions(SpellTag tag)
    {
        if(resistTags.Contains(tag))
        {
            return new CasterTagDictionary.ReactionMultiSet()
            {
                resistReaction
            };
        }
        return null;
    }

    protected override void Initialize()
    {
        base.Initialize();
        affected.ExtraReactions += GetReactions;
    }

    public override void Cleanup()
    {
        base.Cleanup();
        affected.ExtraReactions -= GetReactions;
    }

    public override void Remove()
    {
        base.Remove();
        SpellFxManager.instance.LogMessage($"{affected.DisplayName}'s shield was lowered");
    }
}
