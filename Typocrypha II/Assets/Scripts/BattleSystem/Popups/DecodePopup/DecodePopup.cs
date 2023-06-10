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
    private static readonly char[] letters = "qwertyuiopasdfghjklzxcvbn".ToArray();
    private static readonly string highlightTag = "<color=red>"; 
    private static readonly string highlightCloseTag = "</color>"; 
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI promptText;
    private string realText = string.Empty;
    private StringBuilder obscuredText;
    private int currIndex;
    private int obscureIndex;
    private bool focus;


    public override void Focus()
    {
        focus = true;
    }

    public override void Unfocus()
    {
        focus = false;
    }

    private void Update()
    {
        if (!focus || Completed || currIndex >= realText.Length)
            return;
        if (Input.GetKeyDown(realText[currIndex].ToString().ToLower()))
        {
            RevealCurrent();
        }
    }

    protected override void Setup(string header, string dataKey, float time)
    {
        Completed = false;
        LastPromptSuccess = true;
        headerText.text = header;
        var data = PlayerDataManager.instance.researchData.GetData(dataKey);
        realText = data.unlockedWord.internalName;
        currIndex = 0;
        obscureIndex = 0;
        obscuredText = ObscureWord(data);
        SetPrompt(obscuredText.ToString());
        ResetBubbles();
        gameObject.SetActive(true);
        InputManager.Instance.StartInput(this);
    }

    private void ResetBubbles()
    {
        CleanupBubbles();
        if (obscuredText[currIndex] == obscureChar)
        {
            ShowBubbles(char.ToLower(realText[currIndex]), GetNumBubbles(++obscureIndex));
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
        var availibleLetters = new List<char>(letters);
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

    private void RevealCurrent()
    {
        obscuredText[currIndex] = realText[currIndex];
        if(++currIndex >= realText.Length)
        {
            promptText.text = realText;
            Completed = true;
            InputManager.Instance.CompleteInput();
        }
        else
        {
            SetPrompt(obscuredText.ToString());
            ResetBubbles();
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

    private void SetPrompt(string text)
    {
        var promptBuilder = new StringBuilder(text);
        promptBuilder.Insert(currIndex + 1, highlightCloseTag);
        promptBuilder.Insert(currIndex, highlightTag);
        promptText.text = promptBuilder.ToString();
    }
}
