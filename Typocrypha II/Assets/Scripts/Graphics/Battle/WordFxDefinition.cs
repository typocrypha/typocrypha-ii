using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WordFxDefinition", menuName = "WordFxDefinition")]
public class WordFxDefinition : ScriptableObject
{
    public enum AnimType
    {
        Default,
        None,
    }

    public AnimType AnimationType => animType;
    [SerializeField] AnimType animType;
}
