﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ATB3;


/// Encapsulates battle info. Singleton that is availible from Battlefield.instance
public class Battlefield : MonoBehaviour, IPausable
{
    #region IPausable
    /// <summary>
    /// Pauses battle.
    /// </summary>
    PauseHandle ph;
    public PauseHandle PH { get => ph; }

    /// <summary>
    /// Pauses all actors and keyboard.
    /// </summary>
    /// <param name="b">Whether to pause or not.</param>
    public void OnPause(bool b)
    {
        Typocrypha.Keyboard.instance.PH.Pause = b;
        //SpellCooldownManager.instance.PH.Pause = b;
        if (b)
        {
            foreach (var actor in Actors)
                if (actor != null) actor.Pause = true;
        }
        else
        {
            if (!ATBManager.instance.InSolo)
            {
                foreach (var actor in Actors)
                    if (actor != null) actor.Pause = false;
            }
            else
            {
                ATBManager.instance.SoloActor.Pause = false;
            }
        }
    }
    #endregion

    public enum MoveOption
    {
        Move,
        Swap,
        SwapOnlyIfOtherSpaceIsOccupied,
        DeleteOther,
        DeleteOtherOnlyIfSpaceIsOccupied,
    }
    [System.Flags]
    public enum ClearOptions
    {
        Nothing = 0,
        ClearEnemies = 1,
        ClearObjects = 2,
        ClearAllies = 4,
    }
    public static Battlefield instance;
    public int Columns { get; } = 3;
    public int Rows { get; } = 2;
    public Caster Player 
    {
        get 
        {
            foreach(var caster in Casters)
            {
                if (caster.IsPlayer)
                    return caster;
            }
            return null;
        }

    }

    #region Row and List Accessor Properties
    public FieldObject[] TopRow { get { return field[0]; } }
    public FieldObject[] BottomRow { get { return field[1]; } }
    public IEnumerable<Caster> Enemies { get => Casters.Where((obj) => obj.CasterState == Caster.State.Hostile); }
    public IEnumerable<Caster> Allies { get => Casters.Where((obj) => obj.CasterState == Caster.State.Ally); }
    public List<ATBActor> Actors { get; } = new List<ATBActor>();
    public List<Caster> Casters { get; } = new List<Caster>();
    public IEnumerable<Position> AllPositions => Enumerable.Range(0, Rows).SelectMany((row) => Enumerable.Range(0, Columns).Select((col) => new Position(row, col)));
    public List<Position> ValidReinforcementPositions => AllPositions.Where(IsValidEnemyReinforcementPos).ToList();
    #endregion

    #region Data and Representative Lists
    private SpaceMatrix spaces;
    private FieldMatrix field;
    [SerializeField] private Transform illegalPosition;
    #endregion

