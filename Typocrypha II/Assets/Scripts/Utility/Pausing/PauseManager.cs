using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manages pausing to a pause menu.
/// </summary>
public class PauseManager : MonoBehaviour, IPausable
{
    public static PauseManager instance = null;
    [SerializeField] private GameObject pauseMenu; // Pause menu Canvas.
    [SerializeField] private MenuButton firstButton;
    [SerializeField] private SettingsMenu settings;
    bool pause = false; // Global pause state.

    public List<PauseHandle> AllPausable { get; } = new List<PauseHandle>(); // All pausable scripts' pause handles.

    public PauseHandle PH { get; private set; }
    private void OnPause(bool pause)
    {
        enabled = !pause;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            PH = new PauseHandle(OnPause, true);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        settings.OnClose += Initialize;
        PH.Pause(PauseSources.Title);
    }

    private void Initialize()
    {
        firstButton.InitializeSelection();
        PH.Unpause(PauseSources.Self);
    }

    void Update()
    {
        if (Input.GetButtonDown("PauseMenu"))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        SetPause(!pause);
    }

    private void SetPause(bool value)
    {
        pause = value;
        PauseAll(value, PauseSources.PauseMenu, false); // Set pause state of all pausable scripts.
        PauseMenu(value); // Display/hide pause menu.
    }

    public void UnpauseButton()
    {
        PH.Pause(PauseSources.Self);
        StartCoroutine(UnpauseButtonCR());
    }

    private IEnumerator UnpauseButtonCR()
    {
        yield return null;
        SetPause(false);
        PH.Unpause(PauseSources.Self);
    }

    // Pause/Unpause all pausable scripts.
    public void PauseAll(bool value, PauseSources sources, bool includePauseMenu, params PauseHandle[] except)
    {
        List<PauseHandle> destroyed = null; // Destroyed pausables.
        foreach (var ph in AllPausable)
        {
            try
            {
                if (!includePauseMenu && ph == PH) // Skip own pause handle
                    continue;
                if (except != null && except.Contains(ph))
                    continue;
                if (value)
                {
                    ph.Pause(sources);
                }
                else
                {
                    ph.Unpause(sources);
                }
            }
            catch (System.Exception e) // Check if object was destroyed.
            {
                Debug.LogError($"PauseHandle exception: {e.Message}");
                destroyed = destroyed ?? new List<PauseHandle>();
                destroyed.Add(ph);
            }
        }
        if(destroyed != null)
        {
            foreach (var ph in destroyed) 
                AllPausable.Remove(ph);
        }
    }

    // Open/Close pause menu
    void PauseMenu(bool value)
    {
        pauseMenu.SetActive(value);
        var keyboard = Typocrypha.Keyboard.instance;
        if (value)
        {
            FaderManager.instance.FadeAll(0.5f, Color.black);
            Initialize();
            if(keyboard != null)
            {
                keyboard.DisableInactiveSfx = true;
            }
        }
        else
        {
            FaderManager.instance.FadeAll(0.0f, Color.black);
            if (keyboard != null)
            {
                keyboard.DisableInactiveSfx = false;
            }
        }
    }


    // Menu Button Functions
    public void Quit()
    {
        SaveManager.instance.Save();
        Application.Quit();
    }

    public void MainMenu()
    {
        PH.Pause(PauseSources.Title);
        PauseMenu(false);
        EventSystem.current.enabled = false;
        TransitionManager.instance.TransitionToMainMenu();
    }

    public void OpenSettingsMenu()
    {
        PH.Pause(PauseSources.Self);
        settings.Open();
    }

    public void Cleanup()
    {
        AllPausable.RemoveAll(IsNotPersistent);
    }

    private static bool IsNotPersistent(PauseHandle ph) => !ph.Persistent;
}
