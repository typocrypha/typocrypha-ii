using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FieldObject : MonoBehaviour
{
    [SerializeField] private string _displayName;
    public string DisplayName { get => _displayName; set => _displayName = value; }
    public Battlefield.Position FieldPos { get; set; } = new Battlefield.Position(0, 0);
    [SerializeField] private bool _moveable = true;
    public bool IsMoveable { get => _moveable; set => _moveable = value; }

    private ScouterData scouterData;
    public ScouterData ScouterData => scouterData ?? (scouterData = GetComponent<ScouterData>());
}
