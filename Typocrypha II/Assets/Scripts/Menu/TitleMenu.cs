using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private MenuButton continueButton;
    [SerializeField] private MenuButton newGameButton;
    [SerializeField] private SettingsMenu settings;

    private static readonly AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 2, 0);

    private void Start()
    {
        AudioManager.instance.PlayBGM(titleBGM);
        settings.OnClose += Initialize;
        Initialize();
    }

    private void Initialize()
    {
        if (SaveManager.instance.HasCampaignSaveFile())
        {
            continueButton.gameObject.SetActive(true);
            continueButton.InitializeSelection();
        }
        else
        {
            continueButton.gameObject.SetActive(false);
            newGameButton.InitializeSelection();
        }
    }
    public void Continue()
    {
        EventSystem.current.enabled = false;
        SaveManager.instance.LoadCampaign();
        AudioManager.instance.StopBGM(fadeOutCurve);
        TransitionManager.instance.Continue();
    }
    public void NewGame()
    {
        EventSystem.current.enabled = false;
        SaveManager.instance.NewGame();
        AudioManager.instance.StopBGM(fadeOutCurve);
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
