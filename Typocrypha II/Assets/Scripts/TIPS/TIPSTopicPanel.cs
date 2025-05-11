using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TIPSTopicPanel : MonoBehaviour
{
    public Action<TIPSEntryData> OnButtonPressed;

    [SerializeField] TextMeshProUGUI uguiTitle, uguiPage;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform buttonContainer;

    private const int PAGE_SIZE = 7;
    private IList<TIPSEntryData> currTopics;
    private int currPage;
    private MenuButton lastPressed;

    public int GetButtonCount(int topicSize, int pageNum) => Mathf.Min(topicSize - pageNum * PAGE_SIZE, PAGE_SIZE);
    public int GetButtonCount() => GetButtonCount(currTopics.Count, currPage);

    public int GetPageCount(int topicSize) => (topicSize - 1) / PAGE_SIZE;
    public int GetPageCount() => GetPageCount(currTopics.Count);

    public void DisplayTitle(string title) => uguiTitle.text = title;

    public void DisplayPageNum(int cur, int max) => uguiPage.text = $"{cur+1}/{max+1}";

    public void SetTopics(IList<TIPSEntryData> topics)
    {
        currTopics = topics;
    }

    public void LoadPageContent(int page = 0, string title = null)
    {
        var pageCount = GetPageCount();
        if (page < 0 || page > pageCount) return;

        //Disable buttons
        foreach (Transform child in buttonContainer)
        {
            child.gameObject.SetActive(false);
        }

        //Initialize buttons
        var buttonCount = GetButtonCount(currTopics.Count, page);
        for (int i = 0; i < buttonCount; i++)
        {
            var button = buttonContainer.GetChild(i).GetComponent<MenuButton>();
            var entry = currTopics[i + page * PAGE_SIZE];
            button.SetText(entry.Title);
            button.button.onClick.RemoveAllListeners();

            if (entry.IsFolder)
            {
                button.button.onClick.AddListener(() => OnButtonPressed(entry));
                button.button.onClick.AddListener(() => lastPressed = button);
            }

            button.gameObject.SetActive(true);
        }

        if (title != null) DisplayTitle(title);
        DisplayPageNum(currPage = page, pageCount);
    }

    public TweenerCore<float, float, FloatOptions> DOFade (float target, float duration) => canvasGroup.DOFade(target, duration);

    public void SelectTopicPageTop()
    {
        var firstButton = buttonContainer.GetComponentInChildren<MenuButton>();
        EventSystem.current.SetSelectedGameObject(null);
        if (firstButton) firstButton.Select();
    }

    public void SelectTopicPageBottom()
    {
        var lastButton = buttonContainer.GetChild(GetButtonCount()-1).GetComponent<MenuButton>();
        EventSystem.current.SetSelectedGameObject(null);
        if (lastButton) lastButton.Select();
    }

    public void SelectTopicLastPressed()
    {
        lastPressed.Select();
    }

    [ContextMenu("Navigate Previous")]
    public void PrevPage(bool selectLastTopic)
    {
        if (currPage <= 0) return;

        LoadPageContent(currPage - 1);
        if (selectLastTopic) SelectTopicPageBottom(); else SelectTopicPageTop();
    }

    [ContextMenu("Navigate Next")]
    public void NextPage()
    {
        if (currPage >= GetPageCount()) return;

        LoadPageContent(currPage + 1);
        SelectTopicPageTop();
    }

    public void LoadEntriesInFolder(string folder)
    {
        var entries = TIPSManager.Instance.FilterEntriesByFolder(folder);
        SetTopics(entries);
        LoadPageContent(0, folder);
        SelectTopicPageTop();
    }
}
