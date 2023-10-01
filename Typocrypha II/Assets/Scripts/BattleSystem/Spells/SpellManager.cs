using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpellFxManager))]
[RequireComponent(typeof(SpellRestrictions))]
public class SpellManager : MonoBehaviour
{
    private const float delayBetweenTargets = 0.1f;
    private const float delayBeforeLog = 0.25f;
    private const float runLogTime = 0.5f;
    public static SpellManager instance;
    public SpellWord counterWord;
    [SerializeField] private SpellWord runWord;
    [SerializeField] private SpellWord runAllWord;

    [Header("Interactive Popups")]
    [SerializeField] private InteractivePopup critPopup;
    [SerializeField] private InteractivePopup decodePopup;

    /// <summary> Singleton implementation </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary> 
    /// Cast the spell's effect with a given caster at a given target position 
    /// Returns the cast coroutine (in case the end of casting must be waited on)
    /// </summary>
    public Coroutine Cast(Spell spell, Caster caster, Battlefield.Position target, string castMessageOverride = null, bool isTopLevel = true)
    {
        return StartCoroutine(CastCR(spell, caster, target, castMessageOverride, isTopLevel));
    }

    /// <summary> 
    /// Cast the spell's effect with a given caster at a given target position, and then Cancels 
    /// Returns the case coroutine (in case the end of casting must be waited on)
    /// </summary>
    public Coroutine CastAndCounter(Spell spell, Caster caster, Battlefield.Position target, string castMessageOverride = null, bool isTopLevel = true)
    {
        var targetCaster = Battlefield.instance.GetCaster(target);
        if (targetCaster == null)
            return StartCoroutine(CastCR(spell, caster, target, castMessageOverride, isTopLevel));
        return StartCoroutine(CastAndCounterCR(spell, caster, target, (c) => c == targetCaster, castMessageOverride, isTopLevel));
    }
    /// <summary> Modify the root words by the modifiers and return the modified roots </summary>
    public List<RootWord> Modify(Spell spell)
    {
        var roots = new List<RootWord>(spell.Count);
        var cloneWords = new List<SpellWord>(spell.Count);
        foreach(var word in spell)
        {
            cloneWords.Add(word.Clone());
        }
        for (int i = 0; i < cloneWords.Count; ++i)
        {
            if(cloneWords[i] is ModifierWord mod)
            {
                mod.Modify(cloneWords, i);
            }
            else if(cloneWords[i] is RootWord root)
            {
                roots.Add(root);
            }
        }
        return roots;
    }
    /// <summary> Cast the spell effects and play the associated fx</summary>
    private IEnumerator CastCR(Spell spell, Caster caster, Battlefield.Position target, string castMessageOverride, bool isTopLevel)
    {
        // If the spell is restricted, break and do not cast
        if (SpellRestrictions.instance.IsRestricted(spell, caster, target, true))
        {
            if (SpellFxManager.instance.HasMessages)
            {
                yield return new WaitForSeconds(delayBeforeLog);
                yield return SpellFxManager.instance.PlayMessages();
            }
            yield break;
        }
        if(!caster.IsPlayer || !isTopLevel)
        {
            if(spell.Count == 1 && SpellWord.CompareKeys(spell[0], runWord))
            {
                if(spell[0] is RootWord root && root.effects.Count > 0 && root.effects[0].pattern.Target(caster.FieldPos, target).Count > 1)
                {
                    SpellFxManager.instance.LogMessage(castMessageOverride ?? $"{caster.DisplayName} and crew ran away!", spell.Icon);
                }
                else
                {
                    SpellFxManager.instance.LogMessage(castMessageOverride ?? $"{caster.DisplayName} ran away!", spell.Icon, runLogTime);
                }
            }
            else if(spell.Count == 1 && SpellWord.CompareKeys(spell[0], runAllWord))
            {
                SpellFxManager.instance.LogMessage(castMessageOverride ?? $"{caster.DisplayName} and crew ran away!", spell.Icon);
            }
            else
            {
                SpellFxManager.instance.LogMessage(castMessageOverride ?? $"{caster.DisplayName} casts {spell.ToDisplayString()}", spell.Icon);
            }

            yield return SpellFxManager.instance.PlayMessages();
        }
        var roots = Modify(spell);
        // Critical chance
        Damage.SpecialModifier mod = Damage.SpecialModifier.None;
        if (roots.Any((r) => r.effects.Any((e) => e.CanCrit)) && UnityEngine.Random.Range(0, 1f) <= Damage.baseCritChance)
        {
            bool friendly = caster.IsPlayer || caster.CasterState == Caster.State.Ally;
            IEnumerator OnCritPopupComplete(bool popupSuccess)
            {
                if (friendly)
                {
                    if (popupSuccess)
                    {
                        mod = Damage.SpecialModifier.Critical;
                    }
                }
                else
                {
                    mod = popupSuccess ? Damage.SpecialModifier.CritBlock : Damage.SpecialModifier.Critical;
                }
                return null;
            }
            LogInteractivePopup(critPopup, "Critical Chance!", friendly ? "CRITICAL" : "BLOCK", 5, OnCritPopupComplete);
            yield return StartCoroutine(PlayPrompts());
        }
        var casterSpace = Battlefield.instance.GetSpaceScreenSpace(caster.FieldPos);
        List<Coroutine> crList = new List<Coroutine>();
        for (int rootIndex = 0; rootIndex < roots.Count; rootIndex++)
        {
            var root = roots[rootIndex];
            var spellData = new RootCastData(spell, roots, rootIndex);
            // Log the effect of each effect
            var rootResults = new RootCastResults();
            foreach (var effect in root.effects)
            {
                // Get the effect's targets
                var targets = effect.pattern.Target(caster.FieldPos, target);
                // Log the effect of each effect
                var effectResults = new List<CastResults>();
                crList.Clear();
                foreach (var t in targets)
                {
                    var targetCaster = Battlefield.instance.GetCaster(t);
                    var targetSpace = Battlefield.instance.GetSpaceScreenSpace(t);
                    if (targetCaster == null || targetCaster.BStatus == Caster.BattleStatus.Dead || targetCaster.BStatus == Caster.BattleStatus.Fled)
                    {
                        caster.OnNoTargetHit?.Invoke(t);
                        crList.Add(SpellFxManager.instance.NoTargetFx(targetSpace));
                    }
                    else
                    {
                        // Apply the rule effect if necessary
                        Rule.ActiveRule?.ApplyToEffect(effect, caster, targetCaster);
                        // Apply OnCast Callbacks
                        caster.OnBeforeSpellEffectResolved?.Invoke(effect, caster, targetCaster);
                        // Cast the effect
                        var castResults = effect.Cast(caster, targetCaster, spellData, mod, rootResults);
                        // Apply OnHit Callbacks (Updates AI)
                        targetCaster.OnAfterHitResolved?.Invoke(effect, caster, targetCaster, spellData, castResults);
                        // Play Effects
                        var fx = new SpellFxData[] { root.leftMod?.fx, effect.fx, root.rightMod?.fx };
                        crList.Add(SpellFxManager.instance.Play(fx, castResults, targetSpace, casterSpace));
                        // Log the results of this target
                        effectResults.Add(castResults);
                        // Wait for delay between targets
                        yield return new WaitForSeconds(delayBetweenTargets);
                    }
                }
                // Wait for all of the animations to finish
                foreach (var cr in crList)
                    yield return cr;
                // Apply callbacks after the effect is finished
                caster.OnAfterSpellEffectResolved?.Invoke(spell, caster);
                if (SpellFxManager.instance.HasMessages)
                {
                    yield return new WaitForSeconds(delayBeforeLog);
                    yield return SpellFxManager.instance.PlayMessages();
                }
                if (HasInterrupts)
                {
                    yield return StartCoroutine(ProcessInterrupts());
                }
                // Log the effects of this effect
                rootResults.Add(effectResults);
            }
            if (caster.IsPlayer && PlayerDataManager.instance != null)
            {
                PlayerDataManager.instance.equipment.UnlockWord(root);
            }
        }
        if (mod == Damage.SpecialModifier.Critical)
        {
            SpellFxManager.instance.LogMessage("A critical hit!");
            yield return SpellFxManager.instance.PlayMessages();
        }
        if (HasPrompts)
        {
            yield return StartCoroutine(PlayPrompts());
        }
        // Apply callbacks after the whole cast is finished
        caster.OnAfterCastResolved?.Invoke(spell, caster);
        if (SpellCooldownManager.instance.Overheated)
        {
            SpellCooldownManager.instance.DoOverheat();
        }
    }

