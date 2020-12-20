﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Utility methods for Unity.
/// </summary>
public static class UnityUtil
{
    /// <summary>
    /// Sorts the objects in the transforms hiearchy in ascending order based on comparator.
    /// Uses selection sort.
    /// </summary>
    /// <param name="comparator">Comparison function.</param>
    public static void SortHiearchy(this Transform tr, System.Func<Transform, Transform, int> comparator)
    {
        for (int i = 0; i < tr.childCount; i++)
        {
            Transform min = tr.GetChild(i);
            for (int j = i + 1; j < tr.transform.childCount; j++)
                if (comparator(min, tr.GetChild(j)) < 0)
                    min = tr.GetChild(j);
            min.SetSiblingIndex(i);
        }
    }
}

[System.Serializable]
public class IntEvent : UnityEvent<int> { }

[System.Serializable]
public class StringEvent : UnityEvent<string> { }

[System.Serializable]
public class FloatEvent : UnityEvent<float> { }