using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scouter info handler for enemies.
/// </summary>
public class ScouterInfo_Player : ScouterInfo
{
    public ScouterInfo_Player(Caster caster)
    {
        _description = "";
        _description += "Clarke: git good\n"; // LOOKUP CLARKE TIP FROM DATABASE
        string desc = "Your tags: ";
        foreach (var tag in caster.TagDict)
            desc += tag.DisplayName + ", ";
        _description += desc;
    }

    private string _description;
    public override string DescriptionText => _description;
}

