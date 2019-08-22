using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scouter info for enemies.
/// </summary>
public class ScouterInfo_Enemy : ScouterInfo
{
    public ScouterInfo_Enemy(Caster caster)
    {
        string desc = "tags: ";
        foreach (var tag in caster.TagDict)
            desc += tag.DisplayName + ", ";
        _description = desc;
    }

    private string _description;
    public override string DescriptionText => _description;
}
