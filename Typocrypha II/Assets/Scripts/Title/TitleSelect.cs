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
        Text t = gameObject.GetComponentInChildren<Text>();
        t.color = new Color (219f/255f,56f/255f, 202f/255f); //magenta
        t.text = "> " + t.text + " <";

        if (!isFirstSelect) asrc.PlayOneShot(selectSFX);
        isFirstSelect = false;
    }

    public void OnDeselect(BaseEventData eventData) {
        //Do this on un-highlight
        Text t = gameObject.GetComponentInChildren<Text>();
        t.color = Color.white;
        t.text = t.text.Trim(new char[]{'>',' ', ' ', '<'});
    }

    public void OnSubmit(BaseEventData eventData) {
        asrc.PlayOneShot(enterSFX);
    }
}
