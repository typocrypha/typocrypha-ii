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
    PauseHandle ph;
    public PauseHandle PH { get => ph; }

    /// <summary>
    /// Pauses all actors. (May pause other things: to be implemented).
    /// </summary>
    /// <param name="b">Whether to pause or not.</param>
    public void OnPause(bool b)
    {
        Typocrypha.Keyboard.instance.PH.Pause = b;
        SpellCooldownManager.instance.PH.Pause = b;
        if (b)
        {
            foreach (var actor in Actors)
                if (actor != null) actor.pause = true;
        }
        else
        {
            if (ATBManager.soloStack.Count == 0)
            {
                foreach (var actor in Actors)
                    if (actor != null) actor.pause = false;
            }
            else
            {
                ATBManager.soloStack.Peek().pause = false;
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
    public Player Player { get => (Casters.FirstOrDefault((obj) => obj.GetComponent<Player>() != null))?.GetComponent<Player>(); }

    #region Debug Fields
    public ATBActor[] actorsToAdd;
    public FieldObject[] objectsToAdd;
    #endregion

    #region Row and List Accessor Properties
    public FieldObject[] TopRow { get { return field[0]; } }
    public FieldObject[] BottomRow { get { return field[1]; } }
    public Caster[] Enemies { get => Casters.Select((obj) => obj.GetComponent<Caster>()).Where((obj) => obj != null && obj.CasterState == Caster.State.Hostile).ToArray(); }
    public Caster[] Allies { get => Casters.Select((obj) => obj.GetComponent<Caster>()).Where((obj) => obj != null && obj.CasterState == Caster.State.Ally).ToArray(); }
    public List<ATBActor> Actors { get; } = new List<ATBActor>();
    public List<Caster> ExternalCasters { get => Casters.Where((obj) => !obj.FieldPos.IsLegal) as List<Caster>; }
    public List<Caster> Casters { get; } = new List<Caster>();
    #endregion

    #region Data and Representative Lists
    private SpaceMatrix spaces;
    private FieldMatrix field;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
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

    //Initialize space objects from children
    private void Start()
    {
        //spaces = new SpaceMatrix(Rows, Columns);
        //field = new FieldMatrix(Rows, Columns);
        //var spaceTransforms = GetComponentsInChildren<Transform>();
        //spaces[0, 0] = spaceTransforms[1];
        //spaces[0, 1] = spaceTransforms[2];
        //spaces[0, 2] = spaceTransforms[3];
        //spaces[1, 0] = spaceTransforms[4];
        //spaces[1, 1] = spaceTransforms[5];
        //spaces[1, 2] = spaceTransforms[6];
        Actors.AddRange(actorsToAdd);
        for (int i = 0; i < objectsToAdd.Length; ++i)
        {
            if (objectsToAdd[i] != null)
                Add(objectsToAdd[i], new Position(i / 3, i % 3));
        }
            
    }

    #region Interface Functions
    /// <summary> Add an object to the battlefield at given field position
    /// Works differently for allies and enemies
    /// Implicitly add toAdd to the actor list if applicable </summary> 
    public void SpecialAdd(FieldObject toAdd, Position pos)
    {
        var obj = field[pos];
        if(obj == null)
        {
            Add(toAdd, pos);
        }
        else if(toAdd is Caster)
        {
            var addCaster = toAdd as Caster;
            if(addCaster.CasterClass == Caster.Class.Other)
            {

            }
            //if(toAdd.c)
        }
        else
        {

        }
    }
    /// <summary> Add am object to the battlefield at given field position
    /// Implicitly add toAdd to the actor list if applicable </summary> 
    public void Add(FieldObject toAdd, Position pos)
    {
        toAdd.FieldPos = pos;
        toAdd.transform.position = spaces[pos].transform.position;
        field[pos] = toAdd;
        var actor = toAdd.GetComponent<ATBActor>();
        if (actor != null) Actors.Add(actor);
        var caster = toAdd.GetComponent<Caster>();
        if (caster != null) Casters.Add(caster);
    }
    /// <summary> Add a actor that is not necessarily a FieldObject and is not in a field position </summary> 
    public void AddActor(ATBActor a)
    {
        Actors.Add(a);
    }
    /// <summary> Add a caster in an illegal field position </summary>
    public void AddExternalCaster(Caster c, Position pos)
    {
        if(pos.IsLegal)
        {
            Debug.LogWarning("Attempting to put an external caster in a legal position");
            return;
        }
        c.FieldPos = pos;
        Casters.Add(c);
    }
    /// <summary> Get the position of a battlefield space. The space may be empty </summary> 
    public Vector2 GetSpace(Position pos)
    {
        return spaces[pos].position;
    }
    /// <summary> Get the caster in a specific space. returns null if the space is empty or the object is not a caster </summary> 
    public Caster GetCaster(Position pos) => GetObject(pos) as Caster;
    /// <summary> Get the field object </summary> 
    public FieldObject GetObject(Position pos) => field[pos];
    /// <summary> Destroy an object on the field </summary>
    public void Destroy(Position pos)
    {
        var obj = field[pos];
        if (obj != null)
            Destroy(obj);
    }
    /// <summary> Clear the data and destroy all of the game objects </summary>
    public void DestroyAllAndClear()
    {
        foreach (var obj in field)
            if(obj != null)
                Destroy(obj.gameObject);
        foreach (var obj in Actors)
            if (obj != null)
                Destroy(obj.gameObject);
        foreach (var obj in Casters)
            if (obj != null)
                Destroy(obj.gameObject);
        Clear();
    }
    /// <summary> Clear the data and representative lists </summary> 
    public void Clear()
    {
        field = new FieldMatrix(2, 3);
        Actors.Clear();
        Casters.Clear();
    }
    /// <summary> Clear according to options </summary>
    public void ClearAndDestroy(ClearOptions options)
    {
        if (options == ClearOptions.Nothing)
            return;
        int row = 0, col = 0;
        while (row < field.Rows)
        {
            var obj = field[row, col];
            if (obj == null)
            {
                if (++col >= field.Columns)
                {
                    col = 0;
                    ++row;
                }
                continue;
            }

            var caster = obj as Caster;
            if (caster != null)
            {
                if ((caster.CasterClass == Caster.Class.Other && options.HasFlag(ClearOptions.ClearEnemies))
                    || (caster.CasterClass == Caster.Class.PartyMember && options.HasFlag(ClearOptions.ClearAllies)))
                {
                    Destroy(obj.gameObject);
                    field[row, col] = null;
                }
            }
            else if (options.HasFlag(ClearOptions.ClearObjects))
            {
                Destroy(obj.gameObject);
                field[row, col] = null;
            }    
            
            if (++col >= field.Columns)
            {
                col = 0;
                ++row;
            }
        }
        RecalculateCasters();
    }
    /// Reset the caster array after clearing objects
    private void RecalculateCasters()
    {
        Casters.Clear();
        foreach(var obj in field)
        {
            var caster = obj?.GetComponent<Caster>();
            if (caster != null)
                Casters.Add(caster);
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
                Destroy(other.FieldPos);
                SetPosition(self, target);
                return true;
            case MoveOption.DeleteOtherOnlyIfSpaceIsOccupied:
                if (other == null)
                    return false;
                Destroy(other.FieldPos);
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
