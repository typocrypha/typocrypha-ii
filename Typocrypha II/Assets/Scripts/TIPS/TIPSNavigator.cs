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

    private void OnEnable()
    {
        FocusOnTopics();
    }

    private void Update()
    {
        if (CurrentFocus == Focus.searchbar && Input.GetAxisRaw("Vertical") != 0)
        {
            FocusOnTopics();
        }

        if (CurrentFocus == Focus.topics && new Regex("[A-Za-z]+").IsMatch(Input.inputString))
        {
            FocusOnSearchbar();
            searchbar.ProcessInput(Input.inputString);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log(CurrentFocus);

            if (CurrentFocus == Focus.topics && topicStack.currentLayer > TIPSTopicStack.Layer.Top)
            {
                topicStack.ExitFolderEntry();
            }
            else if (CurrentFocus == Focus.topics && topicStack.currentLayer == TIPSTopicStack.Layer.Top)
            {
                OnExit.Invoke();
            }
            else if (CurrentFocus == Focus.searchbar)
            {
                OnExit.Invoke();
            }
        }
    }

    public void FocusOnSearchbar()
    {
        searchbar.enabled = true;
        CurrentFocus = Focus.searchbar;
        currentEventSystem.SetSelectedGameObject(null);
    }

    public void FocusOnTopics()
    {
        searchbar.enabled = false;
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

        var matches = TIPSManager.Instance.HandlePlayerQuery(input);
        if (matches.Length == 0)
            OnMatchNone();
        else if (matches.Length == 1 && matches[0].MatchTitleExact(input))
            OnMatchExact(matches[0]);
        else
            OnMatchPartial(matches);
    }

    protected virtual void OnMatchNone()
    {
        AudioManager.instance.PlaySFX(sfxSearchBad);
    }

    protected virtual void OnMatchExact(TIPSEntryData entry)
    {
        AudioManager.instance.PlaySFX(sfxSearchGood);
        //navigate to page
        //navigate to matching button
    }

    protected virtual void OnMatchPartial(TIPSEntryData[] entries)
    {
        AudioManager.instance.PlaySFX(sfxSearchGood);
        //navigate to top layer
        //populate buttons with matching entries
        //navigate to first button
    }

}
