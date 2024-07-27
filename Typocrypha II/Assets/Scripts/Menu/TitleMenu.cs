//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities.Unity;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private MenuButton continueButton;
    [SerializeField] private MenuButton newGameButton;
    [SerializeField] private SettingsMenu settings;
    [SerializeField] private ConfirmationWindow confirmationWindow;

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
            newGameButton.button.onClick.ReplaceAllListeners(ConfirmNewGame);
        }
        else
        {
            continueButton.gameObject.SetActive(false);
            newGameButton.InitializeSelection();
            newGameButton.button.onClick.ReplaceAllListeners(NewGame);
        }

        confirmationWindow.SetConfirmAction(NewGame);
        confirmationWindow.SetDenyAction(newGameButton.InitializeSelection);
    }
    public void Continue()
    {
        EventSystem.current.enabled = false;
        SaveManager.instance.LoadCampaign();
        AudioManager.instance.StopBGM(fadeOutCurve);
        TransitionManager.instance.Continue();
    }

    public void ConfirmNewGame()
    {
        confirmationWindow.Open(null, false);
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
