using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour, IPausable
{
    public static InputManager Instance { get; private set; }
    public PauseHandle PH { get; private set; }
    private void OnPause(bool pause)
    {
        foreach(var input in inputStack)
        {
            input.PH.SimpleParentPause(pause);
        }
        Typocrypha.Keyboard.instance.PH.SimpleParentPause(pause);
    }
    private bool blockCasting = false;
    public bool BlockCasting 
    { 
        get => blockCasting;
        set 
        {
            if(value)
            {
                if (!blockCasting)
                {
                    Typocrypha.Keyboard.instance.PH.SimpleParentPause(true);
                }
            }
            else if (!PH.Paused)
            {
                Typocrypha.Keyboard.instance.PH.SimpleParentPause(false);
            }
            blockCasting = value;
        }
    }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        PH = new PauseHandle(OnPause);
    }

    private readonly Stack<IInputHandler> inputStack = new Stack<IInputHandler>();

    public void StartInput(IInputHandler handler)
    {
        if (inputStack.Count > 0)
        {
            inputStack.Peek().Unfocus();
            Typocrypha.Keyboard.instance.PH.SimpleParentPause(false);
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
            inputStack.Peek().Focus();
            if(inputStack.Count == 1 && BlockCasting)
            {
                Typocrypha.Keyboard.instance.PH.SimpleParentPause(true);
            }
        }
    }

    public void Clear()
    {
        if (inputStack.Count <= 0)
        {
            return;
        }
        inputStack.Peek().Clear();
    }
}
