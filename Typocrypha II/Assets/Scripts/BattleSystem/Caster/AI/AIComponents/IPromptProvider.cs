using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPromptProvider
{
    string Title(int index);
    string Prompt(int index);
    float Time(int index);
}
