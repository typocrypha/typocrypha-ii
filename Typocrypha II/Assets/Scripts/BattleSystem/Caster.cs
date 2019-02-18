using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caster : FieldObject
{ 
    public enum Type
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

    [SerializeField] private Type _type;
    public Type CasterType { get => _type; protected set => _type = value; }
   
    //CasterStats Stats { get; }
    public int Health { get; set; }
    public int Stagger { get; set; }
    public bool Stunned { get; }
    public BattleStatus BStatus { get; } 
    //CasterTagDictionary Tags { get; }
}
