using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scouter info handler for enemies.
/// </summary>
public class ScouterInfo_Enemy : ScouterInfo
{
    public ScouterInfo_Enemy(Caster caster)
    {
        _description = "";
        var key = caster.name.Substring(0, caster.name.Length - 7); // Get enemy name (remove "(clone)").
        var data = Scouter.GetScouterData(key); // Lookup scouter data.
        _description += data.description;
        string desc = "\nEnemy tags: ";
        foreach (var tag in caster.TagDict)
            desc += tag.DisplayName + ", ";
        _description += desc;
        DisplayImage = data.pic; // TEMP IMAGE
    }

    private string _description;
    public override string DescriptionText => _description;
}
