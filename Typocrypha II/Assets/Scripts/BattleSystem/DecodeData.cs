using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "decodeData", menuName = "Decode Data")]
public class DecodeData : ScriptableObject
{
    [Header("Internal Data")]
    public string key;
    [Header("Gameplay Data")]
    public string obscuredWord;
    public SpellWord unlockedWord;
}
