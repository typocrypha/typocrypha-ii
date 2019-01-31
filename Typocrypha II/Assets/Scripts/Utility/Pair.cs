using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// OBSOLETE: Should use C# 'Tuple'

/// <summary>
/// Tuple of two generic objects.
/// </summary>
/// <typeparam name="T">First object.</typeparam>
/// <typeparam name="U">Second object.</typeparam>
public class Pair<T,U>
{
    public T first;
    public U second;

    public Pair(T _first, U _second)
    {
        first = _first;
        second = _second;
    }

}
