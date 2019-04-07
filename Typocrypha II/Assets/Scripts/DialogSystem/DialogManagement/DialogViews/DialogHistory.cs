using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages saving past dialog lines and displaying the dialog history.
/// </summary>
public class DialogHistory : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH { get => ph; }

    public void OnPause(bool b)
    {
        enabled = !b;
    }
    #endregion

    public static DialogHistory instance = null;
    public GameObject view; // Gameobject for entire viewable window.
    public RectTransform content; // Content transform for scroll view.
    public Scrollbar scrollBar; // Scrollbar for window.
    public GameObject dialogBoxPrefab; // Prefab for line of dialog.

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        ph = new PauseHandle(OnPause);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            view.SetActive(!view.activeSelf);
            PauseManager.instance.PauseAll(view.activeSelf);
            if (view.activeSelf) ph.Pause = false;
        }
    }

    /// <summary>
    /// Adds a new line to the log.
    /// </summary>
    /// <param name="speaker">Speaker's name.</param>
    /// <param name="dialog">Dialog text.</param>
    public void AddHistory(string speaker, string dialog)
    {
        // Create new dialog box and set its parameters.
        var go = Instantiate(dialogBoxPrefab, content);
        var dialogBox = go.GetComponent<DialogBox>();
        dialogBox.dialogText.text = DialogParser.instance.RemoveTags(dialog);
        dialogBox.SetBoxHeight(true);
        go.transform.Find("NameText").GetComponent<Text>().text = 
            speaker == "" ? "-" : speaker;
        // Resize window.
        content.sizeDelta = new Vector2(content.sizeDelta.x, 
            content.sizeDelta.y + dialogBox.GetBoxHeight() + 
            content.GetComponent<VerticalLayoutGroup>().spacing);
        // Scroll to most recent entry.
        scrollBar.value = 0f; 
    }
}
