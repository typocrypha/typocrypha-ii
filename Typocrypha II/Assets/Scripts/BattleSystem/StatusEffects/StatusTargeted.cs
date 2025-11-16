using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusTargeted : StatusRemoveAfterHitOrCast
{
    public CasterTagDictionary.ReactionMultiSet GetReactions(SpellTag tag)
    {
        return new CasterTagDictionary.ReactionMultiSet()
        {
            Reaction.Weak,
        };
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
