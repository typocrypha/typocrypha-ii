using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAllyUI : MonoBehaviour
{
    [SerializeField] private ShadowBar shadow;
    [SerializeField] private FilledSlicedImage health;
    [SerializeField] private GameObject ui;

    public void SetEnabled(bool isEnabled)
    {
        ui.SetActive(isEnabled);
    }

    public void UpdateHP(float percent)
    {
        shadow.Curr = percent;
        health.FillAmount = percent;
    }
}
