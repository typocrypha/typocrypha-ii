using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnBattleWordEffect : RootWordEffect
{
    private const float baseWordTime = 1.2f;
    private const float letterTime = 0.2f;
    private const float multiWordTimerDelta = -0.75f;
    private static readonly WaitForSeconds staggerYielder = new WaitForSeconds(0.25f);

    private Caster self;

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        self = caster;
        SpellManager.instance.LogDelay(Play(caster, caster.transform));
        return InitializeCastResults(caster, target, mod);
    }

    protected abstract IReadOnlyList<BattleWord.SequenceData> GetSequenceData(Caster caster, out GameObject defaultPrefab);

    private void HideUI()
    {
        Typocrypha.Keyboard.instance.PH.Pause(PauseSources.Misc);
        TargetReticle.instance.PH.Pause(PauseSources.Misc);
        self.ui.gameObject.SetActive(false);
        AllyBattleBoxManager.instance.HideCharacter();
        SpellCooldownManager.instance.Hide();
    }

    private void ShowUI()
    {
        Typocrypha.Keyboard.instance.PH.Unpause(PauseSources.Misc);
        TargetReticle.instance.PH.Unpause(PauseSources.Misc);
        self.ui.gameObject.SetActive(true);
        AllyBattleBoxManager.instance.ShowCharacter();
        SpellCooldownManager.instance.Show();
    }

    private IEnumerator Play(Caster caster, Transform parent)
    {
        HideUI();
        foreach(var sequenceData in GetSequenceData(caster, out var defaultPrefab))
        {
            var focusedWords = new List<BattleWord>(sequenceData.words.Count);
            float focusTime = 0;
            for (int i = 0; i < sequenceData.words.Count; i++)
            {
                var data = sequenceData.words[i];
                var prefab = data.prefabOverride != null ? data.prefabOverride : defaultPrefab;
                var word = Instantiate(prefab, parent).GetComponent<BattleWord>(); // TODO: use pooling
                word.SetText(data.text);
                focusTime += baseWordTime + (letterTime * data.text.Length) + (multiWordTimerDelta * i);
                word.Focus(focusTime, BattleWord.GetFocusPosition(data.position), i == 0, 5 - i);
                focusedWords.Add(word);
                yield return staggerYielder;
            }
            for (int i = 0; i < focusedWords.Count; i++)
            {
                var word = focusedWords[i];
                if (i > 0)
                {
                    if (word.PendingFocus)
                    {
                        yield return new WaitWhile(() => word.PendingFocus);
                    }
                    word.SetTarget();
                }
                yield return new WaitWhile(() => word.isActiveAndEnabled);
            }
        }

        ShowUI();
    }
}
