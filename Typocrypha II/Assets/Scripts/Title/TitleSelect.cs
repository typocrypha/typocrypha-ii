using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleSelect : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    private AudioSource asrc;
    
    public AudioClip selectSFX;
    public AudioClip enterSFX;

    private static bool isFirstSelect = true;

    void Awake() {
        asrc = gameObject.GetComponent<AudioSource>();
    }
    public void OnSelect(BaseEventData eventData) {
        //Do this on highlight
        gameObject.GetComponentInChildren<Text>().color = new Color (219f/255f,56f/255f, 202f/255f);
        if (!isFirstSelect) asrc.PlayOneShot(selectSFX);
        isFirstSelect = false;
    }

    public void OnDeselect(BaseEventData eventData) {
        //Do this on un-highlight
        gameObject.GetComponentInChildren<Text>().color = Color.white;
    }

    public void OnSubmit(BaseEventData eventData) {
        asrc.PlayOneShot(enterSFX);
    }
}
