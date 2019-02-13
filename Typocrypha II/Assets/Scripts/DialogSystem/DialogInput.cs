using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A specific instance of an input display during dialog.
/// </summary>
public class DialogInput : MonoBehaviour
{
    void Start()
    {
        GetComponentInChildren<InputField>().ActivateInputField();
        GetComponentInChildren<InputField>().Select();
    }

    /// <summary>
    /// Destroys dialog display.
    /// </summary>
    /// <param name="value">Submitted value.</param>
    public void Submit(string value)
    {
        DialogInputManager.instance.SubmitInput(value);
        Destroy(gameObject);
    }
}
