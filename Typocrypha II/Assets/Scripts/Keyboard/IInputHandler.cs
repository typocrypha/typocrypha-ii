using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputHandler : IPausable
{
    void Focus();
    void Unfocus();

    // Return null for no sfx, false for no input (fail) sfx, and true for key sfx
    bool? CheckInput(char inputChar);

    void Submit();
}
