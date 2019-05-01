using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class to organize all AI components and force them to require caster AI.
/// Contains protected references to the caster and parent AI (caster and AI respectively).
/// These references will be initialized if InitializeBase is called
/// </summary>
[RequireComponent(typeof(CasterAI))]
public abstract class AIComponent : MonoBehaviour
{
    protected CasterAI AI;
    protected Caster caster;
    protected void InitializeBase()
    {
        AI = GetComponent<CasterAI>();
        caster = GetComponent<Caster>();
    }
}
