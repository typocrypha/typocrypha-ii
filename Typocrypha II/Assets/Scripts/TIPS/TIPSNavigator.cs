//using System;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System;
using TMPro;
using System.Linq;

public class TIPSNavigator : MonoBehaviour
{
    [Header("Internal References")]
    [SerializeField] protected TextMeshProUGUI searchHint;
    [SerializeField] protected TIPSCastBar searchbar;
    [SerializeField] protected TIPSTopicStack topicStack;
    [SerializeField] protected TIPSEntryPanel entryPanel;
    [SerializeField] protected AudioClip sfxSearchBad;
    [SerializeField] protected AudioClip sfxSearchGood;

    public Action OnExit;

    public enum Focus { searchbar = 0, topics = 1 }

    public Focus CurrentFocus { get; private set; }

    private EventSystem currentEventSystem;
    private TIPSEntryData currentEntry;
    private int contentPage;


    private void Awake()
    {
        currentEventSystem = EventSystem.current;
        if (searchbar) searchbar.OnSearchCast.AddListener(HandleSearchInput);
        topicStack.OnButtonSelected += DisplayEntryOnSelect;
    }

    public void InitializeView()
    {
        InitializeSearchbar();
        topicStack.RefreshCurrentFolder();
    }

    private void InitializeSearchbar()
    {
        searchbar.PH.Unpause(PauseSources.TIPS);
        searchbar.Clear();
        FocusOnSearchbar();
        SearchbarShowHint();
    }

    private void Update()
    {
        if (CurrentFocus == Focus.searchbar)
        {
            if (Input.GetAxisRaw("Vertical") != 0)
            {
                FocusOnTopics(true);
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                searchbar.Submit();
            }
            else if (!string.IsNullOrEmpty(Input.inputString))
            {
                searchbar.ProcessInput(Input.inputString);
                SearchbarShowHint();
            }
        }

        if (CurrentFocus == Focus.topics)
        {
            if (new Regex("[A-Za-z\b]+").IsMatch(Input.inputString))
            {
                FocusOnSearchbar();
                searchbar.ProcessInput(Input.inputString);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    DisplayEntryPagePrev();
                }
                else
                {
                    DisplayEntryPageNext();
                }
            }
        }

        // Navigate out of stack and back to visual novel
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (topicStack.currentLayer > TIPSTopicStack.Layer.Top)
            {
                topicStack.StepOutToParent();
            }
            else if (topicStack.currentLayer == TIPSTopicStack.Layer.Top)
            {
                OnExit.Invoke();
            }
        }
    }

    public void SearchbarShowHint()
    {
        string activeEntry = DialogManager.instance.ActiveTIPsEntries.FirstOrDefault() ?? string.Empty;
        searchHint.text = activeEntry.StartsWith(searchbar.Text, StringComparison.InvariantCultureIgnoreCase)
            ? activeEntry
            : string.Empty;
    }

    public void FocusOnSearchbar()
    {
        searchbar.Focus();
        CurrentFocus = Focus.searchbar;
        currentEventSystem.SetSelectedGameObject(null);
    }

    public void FocusOnTopics(bool selectTop)
    {
        searchbar.Unfocus();
        CurrentFocus = Focus.topics;
        if (selectTop)
        {
            topicStack.SelectFirstTopic();
        }
    }

    protected virtual void HandleSearchInput(string input)
    {
        if (input.Length == 0)
        {
            OnMatchNone();
            return;
        }

        var exactMatch = TIPSManager.Instance.HandlePlayerQuery(input, out var allMatches);
        if (allMatches.Length == 0 && exactMatch == null)
            OnMatchNone();
        else if (exactMatch != null)
            OnMatchExact(exactMatch);
        else
            OnMatchPartial(allMatches);
    }

    protected virtual void OnMatchNone()
    {
        AudioManager.instance.PlaySFX(sfxSearchBad);
    }

    protected virtual void OnMatchPartial(TIPSEntryData[] entries)
    {
        AudioManager.instance.PlaySFX(sfxSearchGood);
        //navigate to top layer
        topicStack.JumpToLayer(TIPSTopicStack.Layer.Sub);
        //populate buttons with matching entries
        topicStack.GetCurrentPanel().LoadEntries(entries);
        //navigate to first button
        FocusOnTopics(true);
    }

    protected virtual void OnMatchExact(TIPSEntryData entry)
    {
        AudioManager.instance.PlaySFX(sfxSearchGood);
        FocusOnTopics(false);
        topicStack.JumpToEntry(entry);
    }

    protected virtual void DisplayEntryOnSelect(TIPSEntryData entry)
    {
        DisplayEntry(entryPanel, currentEntry = entry, contentPage = 0);
    }

    protected virtual void DisplayEntryPageNext()
    {
        DisplayEntry(entryPanel, currentEntry, ++contentPage);
    }

    protected virtual void DisplayEntryPagePrev()
    {
        DisplayEntry(entryPanel, currentEntry, --contentPage);
    }

    protected static void DisplayEntry(TIPSEntryPanel panel, TIPSEntryData entry, int page)
    {
        var paginatedContent = Regex.Split(entry.Content, "{br}");
        var pageIndex = (page % paginatedContent.Length + paginatedContent.Length) % paginatedContent.Length;

        panel.SetTitle(entry.Title);
        panel.SetContent(paginatedContent[pageIndex].Trim());
        panel.SetFooter(entry.Footer);
        panel.DisplayPageNum(pageIndex + 1, paginatedContent.Length);
        panel.SetImage(entry is TIPSEntryDemon ? (entry as TIPSEntryDemon).GetSprite() : null);

        if (paginatedContent.Length > 1)
        {
            panel.ShowPaginationIndicator();
            panel.ReverseIndicator(pageIndex == paginatedContent.Length - 1);
        }
        else
        {
            panel.HidePaginationIndicator();
        }
    }

}
