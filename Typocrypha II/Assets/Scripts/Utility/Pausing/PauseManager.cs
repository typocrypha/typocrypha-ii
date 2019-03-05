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
    public GameObject pauseMenu; // Pause menu Canvas.
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
    public void PauseAll(bool value)
    {
        foreach(var ph in allPausable)
        {
            ph.Pause = value;
        }
    }

    // Open/Close pause menu
    void PauseMenu(bool value)
    {
        pauseMenu.SetActive(value);
        if (value) FaderManager.instance.FadeAll(0.5f, Color.black);
        else       FaderManager.instance.FadeAll(0.0f, Color.black);
    }
}
