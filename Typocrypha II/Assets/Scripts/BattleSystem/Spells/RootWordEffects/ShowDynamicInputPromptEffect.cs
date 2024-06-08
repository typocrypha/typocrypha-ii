using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDynamicInputPromptEffect : BaseShowInputPromptEffect
{
    [SerializeField] private int index;

    private IPromptProvider provider;
    private bool valid;

    protected override void Initialize(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults)
    {
        base.Initialize(caster, target, spellData, mod, prevResults);
        provider = caster.GetComponent<IPromptProvider>();
        valid = provider != null;
    }

    protected override string Title => valid ? provider.Title(index) : string.Empty;
    protected override string Prompt => valid ? provider.Prompt(index) : string.Empty;
    protected override float Time => valid ? provider.Time(index) : 1;
}
