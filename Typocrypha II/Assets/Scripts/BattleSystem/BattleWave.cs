using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class containing the data for a wave made to be passed to the battle manager
/// May make a child class for waves with reinforcements later (or just implement through battle events?)
/// </summary>
public class BattleWave
{
    public Battlefield.ClearOptions fieldOptions;
    public string waveTitle;
    public AudioClip music;
    public GOMatrix2D battleField;
    public List<GameObject> battleEvents;
    public List<GameObject> reinforcementPrefabs;
}
