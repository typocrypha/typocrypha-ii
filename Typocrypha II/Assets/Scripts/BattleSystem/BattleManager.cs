using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleGraphParser))]
public class BattleManager : MonoBehaviour
{
    public static BattleManager instance = null;
    private BattleGraphParser graphParser;

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
        graphParser = GetComponent<BattleGraphParser>();
    }

    private void Start()
    {
        
    }
}
