using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptPopup : MonoBehaviour
{
    public Text headerText;
    public Text promptText;
    public InputField inputField;
    private bool completed = false;
    public bool LastPromptSuccess { get; private set; } = false;

    public void OnSubmit()
    {
        LastPromptSuccess = inputField.text.ToUpper() == promptText.text.ToUpper();
        completed = true;
    }

    public Coroutine Show(string header, string prompt, float time)
    {
        completed = false;
        LastPromptSuccess = false;
        headerText.text = header;
        promptText.text = prompt;
        inputField.text = string.Empty;
        gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
        return StartCoroutine(PromptCr(time));
    }

    private IEnumerator PromptCr(float time)
    {
        float currTime = 0;
        while(!completed)
        {
            yield return new WaitForEndOfFrame();
            if(time > 0)
            {
                currTime += Time.deltaTime * Settings.GameplaySpeed;
                if (currTime >= time)
                    break;
            }
        }
        gameObject.SetActive(false);
    }
}
