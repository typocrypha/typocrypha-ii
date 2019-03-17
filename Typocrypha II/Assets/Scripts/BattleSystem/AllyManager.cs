using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages high level ally behaviour in battle.
/// Primarily checks for ally cast input, and then dispatches.
/// </summary>
public class AllyManager : MonoBehaviour
{
    public static AllyManager instance = null;
    public GameObject lAlly; // Ally in left slot.
    public GameObject rAlly; // Ally in right slot.

    void Awake()
    {
        if (instance== null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) AllyTryCast(lAlly);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) AllyTryCast(rAlly);
    }

    // Start casting sequence for ally.
    // Also checks casting conditions.
    void AllyTryCast(GameObject ally)
    {
        ATB3.ATBAlly actor = ally.GetComponent<ATB3.ATBAlly>();
        if (actor.isCurrentState(ATB3.ATBStateID.Cast)) return; // Dont double cast.
        // CHECK MANA
        actor.cast(); // TEMP, should open ally menu
    }
}
