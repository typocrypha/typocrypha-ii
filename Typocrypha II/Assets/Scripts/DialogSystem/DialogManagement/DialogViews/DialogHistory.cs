using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private readonly List<HistoryData> history = new List<HistoryData>(256);
    [SerializeField] private HistoryDialog[] historyDialogs;

    private int index = 0;
    private int Index
    {
        get => index;
        set
        {
            index = Mathf.Clamp(value, 0, Mathf.Max(0, history.Count - historyDialogs.Length));
        }
    }


    bool IsShowing
    {
        get => view.activeSelf;
        set => view.SetActive(value);
    }

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
        if (IsShowing)
        {
            if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                Hide();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                int oldIndex = Index++;
                if(oldIndex != Index)
                {
                    SetHistoryDialogs(Index);
                }

            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                int oldIndex = Index--;
                if (oldIndex != Index)
                {
                    SetHistoryDialogs(Index);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Semicolon) && !DialogManager.instance.PH.Pause)
        {
            Show();
        }
    }

    private void Show()
    {
        IsShowing = true;
        // Pause all then unpause self
        PauseManager.instance.PauseAll(true);
        ph.Pause = false;
        Index = 0;
        SetHistoryDialogs(Index);
    }

    private void Hide()
    {
        IsShowing = false;
        // Unpause all
        PauseManager.instance.PauseAll(false);
    }

    private void SetHistoryDialogs(int atIndex)
    {
        for (int i = 0; i < historyDialogs.Length; i++)
        {
            int historyIndex = (history.Count - 1) - (atIndex + i);
            if(historyIndex < 0 || historyIndex >= history.Count)
            {
                historyDialogs[i].gameObject.SetActive(false);
                continue;
            }
            historyDialogs[i].SetData(history[historyIndex]);
            historyDialogs[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Adds a new line to the log.
    /// </summary>
    /// <param name="speaker">Speaker's name.</param>
    /// <param name="dialog">Dialog text.</param>
    public void AddHistory(string speaker, string dialog)
    {
        history.Add(new HistoryData(speaker, dialog));
    }

    public class HistoryData
    {
        public HistoryData(string speaker, string text)
        {
            Speaker = speaker;
            Text = text;
        }

        public string Speaker { get; set; }
        public string Text { get; set; }
    }
}