    private Dictionary<string, Caster> proxyCasters = new Dictionary<string, Caster>(3);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        ph = new PauseHandle(OnPause);
        spaces = new SpaceMatrix(Rows, Columns);
        field = new FieldMatrix(Rows, Columns);
        var spaceTransforms = GetComponentsInChildren<Transform>();
        spaces[0, 0] = spaceTransforms[1];
        spaces[0, 1] = spaceTransforms[2];
        spaces[0, 2] = spaceTransforms[3];
        spaces[1, 0] = spaceTransforms[4];
        spaces[1, 1] = spaceTransforms[5];
        spaces[1, 2] = spaceTransforms[6];
    }

    #region Interface Functions
    /// <summary> Add am object to the battlefield at given field position
    /// Implicitly add toAdd to the actor list if applicable </summary> 
    public void Add(FieldObject toAdd, Position pos)
    {
        Remove(pos,true);
        toAdd.FieldPos = pos;
        toAdd.transform.position = spaces[pos].transform.position;
        field[pos] = toAdd;
        var actor = toAdd.GetComponent<ATBActor>();
        if (actor != null) Actors.Add(actor);
        var caster = toAdd.GetComponent<Caster>();
        if (caster != null) Casters.Add(caster);
    }
    public void AddProxyCaster(Caster caster)
    {
        string casterName = caster.DisplayName.ToLower();
        if (proxyCasters.ContainsKey(casterName))
            return;
        caster.FieldPos = new Position(-1, -1);
        caster.transform.position = GetSpace(caster.FieldPos);
        proxyCasters.Add(casterName, caster);

    }
    /// <summary> Get the position of a battlefield space. The space may be empty </summary> 
    public Vector2 GetSpace(Position pos)
    {
        return pos.IsLegal ? spaces[pos].position : illegalPosition.position;
    }
    public Vector2 GetSpaceScreenSpace(Position pos)
    {
        return CameraManager.instance.Camera.WorldToScreenPoint(GetSpace(pos));
    }
    /// <summary> Get the caster in a specific space. returns null if the space is empty or the object is not a caster </summary> 
    public Caster GetCaster(Position pos) => GetObject(pos) as Caster;
    public Caster GetCaster(string name, bool allowProxies)
    {
        var nameProcessed = name.ToLower();
        foreach(var caster in Casters)
        {
            if (caster.DisplayName.ToLower() == nameProcessed)
                return caster;
        }
        return allowProxies ? GetProxyCaster(name) : null;
    }
    public Caster GetProxyCaster(string proxyName)
    {
        proxyCasters.TryGetValue(proxyName.ToLower(), out Caster caster);
        return caster;
    }
    /// <summary> Get the field object </summary> 
    public FieldObject GetObject(Position pos) => field[pos];

    public void Remove(int row, int col, bool destroy = false)
    {
        Remove(new Position(row, col), destroy);
    }
    public void Remove(Position pos, bool destroy = false)
    {
        var obj = field[pos];
        if (obj == null)
            return;
        field[pos] = null;
        if(obj is Caster caster)
        {
            Casters.Remove(caster);
        }
        var actor = obj.GetComponent<ATBActor>();
        if (actor != null)
        {
            Actors.Remove(actor);
        }
        if (destroy)
        {
            Destroy(obj.gameObject);
        }
    }
    /// <summary> Clear according to options </summary>
    public void Clear(ClearOptions options)
    {
        if (options == ClearOptions.Nothing)
            return;
        int row = 0, col = 0;
        while (row < field.Rows)
        {
            var obj = field[row, col];
            if (obj != null)
            {
                if (obj is Caster caster)
                {
                    if(caster.CasterState == Caster.State.Hostile)
                    {
                        if (options.HasFlag(ClearOptions.ClearEnemies))
                        {
                            Remove(row, col, true);
                        }
                    }
                    else if (caster.CasterState == Caster.State.Ally)
                    {
                        if (options.HasFlag(ClearOptions.ClearAllies))
                        {
                            Remove(row, col, true);
                        }
                    }
                    else if (!caster.IsPlayer && options.HasFlag(ClearOptions.ClearObjects))
                    {
                        Remove(row, col, true);
                    }
                }
                else if (options.HasFlag(ClearOptions.ClearObjects))
                {
                    Remove(row, col, true);
                }
            }        
            if (++col >= field.Columns)
            {
                col = 0;
                ++row;
            }
        }
        RecalculateCasters();
        RecalculateActors();
    }
    /// Reset the caster array after clearing objects
    private void RecalculateCasters()
    {
        Casters.Clear();
        foreach(var obj in field)
        {
            if(obj is Caster caster)
            {
                Casters.Add(caster);
            }
        }
    }
    private void RecalculateActors()
    {
        Actors.Clear();
        foreach(var obj in field)
        {
            if (obj == null)
                continue;
            var actor = obj.GetComponent<ATBActor>();
            if(actor != null)
            {
                Actors.Add(actor);
            }
        }
    }
    #endregion

    /// <summary> Move a Field Object to another position. Doesn't move if the position was occupied </summary> 
    public bool Move(Position position, Position target, MoveOption option)
    {
        var self = field[position];
        var other = field[target];
        if (self == null || !self.IsMoveable)
        {
            Debug.LogWarning("Invalid Move: position was empty or object was not movable");
            return false;
        }          
        switch (option)
        {
            case MoveOption.Move:
                // There is an object in the target position!
                if (other != null)
                    return false;
                SetPosition(self, target);
                return true;
            case MoveOption.Swap:
                if (other != null && !other.IsMoveable)
                    return false;
                SetPosition(self, target);
                if(other != null)
                    SetPosition(other, position, false);
                return true;
            case MoveOption.SwapOnlyIfOtherSpaceIsOccupied:
                if (other == null || !other.IsMoveable)
                    return false;
                SetPosition(self, target);
                SetPosition(other, position, false);
                return true;
            case MoveOption.DeleteOther:
                Remove(other.FieldPos, true);
                SetPosition(self, target);
                return true;
            case MoveOption.DeleteOtherOnlyIfSpaceIsOccupied:
                if (other == null)
                    return false;
                Remove(other.FieldPos, true);
                SetPosition(self, target);
                return true;
            default:
                throw new System.Exception("Should not have reached default movement case");
        }
    }
    /// <summary> helper method for setting a FieldObject's position. Will overwrite other objects, and does not check for errors </summary>
    private void SetPosition(FieldObject obj, Position destination, bool clearOldPos = true)
    {
        if(clearOldPos)
            field[obj.FieldPos] = null;
        obj.FieldPos = destination;
        obj.transform.position = spaces[destination].position;        
        field[destination] = obj;      
    }

    public bool IsValidEnemyReinforcementPos(Position position)
    {
        return position.Row == 0 && IsEmpty(position);
    }

    public bool IsEmpty(Position position)
    {
        if (GetObject(position) == null)
            return true;
        var caster = GetCaster(position);
        return caster != null && caster.IsDeadOrFled;
    }

    private class FieldMatrix : Serializable2DMatrix<FieldObject>
    {
        public FieldMatrix(int rows, int columns) : base(rows, columns) { }
        public FieldObject this[Position pos]
        {
            get
            {
                return this[pos.Row, pos.Col];
            }
            set
            {
                this[pos.Row, pos.Col] = value;
            }
        }

    }
    private class SpaceMatrix : Serializable2DMatrix<Transform>
    {
        public SpaceMatrix(int rows, int columns) : base(rows, columns) { }
        public Transform this[Position pos]
        {
            get
            {
                return this[pos.Row, pos.Col];
            }
            set
            {
                this[pos.Row, pos.Col] = value;
            }
        }
    }
    [System.Serializable] public class Position : System.IEquatable<Position>
    {
        public bool IsLegal { get => _col >= 0 && _col < instance.Columns && _row >= 0 && _row < instance.Rows; }
        [SerializeField] int _row;
        public int Row { get => _row; set =>_row = value; }
        [SerializeField] int _col;
        public int Col { get => _col; set => _col = value; }
        public Position(int row, int col)
        {
            _row = row;
            _col = col;
        }

        public Position(Position toCopy)
        {
            _row = toCopy.Row;
            _col = toCopy.Col;
        }

        public Position(Vector2Int toCopy)
        {
            _row = toCopy.y;
            _col = toCopy.x;
        }

        public void SetIllegalPosition (int row, int col)
        {
            _row = row;
            _col = col;
        }
        public override string ToString()
        {
            return "(" + _row + ", " + _col + ")";
        }
        public bool Equals(Position other)
        {
            return Row == other.Row && Col == other.Col;
        }
        public override bool Equals(object other)
        {
            return other is Position ? Equals(other as Position) : false;
        }

        public static bool operator ==(Position a, Position b) => a.Equals(b);
        public static bool operator !=(Position a, Position b) => !(a == b);
        
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + _row.GetHashCode();
                hash = hash * 23 + _col.GetHashCode();
                return hash;
            }
        }
    }
}
