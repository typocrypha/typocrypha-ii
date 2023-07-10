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
    private const int maxBubbleLetters = 6;
    private static readonly char[] allLetters = "qwertyuiopasdfghjklzxcvbn".ToArray();
    public TextMeshProUGUI headerText;
    private string realText = string.Empty;
    private int obscureIndex;

    public override bool? CheckInput(char inputChar)
    {
        if (Completed || pos >= realText.Length)
            return null;
        if (char.ToLower(inputChar) == char.ToLower(realText[pos]))
        {
            SetLetter(pos, realText[pos].ToString());
            if (++pos >= realText.Length)
            {
                Completed = true;
                InputManager.Instance.CompleteInput();
            }
            else
            {
                ResetBubbles();
                UpdateCursor(true);
            }
            return true;
        }
        return false;
    }

    public override void Submit()
    {
        return;
    }

    protected override void Setup(string header, string dataKey, float time)
    {
        Completed = false;
        LastPromptSuccess = true;
        headerText.text = header;
        var data = PlayerDataManager.instance.researchData.GetData(dataKey);
        realText = data.unlockedWord.internalName;
        Prompt = ObscureWord(data).ToString();
        Clear(false);
        obscureIndex = 0;
        ResetBubbles();
        gameObject.SetActive(true);
        InputManager.Instance.StartInput(this);
    }

    private void ResetBubbles()
    {
        CleanupBubbles();
        if (Prompt[pos] == obscureChar)
        {
            ShowBubbles(char.ToLower(realText[pos]), GetNumBubbles(++obscureIndex));
        }
    }

    private int GetNumBubbles(int index)
    {
        if (index < 2)
            return 2;
        if (index < 4)
            return 3;
        return 4;
    }

    public void CleanupBubbles(GameObject except = null)
    {
        foreach (Transform bubble in bubbleContainer)
        {
            if(bubble.gameObject != except)
            {
                Destroy(bubble.gameObject);
            }
        }
    }

    private void ShowBubbles(char correctLetter, int num)
    {
        int correctBubble = RandomU.instance.RandomInt(0, num);
        var availibleLetters = new List<char>(allLetters);
        availibleLetters.Remove(correctLetter);
        var bubbleLetters = new List<char>(maxBubbleLetters);
        for (int i = 0; i < num; ++i)
        {
            int numLetters = Mathf.Min(availibleLetters.Count, RandomU.instance.RandomInt(3, 6));
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
            bubble?.Show(bubbleLetters, correctLetter, this);
        }
    }

    private StringBuilder ObscureWord(DecodeData data)
    {
        string word = data.unlockedWord.internalName;
        string obscuredWord = data.obscuredWord;
        if (string.IsNullOrWhiteSpace(word))
            return null;
        var builder = new StringBuilder(word);
        // If the obscure word isn't specified, randomly generate
        if (string.IsNullOrWhiteSpace(obscuredWord))
        {
            int num = RandomU.instance.RandomInt(0, word.Length - 1);
            var indices = Enumerable.Range(0, word.Length).ToList();
            for (int i = 0; i < num; ++i)
            {
                var index = RandomU.instance.Choice(indices);
                indices.Remove(index);
                builder[index] = obscureChar;
            }
        }
        else // use the obscured word format
        {
            for (int i = 0; i < word.Length; ++i)
            {
                if(obscuredWord[i] == obscureChar)
                {
                    builder[i] = obscureChar;
                }
            }
        }
        return builder;
    }
}
