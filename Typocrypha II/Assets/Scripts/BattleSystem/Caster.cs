using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caster : FieldObject
{ 
    public enum State
    {
        None = -1,
        Player,
        Hostile,
        Neutral,
        Ally,
    }
    public enum BattleStatus
    {
        Alive,
        Dead,
        Fled,
        Banished,
        Hiding,
    }

    [SerializeField] private State _type;
    public State CasterState { get => _type; set => _type = value; }
   
    public int Health { get; set; }
    public int Armor { get; set; }
    public int SP { get; set; }
    public int Stagger { get; set; }
    public bool Stunned { get; } = false;
    public BattleStatus BStatus { get; }
    [SerializeField] private CasterTagDictionary _tags;
    public CasterTagDictionary Tags { get => _tags; set => _tags = value; }
    public CasterStats Stats { get => _tags.statMod; }

    private void Awake()
    {
        _tags.RecalculateStats();
        Health = Stats.MaxHP;
        Armor = Stats.MaxArmor;
        SP = Stats.MaxSP;
        Stagger = Stats.MaxStagger;
    }
}
