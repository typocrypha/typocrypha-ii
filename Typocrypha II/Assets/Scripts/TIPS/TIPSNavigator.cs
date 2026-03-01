using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System;
using TMPro;
using System.Linq;

// Handles user inputs for TIPS menu
public class TIPSNavigator : MonoBehaviour
{
    [Header("Internal References")]
    [SerializeField] TextMeshProUGUI searchHint;
    [SerializeField] TIPSCastBar searchbar;
    [SerializeField] TIPSTopicStack topicStack;
    [SerializeField] TIPSEntryPanel entryPanel;
    [SerializeField] AudioClip sfxSearchBad;
    [SerializeField] AudioClip sfxSearchGood;
    [SerializeField] TIPSControlGuideSetter controlGuide;

    public Action OnExit;

    public enum Focus { searchbar = 0, topics = 1 }

    public Focus CurrentFocus { get; private set; }

    private EventSystem currentEventSystem;
    private TIPSEntryData currentEntry;
    private int contentPage;

    private const string DEFAULT_SEARCH_HINT = "Type to search";

    private void Awake()
    {
        currentEventSystem = EventSystem.current;
        if (searchbar) searchbar.OnSearchCast.AddListener(HandleSearchInput);
        topicStack.OnButtonSelected += DisplayEntryOnSelect;
    }

    /// <summary>
    /// Prepare the TIPS view.
    /// </summary>
    public void InitializeView()
    {
        InitializeSearchbar();
        topicStack.RefreshCurrentFolder();
    }

    /// <summary>
    /// Prepare the searchbar.
    /// </summary>
    private void InitializeSearchbar()
    {
        searchbar.PH.Unpause(PauseSources.TIPS);
        searchbar.Clear();
        FocusOnSearchbar();
        SearchbarShowHint();
    }

    private void Update()
    {
        /***
         * Searchbar navigation
         * 1. Down arrow to focus on topics
         * 2. Return key to input search
         * 3. Any letter key to modify search term
         */
        if (CurrentFocus == Focus.searchbar)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                searchbar.Clear();
                SearchbarShowHint();
                FocusOnTopics(true);
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                searchbar.Submit();
                SearchbarShowHint();
            }
            else if (!string.IsNullOrEmpty(Input.inputString))
            {
                searchbar.ProcessInput(Input.inputString);
                SearchbarShowHint();
            }
        }

        /***
         * Topics navigation
         * 1. Any letter key to focus on searchbar
         * 2. Space or Shift+Space to cycle through entry pages
         * 3. Backspace to navigate up a folder
         */
        if (CurrentFocus == Focus.topics)
        {
            if (new Regex("[A-Za-z]+").IsMatch(Input.inputString))
            {
                FocusOnSearchbar();
                searchbar.ProcessInput(Input.inputString);
                SearchbarShowHint();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
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
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (topicStack.currentLayer > TIPSTopicStack.Layer.Top)
                {
                    topicStack.StepOutToParent();
                }
            }
        }
    }

    /// <summary>
    /// Shows a hint in the searchbar depending on context
    /// </summary>
    public void SearchbarShowHint()
    {
        string activeEntry = DialogManager.instance.ActiveTIPsEntries.FirstOrDefault() ?? string.Empty;
        string hintMessage = TIPSManager.Instance.EntryIsUnlocked(activeEntry) ? string.Empty : activeEntry;

        searchHint.text = (string.IsNullOrEmpty(hintMessage) && string.IsNullOrEmpty(searchbar.Text))
            ? DEFAULT_SEARCH_HINT
            : hintMessage.StartsWith(searchbar.Text, StringComparison.InvariantCultureIgnoreCase)
                ? hintMessage.ToUpper()
                : string.Empty;
            
    }

    /// <summary>
    /// Set the focus of the TIPS UI to the searchbar
    /// </summary>
    public void FocusOnSearchbar()
    {
        searchbar.Focus();
        CurrentFocus = Focus.searchbar;
        currentEventSystem.SetSelectedGameObject(null);
        controlGuide.SetContextSearch();
    }


    /// <summary>
    /// Set the focus of the TIPS UI to the topics
    /// </summary>
    public void FocusOnTopics(bool selectTop)
    {
        searchbar.Unfocus();
        CurrentFocus = Focus.topics;
        if (selectTop)
        {
            topicStack.SelectFirstTopic();
        }
        controlGuide.SetContextTopics();
    }


    /// <summary>
    /// Compare TIPS entries against the search term and
    /// respond differently depending on the number of matches.
    /// </summary>
    /// <param name="input"> The search term to look up. </param>
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

    /// <summary>
    /// Alert the player and do nothing.
    /// </summary>
    protected virtual void OnMatchNone()
    {
        AudioManager.instance.PlaySFX(sfxSearchBad);
    }

    /// <summary>
    /// Pull up a list containing entries that matched the search.
    /// </summary>
    /// <param name="entries"> List of matching entries. </param>
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

    /// <summary>
    /// Navigate to the exact entry.
    /// </summary>
    /// <param name="entry"> Entry to navigate to. </param>
    protected virtual void OnMatchExact(TIPSEntryData entry)
    {
        AudioManager.instance.PlaySFX(sfxSearchGood);
        FocusOnTopics(false);
        topicStack.JumpToEntry(entry);
    }

    #region TODO: move out of navigator
    /// <summary>
    /// Update the entry view with the current selection.
    /// </summary>
    /// <param name="entry"> Entry currently selected. </param>
    protected virtual void DisplayEntryOnSelect(TIPSEntryData entry)
    {
        DisplayEntry(entryPanel, currentEntry = entry, contentPage = 0);
    }

    /// <summary>
    /// Display the next page of the current entry.
    /// </summary>
    protected virtual void DisplayEntryPageNext()
    {
        DisplayEntry(entryPanel, currentEntry, ++contentPage);
    }


    /// <summary>
    /// Display the previous page of the current entry.
    /// </summary>
    protected virtual void DisplayEntryPagePrev()
    {
        DisplayEntry(entryPanel, currentEntry, --contentPage);
    }

    /// <summary>
    /// Set the entry panel.
    /// </summary>
    /// <param name="panel"> View for entry data. </param>
    /// <param name="entry"> Entry data source. </param>
    /// <param name="page"> Current page to display. </param>
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
    #endregion
}