    private IEnumerator CastAndCounterCR(Spell spell, Caster caster, Battlefield.Position target, Func<Caster, bool> pred, string castMessageOverride, bool isTopLevel)
    {
        yield return StartCoroutine(CastCR(spell, caster, target, castMessageOverride, isTopLevel));
        var cancelTargets = Battlefield.instance.Casters.Where(pred);
        foreach (var cancelTarget in cancelTargets)
        {
            if (cancelTarget.Spell == null || cancelTarget.Stunned || cancelTarget == caster)
                continue;
            var remainingWords = cancelTarget.Spell.Where((word) => !spell.Contains(word));
            // No words were countered, continue to next target
            if (remainingWords.Count() == cancelTarget.Spell.Count)
                continue;
            // Full counter (no remaining roots)
            if (remainingWords.Count((w) => w is RootWord) <= 0)
            {
                cancelTarget.Spell = new Spell(counterWord);
            }
            else // Partial counter
            {
                cancelTarget.Spell = new Spell(remainingWords);
            }
            SpellFxManager.instance.CounterFx(cancelTarget.FieldPos);
            cancelTarget.OnCounter?.Invoke(cancelTarget);
        }
    }

    public void LogInterruptCast(Spell spell, Caster caster, Battlefield.Position target, string messageOverride = null)
    {
        interrupts.Enqueue(new InterruptData(spell, caster, target, messageOverride, false));
    }

