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
        "Save Me",
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
    };

    private void OnSpiritForm()
    {
        Battlefield.instance.PH.Pause(PauseSources.Misc);
        Typocrypha.Keyboard.instance.PH.Pause(PauseSources.Misc);
        TargetReticle.instance.PH.Pause(PauseSources.Misc);
        caster.ui.gameObject.SetActive(false);
        AllyBattleBoxManager.instance.HideCharacter();
        SpellCooldownManager.instance.Hide();
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

    private Vector2[] focusPositions = new Vector2[]
    {
        new Vector2(-4, 3f),
        new Vector2(-2.5f, 2),
        new Vector2(0, 1f),
        new Vector2(2.5f, 2),
        new Vector2(4, 3f),
    };

    private IEnumerator SpiritFormCR(Dictionary<string, List<WordRotator>> words)
    {
        AudioManager.instance.StopBGM();
        yield return new WaitForSeconds(2f);
        AudioManager.instance.PlayBGM(bgm);
        int positionIndex = 1;
        int cadenceIndex = -1;
        var focusedWords = new List<List<WordRotator>>();
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
            for (int i = 0; i < cadence; i++)
            {
                string text = RandomUtils.RandomU.instance.Choice(words.Keys);
                var wordList = words[text];
                var word = wordList[0];
                Vector2 focusPosition;
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
                word.FocusPending(0.4f, focusPosition);
                words.Remove(text);
                focusedWords.Add(wordList);
            }

            for (int i = 0; i < focusedWords.Count; i++)
            {
                var allWords = focusedWords[i];
                var word = allWords[0];
                word.DoFocused();
                yield return new WaitWhile(() => word.isActiveAndEnabled);
                for (int j = 1; j < allWords.Count; j++)
                {
                    allWords[j].FadePending();
                }
            }
        }
        caster.Damage(9999);
        SpellFxManager.instance.PlayDamageNumber(999, caster);
        Battlefield.instance.PH.Unpause(PauseSources.Misc);
    }
}
