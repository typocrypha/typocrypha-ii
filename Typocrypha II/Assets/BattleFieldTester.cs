using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFieldTester : MonoBehaviour
{
    public Battlefield.Position posFrom;
    public Battlefield.Position posTo;
    public Battlefield.MoveOption option;
    public void Move()
    {
        Battlefield.instance.Move(posFrom, posTo, option);
    }
}
