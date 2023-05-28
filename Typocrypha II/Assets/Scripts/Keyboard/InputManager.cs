using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
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
        handler.Focus();
        inputStack.Push(handler);
    }

    public void CompleteInput()
    {
        var handler = inputStack.Pop();
        handler.Unfocus();
        if(inputStack.Count > 0)
        {
            inputStack.Peek().Focus();
        }
    }
}
