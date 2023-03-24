using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleMenu : MonoBehaviour
{
    public void Continue()
    {
        TransitionManager.instance.TransitionToNextScene();
    }
    public void NewGame()
    {
        TransitionManager.instance.TransitionToScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
