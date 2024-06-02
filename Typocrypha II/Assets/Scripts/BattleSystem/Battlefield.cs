using System.Collections;
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
    public PauseHandle PH { get; private set; }

    /// <summary>
    /// Pauses all actors and keyboard.
    /// </summary>
    /// <param name="b">Whether to pause or not.</param>
    public void OnPause(bool b)
    {
        //Typocrypha.Keyboard.instance.CastingEnabled = !b;
        //SpellCooldownManager.instance.PH.Pause = b
        if (b)
        {
            foreach (var actor in Actors)
            {
                if (actor != null)
                {
                    actor.PH.Pause(PauseSources.Parent);
                }
            }
        }
        else
        {
            if (!ATBManager.instance.ProcessingActions)
            {
                foreach (var actor in Actors)
                {
                    if (actor != null)
                    {
                        actor.PH.Unpause(PauseSources.Parent);
                    }
                }
            }
            else
            {
                ATBManager.instance.SoloActor.PH.Unpause(PauseSources.Parent);
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
    public Player Player
    {
        get
        {
            foreach (var caster in Casters)
            {
                if (caster is Player player)
                    return player;
            }
            return null;
        }

    }

    public List<Caster> Enemies => Casters.Where(c=>!c.IsPlayer).ToList();

    #region List Accessor Properties
    public List<ATBActor> Actors { get; } = new List<ATBActor>(6);
    public List<Caster> Casters { get; } = new List<Caster>(6);
    public List<Position> ValidReinforcementPositions
    {
        get
        {
            var ret = new List<Position>(3);
            for (int col = 0; col < field.Columns; col++)
            {
                var pos = new Position(0, col);
                if (IsEmpty(pos))
                {
                    ret.Add(pos);
                }
            }
            return ret;
        }
    }
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
        PH = new PauseHandle(OnPause);
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
    public void Add(Caster caster, Position pos)
    {
        Remove(pos,true);
        caster.FieldPos = pos;
        caster.transform.position = spaces[pos].transform.position;
        field[pos] = caster;
        var actor = caster.GetComponent<ATBActor>();
        if (actor != null)
        {
            Actors.Add(actor);
            if (PH.Paused)
            {
                actor.PH.Pause(PauseSources.Parent);
            }
        }
        Casters.Add(caster);
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
    public Caster GetCaster(Position pos) => field[pos];
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

    public void Remove(int row, int col, bool destroy = false)
    {
        Remove(new Position(row, col), destroy);
    }
    public void Remove(Position pos, bool destroy = false)
    {
        var caster = field[pos];
        if (caster == null)
            return;
        field[pos] = null;
        Casters.Remove(caster);
        var actor = caster.GetComponent<ATBActor>();
        if (actor != null)
        {
            Actors.Remove(actor);
            actor.PH.FreeFromParent();
        }
        if (destroy)
        {
            Destroy(caster.gameObject);
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
            var caster = field[row, col];
            if (caster != null)
            {
                if (caster.CasterState == Caster.State.Hostile)
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
        foreach(var caster in field)
        {
            if(caster != null)
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
    /// <summary> helper method for setting a casters's position. Will overwrite other casters, and does not check for errors </summary>
    private void SetPosition(Caster caster, Position destination, bool clearOldPos = true)
    {
        if(clearOldPos)
            field[caster.FieldPos] = null;
        caster.FieldPos = destination;
        caster.transform.position = spaces[destination].position;        
        field[destination] = caster;      
    }

    public bool IsEmpty(Position position)
    {
        var caster = GetCaster(position);
        return caster == null || caster.IsDeadOrFled;
    }

    public bool IsOccupied(Position position) => !IsEmpty(position);

    private class FieldMatrix : Serializable2DMatrix<Caster>
    {
        public FieldMatrix(int rows, int columns) : base(rows, columns) { }
        public Caster this[Position pos]
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
        public int Row { get => _row; set => _row = value; }
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

        public void SetIllegalPosition(int row, int col)
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
            return other is Position pos && Equals(pos);
        }

        public static bool operator ==(Position a, Position b) 
        {
            if (a is null || b is null)
            {
                return a is null && b is null;
            }
            return a.Equals(b);
        }

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
