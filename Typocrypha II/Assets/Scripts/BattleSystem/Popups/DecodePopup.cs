using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using RandomUtils;
using System.Linq;
using System.Text;

public class DecodePopup : InteractivePopup
{
    private const char obscureChar = '?';
    private static readonly string highlightTag = "<color=red>"; 
    private static readonly string highlightCloseTag = "</color>"; 
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI promptText;
    private string realText = string.Empty;
    private StringBuilder obscuredText;
    private int currIndex;

    private void Update()
    {
        if (Completed || currIndex >= realText.Length)
            return;
        if (Input.GetKeyDown(realText[currIndex].ToString().ToLower()))
        {
            RevealCurrent();
        }
    }

    protected override void Setup(string header, string prompt, float time)
    {
        Completed = false;
        LastPromptSuccess = true;
        headerText.text = header;
        realText = prompt;
        currIndex = 0;
        obscuredText = ObscureWord(prompt);
        SetPrompt(obscuredText.ToString());
        ShowBubbles(realText[currIndex], RandomU.instance.RandomInt(1, 4));
        gameObject.SetActive(true);
    }

    private void ShowBubbles(char correctLetter, int num)
    {

    }

    private void RevealCurrent()
    {
        obscuredText[currIndex] = realText[currIndex];
        if(++currIndex >= realText.Length)
        {
            promptText.text = realText;
            Completed = true;
        }
        else
        {
            SetPrompt(obscuredText.ToString());
            ShowBubbles(realText[currIndex], RandomU.instance.RandomInt(1, 4));
        }
    }

    private StringBuilder ObscureWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return null;
        var builder = new StringBuilder(word);
        int num = RandomU.instance.RandomInt(0, word.Length - 1);
        var indices = Enumerable.Range(0, word.Length).ToList();
        for (int i = 0; i < num; ++i)
        {
            var index = RandomU.instance.Choice(indices);
            indices.Remove(index);
            builder[index] = obscureChar; 
        }
        return builder;
    }

    private void SetPrompt(string text)
    {
        var promptBuilder = new StringBuilder(text);
        promptBuilder.Insert(currIndex + 1, highlightCloseTag);
        promptBuilder.Insert(currIndex, highlightTag);
        promptText.text = promptBuilder.ToString();
    }
}
