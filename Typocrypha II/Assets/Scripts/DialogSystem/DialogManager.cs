using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Starts and manages dialog sequences.
/// </summary>
public class DialogManager : MonoBehaviour
{
    public static DialogManager instance = null;
    public bool startOnAwake = true; // Should dialog start when scene starts up?

    // DIALOG GRAPH FIELD 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (startOnAwake)
        {
            StartDialog();
        }
    }

    /// <summary>
    /// Starts dialog from beginning of graph.
    /// </summary>
    public void StartDialog()
    {

    }
}

