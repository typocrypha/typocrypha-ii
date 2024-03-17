﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manages pausing to a pause menu.
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static PauseManager instance = null;
    [SerializeField] private GameObject pauseMenu; // Pause menu Canvas.
    [SerializeField] private MenuButton firstButton;
    [SerializeField] private SettingsMenu settings;
    bool pause = false; // Global pause state.
    public bool Interactable { get; set; } = true;

    public List<PauseHandle> AllPausable { get; } = new List<PauseHandle>(); // All pausable scripts' pause handles.

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
    }
    private void Start()
    {
        settings.OnClose += Initialize;
    }

    private void Initialize()
    {
        firstButton.InitializeSelection();
        Interactable = true;
    }

    void Update()
    {
        if (!Interactable)
            return;
        if (Input.GetButtonDown("PauseMenu"))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        pause = !pause; // Toggle pause state.
        PauseAll(pause); // Set pause state of all pausable scripts.
        PauseMenu(pause); // Display/hide pause menu.
    }

    // Pause/Unpause all pausable scripts.
    public void PauseAll(bool value)
    {
        List<PauseHandle> destroyed = new List<PauseHandle>(); // Destroyed pausables.
        foreach(var ph in AllPausable)
        {
            try
            {
                ph.Pause = value;
            }
            catch (MissingReferenceException) // Check if object was destroyed.
            {
                destroyed.Add(ph);
            }
        }
        foreach(var ph in destroyed) AllPausable.Remove(ph);
    }

    // Open/Close pause menu
    void PauseMenu(bool value)
    {
        pauseMenu.SetActive(value);
        if (value)
        {
            FaderManager.instance.FadeAll(0.5f, Color.black);
            Initialize();
        }
        else
        {
            FaderManager.instance.FadeAll(0.0f, Color.black);
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
        Interactable = false;
        EventSystem.current.enabled = false;
        TransitionManager.instance.TransitionToMainMenu();
    }

    public void OpenSettingsMenu()
    {
        Interactable = false;
        settings.Open();
    }
}
