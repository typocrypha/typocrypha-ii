using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMariSpritForm : AIComponent
{
    [SerializeField] private GameObject wordPrefab;
    [SerializeField] private int numWords;
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
        "I miss her",
        "I don't want",
        "To die",
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
        var words = new List<WordRotator>(numWords);
        AllyBattleBoxManager.instance.HideCharacter();
        SpellCooldownManager.instance.Hide();
        foreach(var enemy in Battlefield.instance.Enemies)
        {
            if(enemy == caster)
            {
                continue;
            }
            enemy.Damage(999);
            enemy.gameObject.SetActive(false);
        }
        for (int i = 0; i < numWords; i++)
        {
            var word = Instantiate(wordPrefab, caster.transform).GetComponent<WordRotator>();
            word.Play(wordList[i % wordList.Length]);
            words.Add(word);
        }
        StartCoroutine(SpiritFormCR(words));
    }

    private IEnumerator SpiritFormCR(IList<WordRotator> words)
    {
        yield return new WaitForSeconds(2f);
        while (words.Count > 0)
        {
            if (Battlefield.instance.Player.BStatus == Caster.BattleStatus.SpiritMode)
                yield break;
            var word = RandomUtils.RandomU.instance.Choice(words, out int index);
            word.FocusPending(0.4f);
            yield return new WaitWhile(() => word.isActiveAndEnabled);
            words.RemoveAt(index);
        }
        caster.Damage(9999);
        Battlefield.instance.PH.Unpause(PauseSources.Misc);
    }
}
