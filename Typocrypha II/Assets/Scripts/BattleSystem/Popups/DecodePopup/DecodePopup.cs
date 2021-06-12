using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using RandomUtils;
using System.Linq;
using System.Text;

public class DecodePopup : InteractivePopup
{
    [SerializeField] private GameObject bubblePrefab; 
    [SerializeField] private Transform bubbleContainer; 
    private const char obscureChar = '?';
    private static readonly string highlightTag = "<color=red>"; 
    private static readonly string highlightCloseTag = "</color>"; 
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI promptText;
    private string realText = string.Empty;
    private StringBuilder obscuredText;
    private int currIndex;
    private static readonly char[] letters = "qwertyuiopasdfghjklzxcvbn".ToArray();
    private const int maxBubbleLetters = 6;

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
        ResetBubbles();
        gameObject.SetActive(true);
    }

    private void ResetBubbles()
    {
        CleanupBubbles();
        if (obscuredText[currIndex] == obscureChar)
        {
            ShowBubbles(char.ToLower(realText[currIndex]), RandomU.instance.RandomInt(1, 4));
        }
    }

    private void CleanupBubbles()
    {
        foreach (Transform bubble in bubbleContainer)
        {
            Destroy(bubble.gameObject);
        }
    }

    private void ShowBubbles(char correctLetter, int num)
    {
        int correctBubble = RandomU.instance.RandomInt(0, num);
        var availibleLetters = new List<char>(letters);
        availibleLetters.Remove(correctLetter);
        var bubbleLetters = new List<char>(maxBubbleLetters);
        for (int i = 0; i < num; ++i)
        {
            int numLetters = Mathf.Min(availibleLetters.Count, RandomU.instance.RandomInt(2, 6));
            bubbleLetters.Clear();
            if(i == correctBubble)
            {
                numLetters--;
            }
            while(numLetters-- > 0)
            {
                char letter = RandomU.instance.Choice(availibleLetters);
                bubbleLetters.Add(letter);
                availibleLetters.Remove(letter);
            }
            if(i == correctBubble)
            {
                bubbleLetters.Insert(RandomU.instance.RandomInt(0, bubbleLetters.Count), correctLetter);
            }
            var bubble = Instantiate(bubblePrefab, bubbleContainer).GetComponent<DecodeBubble>();
            bubble?.Show(bubbleLetters, correctLetter);
        }
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
            ResetBubbles();
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
