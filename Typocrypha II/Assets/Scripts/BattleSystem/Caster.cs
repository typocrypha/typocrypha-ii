using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caster : FieldObject
{ 
    public enum State
    {
        None = -1,
        Player,
        Enemy,
        Neuteral,
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

    [SerializeField] private CasterStats _stats;
    public CasterStats Stats { get => _stats; set => _stats = value; }
    public int Health { get; set; }
    public int Stagger { get; set; }
    public bool Stunned { get; }
    public BattleStatus BStatus { get; }
    [SerializeField] private CasterTagDictionary _tags;
    public CasterTagDictionary Tags { get => _tags; set => _tags = value; }
}
