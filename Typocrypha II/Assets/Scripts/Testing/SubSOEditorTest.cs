using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Testing/SubSOtest")]
public class SubSOEditorTest : ScriptableObject
{
    [SubSO("Formula")]
    public CustomFormula formula;
    [SubSO("Formula2")]
    public CustomFormula formula2;
}
