using System;
using System.IO;
using UnityEngine;

/// <summary>
/// TIPS entry spell word variant
/// </summary>
[CreateAssetMenu]
[System.Serializable]
public class TIPSEntryDemon : TIPSEntryData
{
    public GameObject prefabEnemy;

    public CasterScouterData GetScouter() => prefabEnemy.GetComponent<CasterScouterData>();
    public Sprite GetSprite() => GetScouter().Image;

    const string footerStyle = "<color=#AAAAAA><size=14>";

#if UNITY_EDITOR
    public override void OnValidate()
    {
        base.OnValidate();

        var caster = prefabEnemy.GetComponent<Caster>();
        var hp = $"HP: {caster.Stats.MaxHP}";
        var sp = $"SP: {caster.Stats.MaxSP}";
        var stag = $"Stagger: {caster.Stats.MaxStagger}";

        var scouter = prefabEnemy.GetComponent<CasterScouterData>();

        Content = string.Join("\n", hp, sp, stag, "", scouter.Description);
        Footer = footerStyle + scouter.TIPsFlavorText;
    }
#endif
}