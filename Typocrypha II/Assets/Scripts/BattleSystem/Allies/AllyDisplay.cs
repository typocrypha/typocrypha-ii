using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Reference point for displaying ally spell descriptions.
/// </summary>
public class AllyDisplay : MonoBehaviour
{
    public static AllyDisplay instance;
    public Text descriptionText;

    private void Awake()
    {
        instance = this; // Lazy singleton.
        gameObject.SetActive(false);
    }
}
