using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleMenu : MonoBehaviour
{
    public bool canContinue = true;
    // Start is called before the first frame update
    void Awake()
    {
        if (canContinue) {
            GameObject resumeButton = transform.GetChild(0).gameObject;
            resumeButton.SetActive(true);
            EventSystem.current.firstSelectedGameObject = resumeButton;
            transform.GetChild(1).gameObject.SetActive(true);
            
        }
    }
}
