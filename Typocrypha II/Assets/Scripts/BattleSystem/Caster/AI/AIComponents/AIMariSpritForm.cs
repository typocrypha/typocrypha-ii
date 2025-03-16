using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMariSpritForm : AIComponent
{
    [SerializeField] private GameObject wordPrefab;
    [SerializeField] private int numWords;
    [SerializeField] private AudioClip bgm;
    [SerializeField] private int[] wordCadence;
    private static readonly int[][] cadenceMapping = new int[][]
    {
        new int[] { 2 },
        new int[] { 2 },
        new int[] { 1, 3 },
        new int[] { 0, 2, 4 },
        new int[] { 0, 1, 3, 4 },
        new int[] { 0, 1, 2, 3, 4 },
    };
    [SerializeField] private GameObject standardVisuals;
    [SerializeField] private GameObject spiritFormVisuals;
    [SerializeField] private GameObject spiritFormBG;


    private void OnEnable()
    {
        caster.OnSpiritMode += OnSpiritForm;
    }

    private void OnDisable()
    {
        caster.OnSpiritMode -= OnSpiritForm;
    }

    private string[] wordList = new[]
    {
        "Revenge",
        "Birthright",
        "Obscurian",
        "Hurricane",
        "Howling",
        "Pain",
        "Sorrow",
        "Suffering",
        "Hatred",
        "Agartha",
        "Soldier",
        "Evil",
        "Eye",
        "Die",
        "Murder",
        "Unfair",
        "Avenge",
        "Ayin",
        "Obscurian",
        "Land",
        "Agartha",
        "Evil",
        "Eye",
        "Hurt",
        "Hurricane",
        "Torrential",
        "Protector",
        "Defender",
        "Guardian",
        "Storm",
        "Evil",
        "Eye",
        "Terror",
        "Dark",
        "Freezing",
        "Screaming",
        "Goodbye",
        "Resolve",
        "Determination",
        "Pouring",
        "Endless",
        "Always"
    };

    private void OnSpiritForm()
    {
        Rule.ActiveRule = null;
        Battlefield.instance.PH.Pause(PauseSources.Misc);
        Typocrypha.Keyboard.instance.PH.Pause(PauseSources.Misc);
        Typocrypha.Keyboard.instance.Clear();
        TargetReticle.instance.PH.Pause(PauseSources.Misc);
        caster.ui.gameObject.SetActive(false);
        AllyBattleBoxManager.instance.HideCharacter();
        SpellCooldownManager.instance.Hide();
        BackgroundManager.instance.SetBackground(spiritFormBG);
        // TODO form change sequence
        standardVisuals.SetActive(false);
        spiritFormVisuals.SetActive(true);
        // TODO: something better than this
        foreach(var enemy in Battlefield.instance.Enemies)
        {
            if(enemy == caster)
            {
                continue;
            }
            SpellFxManager.instance.PlayDamageNumber(999, enemy);
            enemy.Damage(999);
            enemy.gameObject.SetActive(false);
        }
        var wordDict = new Dictionary<string, List<WordRotator>>(numWords);
        for (int i = 0; i < numWords; i++)
        {
            var word = Instantiate(wordPrefab, caster.transform).GetComponent<WordRotator>();
            string text = wordList[i % wordList.Length];
            if (!wordDict.ContainsKey(text))
            {
                wordDict[text] = new List<WordRotator>() { word };
            }
            else
            {
                wordDict[text].Add(word);
            }
            word.Play(text);
        }
        StartCoroutine(SpiritFormCR(wordDict));
    }

    private BattleWord.FocusPosition[] focusPositions = new BattleWord.FocusPosition[]
    {
        BattleWord.FocusPosition.LeftFar,
        BattleWord.FocusPosition.LeftClose,
        BattleWord.FocusPosition.Center,
        BattleWord.FocusPosition.RightClose,
        BattleWord.FocusPosition.RightFar,
    };

    private IEnumerator SpiritFormCR(Dictionary<string, List<WordRotator>> words)
    {
        AudioManager.instance.StopBGM();
        yield return new WaitForSeconds(2f);
        AudioManager.instance.PlayBGM(bgm);
        int positionIndex = 1;
        int cadenceIndex = -1;
        var focusedWords = new List<List<WordRotator>>();
        var staggerYielder = new WaitForSeconds(0.25f);
        float delay = 0.15f;
        while (words.Count > 0)
        {
            // TODO: stop player from typing after death
            if (Battlefield.instance.Player.BStatus == Caster.BattleStatus.SpiritMode)
                yield break;
            if (++cadenceIndex >= wordCadence.Length)
            {
                cadenceIndex = 0;
            }

            int cadence = wordCadence[cadenceIndex];
            var mapping = cadenceMapping[cadence];
            focusedWords.Clear();
            float focusTime = 0;
            for (int i = 0; i < cadence; i++)
            {
                string text = RandomUtils.RandomU.instance.Choice(words.Keys);
                if (text == null)
                    break;
                var wordList = words[text];
                var word = wordList[0];
                BattleWord.FocusPosition focusPosition;
                if(cadence == 1 || i >= mapping.Length)
                {
                    if (++positionIndex >= focusPositions.Length)
                    {
                        positionIndex = -1;
                        focusPosition = RandomUtils.RandomU.instance.Choice(focusPositions);
                    }
                    else
                    {
                        focusPosition = focusPositions[positionIndex];
                    }
                }
                else
                {
                    focusPosition = focusPositions[mapping[i]];
                }
                focusTime += 1.25f + 0.25f * text.Length;
                word.Focus(focusTime, BattleWord.GetFocusPosition(focusPosition), i == 0, 5 - i);
                focusTime -= 0.25f;
                words.Remove(text);
                focusedWords.Add(wordList);
                yield return staggerYielder;
            }


            for (int i = 0; i < focusedWords.Count; i++)
            {
                var allWords = focusedWords[i];
                var word = allWords[0];
                if (i > 0)
                {
                    if (word.PendingFocus)
                    {
                        yield return new WaitWhile(() => word.PendingFocus);
                    }
                    word.SetTarget();
                }
                yield return new WaitWhile(() => word.isActiveAndEnabled);
                for (int j = 1; j < allWords.Count; j++)
                {
                    allWords[j].FadePending();
                }
            }
            if(delay > 0)
            {
                yield return new WaitForSeconds(delay + (float)(0.1 * RandomUtils.RandomU.instance.RandomDouble()));
            }
            delay -= 0.01f;
        }
        caster.Damage(9999);
        SpellFxManager.instance.PlayDamageNumber(999, caster);
        Battlefield.instance.PH.Unpause(PauseSources.Misc);
    }
}
