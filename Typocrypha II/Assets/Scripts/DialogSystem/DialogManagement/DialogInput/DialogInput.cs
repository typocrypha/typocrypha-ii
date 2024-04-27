using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A specific instance of an input display during dialog.
/// </summary>
public class DialogInput : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH { get => ph; }

    public void OnPause(bool b)
    {
    }
    #endregion

    void Awake()
    {
        ph = new PauseHandle(OnPause);
    }

    void Start()
    {
        GetComponentInChildren<InputField>().ActivateInputField();
        GetComponentInChildren<InputField>().Select();
    }

    Regex alphaNumeric = new Regex("^[a-zA-Z0-9]*$");

    /// <summary>
    /// Destroys dialog display.
    /// </summary>
    /// <param name="value">Submitted value.</param>
    public void Submit(string value)
    {
        if (!alphaNumeric.IsMatch(value) || value.Length == 0 || PH.Paused)
        {
            return;
        }
        Debug.Log("input:" + alphaNumeric.IsMatch(value) + ":" + value.Length);
        DialogInputManager.instance.SubmitInput(value);
        Destroy(gameObject);
    }
}
