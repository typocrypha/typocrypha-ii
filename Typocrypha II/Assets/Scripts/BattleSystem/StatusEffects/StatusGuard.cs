using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusGuard : StatusRemoveAfterHitOrCast
{
    private SpellTag.TagSet resistTags = new SpellTag.TagSet();
    private Reaction resistReaction;
    public override void Apply(ApplyStatusEffect effect, Caster caster, Caster target, CastResults data)
    {
        resistTags.Add(effect.tags);
        resistReaction = data.IsCrit ? Reaction.Block : Reaction.Resist;      
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
}
