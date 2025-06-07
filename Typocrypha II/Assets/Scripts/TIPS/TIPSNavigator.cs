//using System;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System;

public class TIPSNavigator : MonoBehaviour
{
    [Header("Internal References")]
    [SerializeField] protected TIPSCastBar searchbar;
    [SerializeField] protected TIPSTopicStack topicStack;
    [SerializeField] protected TIPSEntryPanel entryPanel;
    [SerializeField] protected AudioClip sfxSearchBad;
    [SerializeField] protected AudioClip sfxSearchGood;

    public Action OnExit;

    public enum Focus { searchbar = 0, topics = 1, content = 2 }

    public Focus CurrentFocus { get; private set; }

    private EventSystem currentEventSystem;


    private void Awake()
    {
        currentEventSystem = EventSystem.current;
        if (searchbar) searchbar.OnSearchCast.AddListener(HandleSearchInput);
    }

    private void Start()
    {
        topicStack.OnButtonSelected += DisplayEntry;
    }

    public void InitializeFocus()
    {
        searchbar.PH.Unpause(PauseSources.TIPS);
        FocusOnSearchbar();
    }

    private void Update()
    {
        if (CurrentFocus == Focus.searchbar)
        {
            if(Input.GetAxisRaw("Vertical") != 0)
            {
                FocusOnTopics();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                searchbar.Submit();
            }
            else
            {
                searchbar.ProcessInput(Input.inputString);
            }
        }

        if ((CurrentFocus == Focus.topics || CurrentFocus == Focus.content) && new Regex("[A-Za-z\b]+").IsMatch(Input.inputString))
        {
            FocusOnSearchbar();
            searchbar.ProcessInput(Input.inputString);
        }

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

    public void FocusOnSearchbar()
    {
        searchbar.Focus();
        CurrentFocus = Focus.searchbar;
        currentEventSystem.SetSelectedGameObject(null);
    }

    public void FocusOnTopics()
    {
        searchbar.Unfocus();
        CurrentFocus = Focus.topics;
        topicStack.SelectFirstTopic();
    }

    protected virtual void HandleSearchInput(string input)
    {
        if (input.Length == 0)
        {
            OnMatchNone();
            return;
        }

        var exactMatch = TIPSManager.Instance.HandlePlayerQuery(input, out var allMatches);
        if (allMatches.Length == 0)
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
        //populate buttons with matching entries
        //navigate to first button
    }

    protected virtual void OnMatchExact(TIPSEntryData entry)
    {
        AudioManager.instance.PlaySFX(sfxSearchGood);
        topicStack.JumpToEntry(entry);
    }

    protected virtual void DisplayEntry(TIPSEntryData entry)
    {
        entryPanel.SetTitle(entry.Title);
        entryPanel.SetContent(entry.Content);
    }

}
