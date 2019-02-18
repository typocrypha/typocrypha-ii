using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// Encapsulates battle info. Singleton that is availible from Battlefield.instance
public class Battlefield : MonoBehaviour
{
    public enum MoveOption
    {
        Move,
        Swap,
        SwapOnlyIfOtherSpaceIsOccupied,
        DeleteOther,
        DeleteOtherOnlyIfSpaceIsOccupied,
    }
    public int Columns { get; } = 3;
    public int Rows { get; } = 2;

    public static Battlefield instance;
    public Player Player { get => (Casters.FirstOrDefault((obj) => obj.GetComponent<Player>() != null))?.GetComponent<Player>(); }

    //DEBUG FOR TESTING ATB TWEAKS
    public Actor[] actorsToAdd;
    public FieldObject[] objectsToAdd;

    #region Row and List Accessor Properties
    public FieldObject[] TopRow { get { return field[0]; } }
    public FieldObject[] BottomRow { get { return field[1]; } }
    public Caster[] Enemies { get => Casters.Select((obj) => obj.GetComponent<Caster>()).Where((obj) => obj != null && obj.CasterType == Caster.Type.Enemy).ToArray(); }
    public Caster[] Allies { get => Casters.Select((obj) => obj.GetComponent<Caster>()).Where((obj) => obj != null && obj.CasterType == Caster.Type.Ally).ToArray(); }
    public List<Actor> Actors { get; } = new List<Actor>();
    public List<Caster> Casters { get; } = new List<Caster>();
    #endregion

    #region Data and Representative Lists
    private SpaceMatrix spaces;
    private FieldMatrix field;
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    //Initialize space objects from children
    private void Start()
    {
        spaces = new SpaceMatrix(Rows, Columns);
        field = new FieldMatrix(Rows, Columns);
        var spaceTransforms = GetComponentsInChildren<Transform>();
        spaces[0, 0] = spaceTransforms[1];
        spaces[0, 1] = spaceTransforms[2];
        spaces[0, 2] = spaceTransforms[3];
        spaces[1, 0] = spaceTransforms[4];
        spaces[1, 1] = spaceTransforms[5];
        spaces[1, 2] = spaceTransforms[6];
        //Actors.AddRange(actorsToAdd);
        for (int i = 0; i < objectsToAdd.Length; ++i)
            Add(objectsToAdd[i], new Position(i / 3, i % 3));
    }

    #region Interface Functions
    /// <summary> Add a caster to the battlefield at given field position
    /// Implicitly add toAdd to the actor list if applicable </summary> 
    public void Add(FieldObject toAdd, Position pos)
    {
        toAdd.FieldPos = pos;
        toAdd.transform.position = spaces[pos].transform.position;
        field[pos] = toAdd;
        var actor = toAdd.GetComponent<Actor>();
        if (toAdd != null) Actors.Add(actor);
    }
    /// <summary> Add a actor that is not necessarily a FieldObject and is not in a field position </summary> 
    public void AddActor(Actor a)
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
    /// <summary> Clear the data and representative lists </summary> 
    public void Clear()
    {
        field = new FieldMatrix(2, 3);
        Actors.Clear();
        Casters.Clear();
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
                SetPosition(self, target);
                return true;
            case MoveOption.DeleteOtherOnlyIfSpaceIsOccupied:
                if (other == null)
                    return false;
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
        public int Row { get => _row; set =>_row = Mathf.Clamp(0, _row, instance.Rows); }
        [SerializeField] int _col;
        public int Col { get => _col; set => _col = Mathf.Clamp(0, _col, instance.Columns); }
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
