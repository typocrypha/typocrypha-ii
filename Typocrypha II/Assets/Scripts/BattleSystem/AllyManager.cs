using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages high level ally behaviour in battle.
/// Primarily checks for ally cast input, and then dispatches.
/// </summary>
public class AllyManager : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH { get => ph; }
    public void OnPause(bool b)
    {
        Debug.Log("AllyManager pause:" + b);
    }
    #endregion
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
        ph = new PauseHandle(OnPause);
    }

    void Update()
    {
        if (PH.Pause) return;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) OpenMenu(lAlly);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) OpenMenu(rAlly);
    }

    // Open ally menu for casting.
    void OpenMenu(GameObject ally)
    {
        if (ally != null)
        {
            PH.Pause = true;
            ally.GetComponent<ATB3.ATBAlly>().Menu();
        }
    }
}
