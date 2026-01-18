using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleTrigger : MonoBehaviour
{
    [SerializeField] private string id;

    private void Awake()
    {
        BattleManager.instance.OnBattleEventTriggered += OnBattleEventTriggered;
    }

    private void OnDestroy()
    {
        BattleManager.instance.OnBattleEventTriggered -= OnBattleEventTriggered;
    }

    private void OnBattleEventTriggered(string id)
    {
        if (id != this.id)
            return;
        Trigger();
    }

    protected abstract void Trigger();
}
