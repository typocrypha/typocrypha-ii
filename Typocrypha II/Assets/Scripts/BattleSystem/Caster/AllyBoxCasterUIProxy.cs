using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBoxCasterUIProxy : MonoBehaviour
{
    public void UpdateHpPercent(float percent)
    {
        AllyBattleBoxManager.instance.UpdateHp(percent);
    }
}
