using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInputPromptEffect : BaseShowInputPromptEffect
{
    [SerializeField] private string title;
    [SerializeField] private string prompt;
    [SerializeField] private float time;

    protected override string Title => title;

    protected override string Prompt => prompt;

    protected override float Time => time;
}
