using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnBattleWordEffect : RootWordEffect
{
    public enum SequencePosition
    {
        First,
        Middle,
        Last,
        Solo,
    }
    private const float baseWordTime = 1.25f;
    private const float letterTime = 0.25f;
    private const float multiWordTimerDelta = -0.25f;
    private static readonly WaitForSeconds staggerYielder = new WaitForSeconds(0.25f);

    [SerializeField] private SequencePosition sequencePosition = SequencePosition.Solo;

    private Caster self;

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        self = caster;
        SpellManager.instance.LogDelay(Play(caster.transform));
        return new CastResults(caster, target)
        {
            DisplayDamage = false,
            Miss = false,
        };
    }

    protected abstract IReadOnlyList<WordData> GetWordData();

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

    private IEnumerator Play(Transform parent)
    {
        if (sequencePosition == SequencePosition.First || sequencePosition == SequencePosition.Solo)
        {
            HideUI();
        }
        var wordData = GetWordData();
        var focusedWords = new List<BattleWord>(wordData.Count);
        float focusTime = 0;
        for (int i = 0; i < wordData.Count; i++)
        {
            WordData data = wordData[i];
            var word = Instantiate(data.prefab, parent).GetComponent<BattleWord>(); // TODO: use pooling
            word.SetText(data.text);
            focusTime += baseWordTime + (letterTime * data.text.Length);
            word.Focus(focusTime, BattleWord.GetFocusPosition(data.focusPosition), i == 0, 5 - i);
            focusTime += multiWordTimerDelta;
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
        if (sequencePosition == SequencePosition.Last || sequencePosition == SequencePosition.Solo)
        {
            ShowUI();
        }
    }

    [System.Serializable]
    public class WordData
    {
        public string text;
        public GameObject prefab;
        public BattleWord.FocusPosition focusPosition;
    }
}
