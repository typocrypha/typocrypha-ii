using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WordList", menuName = "Word List")]
public class WordList : ScriptableObject
{
    public IReadOnlyList<string> Words => words;
    [SerializeField] private List<string> words;
}
