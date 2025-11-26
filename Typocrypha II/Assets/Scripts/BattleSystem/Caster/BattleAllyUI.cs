using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAllyUI : MonoBehaviour
{
    [SerializeField] private ShadowBar shadow;
    [SerializeField] private FilledSlicedImage health;

    public void UpdateHP(float percent)
    {
        shadow.Curr = percent;
        health.FillAmount = percent;
    }
}
