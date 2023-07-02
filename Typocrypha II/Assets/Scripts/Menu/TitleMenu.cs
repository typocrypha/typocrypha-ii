using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private MenuButton firstButton;
    [SerializeField] private SettingsMenu settings;

    private void Start()
    {
        AudioManager.instance.PlayBGM(titleBGM);
        settings.OnClose += Initialize;
        Initialize();
    }

    private void Initialize()
    {
        firstButton.InitializeSelection();
    }
    public void Continue()
    {
        EventSystem.current.enabled = false;
        TransitionManager.instance.Continue();
    }
    public void NewGame()
    {
        EventSystem.current.enabled = false;
        TransitionManager.instance.TransitionToScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Settings()
    {
        settings.Open();
    }
}
