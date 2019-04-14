﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caster : FieldObject
{
    public enum Class
    {
        None = -1,
        Other,
        Player,
        PartyMember,
    }

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
    [SerializeField] private Class _casterClass;
    public Class CasterClass { get => _casterClass; set => _casterClass = value; }

    int health;
    public int Health
    {
        get => health;
        set
        {
            health = value;
            ui?.onHealthChanged.Invoke((float)health/Stats.MaxHP);
        }
    }
    public int Armor { get; set; }
    public int SP { get; set; }
    int stagger;
    public int Stagger
    {
        get => stagger;
        set
        {
            stagger = value;
            ui?.onStaggerChanged.Invoke(stagger.ToString());
        }
    }
    public bool Stunned { get; } = false;
    string currSpell;
    public string CurrSpell
    {
        get => currSpell;
        set
        {
            currSpell = value;
            ui?.onSpellChanged.Invoke(currSpell);
        }
    }
    public BattleStatus BStatus { get; }
    [SerializeField] private CasterTagDictionary _tags;
    public CasterTagDictionary Tags { get => _tags; set => _tags = value; }
    public CasterStats Stats { get => _tags.statMod; }
    public EnemyUI ui;

    private void Awake()
    {
        _tags.RecalculateStats();
        Health = Stats.MaxHP;
        Armor = Stats.MaxArmor;
        SP = Stats.MaxSP;
        Stagger = Stats.MaxStagger;
    }
}
