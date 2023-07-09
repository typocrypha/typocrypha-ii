using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour, IPausable
{
    public static InputManager Instance { get; private set; }
    public PauseHandle PH => inputStack.Count > 0 ? inputStack.Peek().PH : null;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private readonly Stack<IInputHandler> inputStack = new Stack<IInputHandler>();

    public void StartInput(IInputHandler handler)
    {
        if (inputStack.Count > 0)
        {
            inputStack.Peek().Unfocus();
        }
        StopAllCoroutines();
        handler.Focus();
        inputStack.Push(handler);
    }

    // Return null for no sfx, false for no input (fail) sfx, and true for key sfx
    public bool? CheckInput(char inputChar)
    {
        if(inputStack.Count > 0)
        {
            return inputStack.Peek().CheckInput(inputChar);
        }
        return false;
    }

    // Return null for no sfx, false for no input (fail) sfx, and true for key sfx
    public bool? CheckInput(IEnumerable<char> input)
    {
        bool? playKeySfx = null;
        foreach (char inputChar in input)
        {
            var latestInput = CheckInput(inputChar);
            if (latestInput.HasValue)
            {
                if (playKeySfx.HasValue)
                {
                    playKeySfx = playKeySfx.Value || latestInput.Value;
                }
                else
                {
                    playKeySfx = latestInput;
                }
            }
        }
        return playKeySfx;
    }

    public void Submit()
    {
        if (inputStack.Count > 0)
        {
            inputStack.Peek().Submit();
        }
    }

    public void CompleteInput()
    {
        if(inputStack.Count <= 0)
        {
            Debug.LogError("Attempting to complete an empty input stack!");
            return;
        }
        var handler = inputStack.Pop();
        handler.Unfocus();
        if(inputStack.Count > 0)
        {
            //inputStack.Peek().Focus();
            StartCoroutine(FocusAtEndOfFrame());
        }
    }

    private IEnumerator FocusAtEndOfFrame()
    {
        yield return null;
        inputStack.Peek().Focus();
    }
}
