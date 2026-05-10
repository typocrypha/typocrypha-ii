using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TIPSTopicPanel : MonoBehaviour
{
    public Action<TIPSEntryData> OnButtonPressed;
    public Action<TIPSEntryData> OnButtonSelected;

    [SerializeField] TextMeshProUGUI uguiTitle, uguiPage;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] List<MenuButton> buttons;
    [SerializeField] List<Image> icons;
    [SerializeField] Sprite iconFolder, iconFile;
    [SerializeField] TIPSNavigator navigator;


    private int PageSize => buttons.Count;
    public TIPSEntryData TopEntry => currTopics.Count > 0 ? currTopics[0] : null;
    private IList<TIPSEntryData> currTopics;
    private string currFolder;
    private int currPage;

    private void Awake()
    {
        SetButtonNavigation();
    }

    public int GetButtonCount(int topicSize, int pageNum) => Mathf.Min(topicSize - pageNum * PageSize, PageSize);
    public int GetButtonCount() => GetButtonCount(currTopics.Count, currPage);

    public int GetPageCount(int topicSize) => (topicSize - 1) / PageSize;
    public int GetPageCount() => GetPageCount(currTopics.Count);

    public void DisplayTitle(string title) => uguiTitle.text = title;

    public void DisplayPageNum(int cur, int max) => uguiPage.text = $"{cur + 1}/{max + 1}";

    public void SetCurrentFolder(string folder)
    {
        currFolder = folder;
        currTopics = TIPSManager.Instance.FilterEntries(folder, true);
    }

    public void LoadPageContent(int page = 0, string title = null)
    {
        var pageCount = GetPageCount();
        if (page < 0 || page > pageCount) return;

        //Disable buttons
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }

        //Initialize buttons
        var buttonCount = GetButtonCount(currTopics.Count, page);
        for (int i = 0; i < buttonCount; i++)
        {
            var entry = currTopics[i + page * PageSize];
            var button = buttons[i];
            var icon = icons[i];

            button.gameObject.name = entry.ID;
            button.SetText(entry.ID);
            icon.sprite = entry.IsFolder ? iconFolder : iconFile;

            // Setup listeners
            button.button.onClick.RemoveAllListeners();
            button.onSelect.RemoveAllListeners();
            button.button.onClick.AddListener(() => OnButtonPressed?.Invoke(entry));
            button.onSelect.AddListener(() => OnButtonSelected?.Invoke(entry));

            button.gameObject.SetActive(true);
        }

        if (title != null) DisplayTitle(title);
        DisplayPageNum(currPage = page, pageCount);
    }

    public TweenerCore<float, float, FloatOptions> DOFade(float target, float duration) => canvasGroup.DOFade(target, duration);

    public void SelectTopicPageTop()
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (buttons.Count > 0)
        {
            buttons[0].Select();
        }
    }

    public void SelectTopicPageBottom()
    {
        EventSystem.current.SetSelectedGameObject(null);
        for (int i = 0; i < buttons.Count; ++i)
        {
            if (buttons[i].gameObject.activeSelf) buttons[i].Select();
        }
    }

    public void SelectEntry(TIPSEntryData entry)
    {
        SetCurrentFolder(entry.Parent);
        int page = currTopics.IndexOf(entry) / PageSize;
        int offset = currTopics.IndexOf(entry) % PageSize;

        LoadEntriesInFolder(entry.Parent, page);
        buttons[offset].Select();
    }

    [ContextMenu("Navigate Previous")]
    public void PrevPage()
    {
        if (currPage <= 0 && IsTopSelected())
        {
            navigator.FocusOnSearchbar();
            return;
        }
        else if (currPage <= 0)
        {
            SelectTopicPageTop();
            return;
        }

        LoadPageContent(currPage - 1);
        SelectTopicPageBottom();
    }

    [ContextMenu("Navigate Next")]
    public void NextPage()
    {
        if (currPage >= GetPageCount())
        {
            SelectTopicPageBottom();
            return;
        }

        LoadPageContent(currPage + 1);
        SelectTopicPageTop();
    }

    public void LoadEntries(IList<TIPSEntryData> entries)
    {
        currFolder = "Search";
        currTopics = entries;
        LoadPageContent(0, currFolder);
    }

    public void LoadEntriesInFolder(string folder, int page = 0)
    {
        SetCurrentFolder(folder);
        LoadPageContent(page, folder);
    }

    private void SetButtonNavigation()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            //navigation within a page
            var nav = buttons[i].button.navigation;
            nav.mode = UnityEngine.UI.Navigation.Mode.Explicit;
            nav.selectOnUp = i > 0 ? buttons[i - 1].button : null;
            nav.selectOnDown = i < buttons.Count - 1 ? buttons[i + 1].button : null;
            buttons[i].button.navigation = nav;

            //navigation to previous page
            var prevHandler = buttons[i].gameObject.AddComponent<MoveEventHandler>();
            prevHandler.AddListeners(PrevPage).EnableTriggers(MoveDirection.Left);
            if (i == 0) prevHandler.EnableTriggers(MoveDirection.Up);
            //if (i == 0) prevHandler.AddListeners(PrevPage).EnableTriggers(MoveDirection.Up);

            //navigation to next page
            var nextHandler = buttons[i].gameObject.AddComponent<MoveEventHandler>();
            nextHandler.AddListeners(NextPage).EnableTriggers(MoveDirection.Right);
            if (i == buttons.Count - 1) nextHandler.EnableTriggers(MoveDirection.Down);
            //if (i == buttons.Count - 1) nextHandler.AddListeners(NextPage).EnableTriggers(MoveDirection.Down);
        }
    }

    private bool IsTopSelected()
    {
        return buttons.Count > 0 && EventSystem.current.currentSelectedGameObject == buttons[0].gameObject;
    }
}
