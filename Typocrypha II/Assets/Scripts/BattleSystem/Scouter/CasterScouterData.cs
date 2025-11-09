using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CasterScouterData : ScouterData
{
    [SerializeField] private Caster caster;
    [SerializeField] private SpriteRenderer casterImage;
    [TextArea(2,4)] 
    [Header("Scouter Text / TIPs Description Text")]
    [SerializeField] private string flavorText;
    [TextArea(2, 8)]
    [Header("TIPs Flavor Text")]
    [SerializeField] private string loreText;

    public string TIPsFlavorText => loreText;
    public override string Description => flavorText;
    public override Sprite Image => casterImage.sprite;

    private CasterUI ui = null;

    private void Awake()
    {
        if ((ui = GetComponentInChildren<CasterUI>()) != null)
        {
            ui.onScouterDataChanged.Invoke(Description);
        }

    }
}
