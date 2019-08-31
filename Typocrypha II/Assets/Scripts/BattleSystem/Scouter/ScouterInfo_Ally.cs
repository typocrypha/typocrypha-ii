using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scouter info handler for enemies.
/// </summary>
public class ScouterInfo_Ally : ScouterInfo
{
    public ScouterInfo_Ally(Caster caster)
    {
        _description = "";
        _description += "I'm your ally! Also, git good.\n"; // LOOKUP ALLY DIALOG FROM DATABASE
        string desc = "Ally tags: ";
        foreach (var tag in caster.TagDict)
            desc += tag.DisplayName + ", ";
        _description += desc;
    }

    private string _description;
    public override string DescriptionText => _description;
}
