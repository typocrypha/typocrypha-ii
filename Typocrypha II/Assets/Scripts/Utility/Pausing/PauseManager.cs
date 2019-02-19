using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages pausing to a pause menu.
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static PauseManager instance = null;
    bool pause = false; // Global pause state.

    public List<PauseHandle> allPausable; // All pausable scripts' pause handles.

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
        DontDestroyOnLoad(gameObject);

        allPausable = new List<PauseHandle>();
    }

    void Update()
    {
        if (Input.GetButtonDown("PauseMenu"))
        {
            pause = !pause; // Toggle pause state.
            PauseAll(pause); // Set pause state of all pausable scripts.
            PauseMenu(pause); // Display/hide pause menu.
        }
    }

    // Pause/Unpause all pausable scripts.
    void PauseAll(bool value)
    {
        foreach(var ph in allPausable)
        {
            ph.Pause = value;
        }
    }

    // Open/Close pause menu
    void PauseMenu(bool value)
    {
        Debug.Log("pause:" + value);
    }
}
