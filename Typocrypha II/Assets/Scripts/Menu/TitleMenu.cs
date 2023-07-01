using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private MenuButton firstButton;

    private void Start()
    {
        AudioManager.instance.PlayBGM(titleBGM);
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
}
