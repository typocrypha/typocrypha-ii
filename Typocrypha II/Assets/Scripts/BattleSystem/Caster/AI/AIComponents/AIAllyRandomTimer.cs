using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIAllyRandomTimer : AIComponent
{
    [SerializeField] private float baseChargeTime;
    [SerializeField] private float chargeTimeVariance;

    protected ATB3.ATBActor actor;
    private float charge;
    private float goal;

    protected override void Awake()
    {
        base.Awake();
        actor = GetComponent<ATB3.ATBActor>();
        UpdateGoal();
    }

    private void Update()
    {
        if (actor.IsPausedOrCasting())
            return;
        OnUpdate();
    }
    protected virtual void OnUpdate()
    {
        if ((charge += (Time.deltaTime * Settings.GameplaySpeed)) >= goal)
        {
            charge = 0;
            UpdateGoal();
            DoAction();
        }
    }

    private void UpdateGoal()
    {
        float variance = (float)RandomUtils.RandomU.instance.RandomDouble() * chargeTimeVariance;
        if (RandomUtils.RandomU.instance.RandomBool())
        {
            goal = baseChargeTime + variance;
        }
        else
        {
            goal = baseChargeTime - variance;
        }
    }


    protected abstract void DoAction();
}
