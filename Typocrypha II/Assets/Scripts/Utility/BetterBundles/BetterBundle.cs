using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BetterBundle<T> : ScriptableObject
{
    public string[] assetPaths;
    public abstract void Clear();
    public abstract void Add(T item);
}