    private IEnumerator ProcessInterrupts()
    {
        while (HasInterrupts)
        {
            var interrupt = interrupts.Dequeue();
            if (interrupt.caster == null || interrupt.caster.IsDeadOrFled)
                continue;
            if (interrupt.canCounter)
            {
                yield return CastAndCounter(interrupt.spell, interrupt.caster, interrupt.target, interrupt.msgOverride, false);
            }
            else
            {
                yield return Cast(interrupt.spell, interrupt.caster, interrupt.target, interrupt.msgOverride, false);
            }
        }
    }

    public bool HasInterrupts => interrupts.Count > 0;

    public void LogInteractivePopup(InteractivePopup popup, string title, string prompt, float time, Func<bool, IEnumerator> onComplete = null)
    {
        popupRequests.Enqueue(new PopupData() { popup = popup, title = title, prompt = prompt, time = time, onComplete = onComplete });
    }

    public void LogDecodePopup(string title, string prompt, float time, Func<bool, IEnumerator> onComplete = null)
    {
        LogInteractivePopup(decodePopup, title, prompt, time, onComplete);
    }

    private IEnumerator PlayPrompts()
    {
        while(HasPrompts)
        {
            var req = popupRequests.Dequeue();
            var popup = req.popup;
            if (popup == null)
                continue;
            yield return popup.Show(req.title, req.prompt, req.time);
            var nextAction = req.onComplete?.Invoke(popup.LastPromptSuccess);
            if(nextAction != null)
            {
                yield return StartCoroutine(nextAction);
            }
        }
    }

    private bool HasPrompts => popupRequests.Count > 0;

    private readonly Queue<PopupData> popupRequests = new Queue<PopupData>();
    private class PopupData
    {
        public string title;
        public string prompt;
        public float time;
        public InteractivePopup popup;
        public Func<bool, IEnumerator> onComplete;
    }

    private readonly Queue<InterruptData> interrupts = new Queue<InterruptData>();
    private readonly Queue<InterruptData> queuedCasts = new Queue<InterruptData>();
    private class InterruptData
    {
        public Spell spell;
        public Caster caster;
        public Battlefield.Position target;
        public string msgOverride;
        public bool canCounter;

        public InterruptData(Spell spell, Caster caster, Battlefield.Position target, string msgOverride, bool canCounter)
        {
            this.spell = spell;
            this.caster = caster;
            this.target = target;
            this.msgOverride = msgOverride;
            this.canCounter = canCounter;
        }
    }
}

/// <summary>
/// A class to store the results of the effects of an entire root
/// </summary>
public class RootCastResults : List<List<CastResults>>
{
    public List<CastResults> LastEffect => this[Count - 1];
}

public class RootCastData
{
    public RootCastData(Spell spell, List<RootWord> roots, int rootIndex)
    {
        Spell = spell;
        Roots = roots;
        RootIndex = rootIndex;
    }

    public Spell Spell { get; }
    public IReadOnlyList<RootWord> Roots { get; }
    public int RootIndex { get; }

    public bool IsLastRoot => RootIndex == Roots.Count - 1;
}
