using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CasterScouterData : ScouterData
{
    [SerializeField] private Caster caster;
    [SerializeField] private SpriteRenderer casterImage;
    [TextArea(2,4)]
    [SerializeField] private string flavorText;
    public override string Description
    {
        get
        {
            string tagString = string.Join(", ", caster.TagDict.Select(tag => tag.DisplayName));
            return $"{caster.DisplayName}:\n{tagString}\n{flavorText}";
        }
    }

    public override Sprite Image => casterImage.sprite;

    public CasterUI ui = null;

    private void Awake()
    {
        if ((ui = GetComponentInChildren<CasterUI>()) != null)
        {
            ui.onScouterDataChanged.Invoke(Description);
        }

    }
}
