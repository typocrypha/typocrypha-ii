using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldObject : MonoBehaviour
{
    [SerializeField] private string _displayName;
    public string DisplayName { get => _displayName; protected set => _displayName = value; }
    private Battlefield.Position _fieldPos = new Battlefield.Position(0,0);
    public Battlefield.Position FieldPos { get => _fieldPos; set => _fieldPos = value; }
    [SerializeField] private bool _moveable = true;
    public bool IsMoveable { get => _moveable; protected set => _moveable = value; }
}
