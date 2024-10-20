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
        for (int i = 0; i < numWords; i++)
        {
            var word = Instantiate(wordPrefab, caster.transform).GetComponent<WordRotator>();
            word.Play(wordList[i % wordList.Length]);
        }
    }
}
