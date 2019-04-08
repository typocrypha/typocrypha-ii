using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages ally menu for ally casting.
/// </summary>
public class AllyMenu : MonoBehaviour
{
    public ATB3.ATBAlly ally; // Ally this menu is for.
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            AllyCast(KeyCode.UpArrow);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AllyCast(KeyCode.RightArrow);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AllyCast(KeyCode.LeftArrow);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            AllyCast(KeyCode.DownArrow);
        }
    }

    // Case spell based on key pressed.
    void AllyCast(KeyCode dir)
    {
        ally.Cast();
        gameObject.SetActive(false);
    }
}
