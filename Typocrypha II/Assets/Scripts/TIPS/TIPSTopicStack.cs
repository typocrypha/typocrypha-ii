using UnityEngine;
using DG.Tweening;
using System;

public class TIPSTopicStack : MonoBehaviour
{
    public const string rootFolderName = "Root";
    [SerializeField] private RectTransform panelContainer;
    [SerializeField] private TIPSTopicPanel panelTop, panelSub, panelAux, panelEnd;
    [SerializeField] Vector2 auxOffsetPosition = new Vector2(-16, 16);

    public enum Layer { Top = 0, Sub = 1, Aux = 2, End = 3};
    public Layer currentLayer = Layer.Top;

    public Action<TIPSEntryData> OnButtonSelected;

    private TIPSTopicPanel[] _panels;
    private TIPSTopicPanel[] panels => _panels != null ? _panels : _panels = new TIPSTopicPanel[] { panelTop, panelSub, panelAux, panelEnd };

    private TIPSEntryData latestFolderEntered;

    public TIPSTopicPanel GetPanel(Layer layer) => panels[(int)layer];
    public TIPSTopicPanel GetCurrentPanel() => GetPanel(currentLayer);

    private void Awake()
    {
        foreach (var p in panels)
        {
            p.OnButtonPressed += StepIntoEntry;
            p.OnButtonSelected += OnButtonSelected;
        }
        panelTop.LoadEntriesInFolder(rootFolderName);
    }

    private void OnDestroy()
    {
        foreach (var p in panels)
        {
            p.OnButtonPressed -= StepIntoEntry;
            p.OnButtonSelected -= OnButtonSelected;
        }
    }

    public void RefreshCurrentFolder()
    {
        if (latestFolderEntered == null)
        {
            panelTop.LoadEntriesInFolder(rootFolderName);
            return;
        }
        GetCurrentPanel().LoadEntriesInFolder(latestFolderEntered.ID);
    }

    /// <summary>
    /// Determine alpha value of panel based on distance to the currently active panel
    /// </summary>
    /// <param name="panel">The panel whose alpha is to be determined. </param>
    /// <returns> The alpha value of the panel. </returns>
    private float GetFadeValue(Layer panel)
    {
        const float NEAR = 1, FAR = 0.5f, NONE = 0f;
        int distance = currentLayer - panel;
        return distance == 0 ? NEAR : distance == 1 ? FAR : NONE;
    }

    public Sequence JumpToLayer(Layer target, float duration = 0.33f)
    {
        currentLayer = target;

        return DOTween.Sequence()
            .Join(panelTop.DOFade(GetFadeValue(Layer.Top), duration))
            .Join(panelSub.DOFade(GetFadeValue(Layer.Sub), duration))
            .Join(panelAux.DOFade(GetFadeValue(Layer.Aux), duration))
            .Join(panelEnd.DOFade(GetFadeValue(Layer.End), duration))
            .Join(panelContainer.DOAnchorPos(auxOffsetPosition * (int)target, duration));
    }

    public void StepToLayer(Layer target, float duration = 0.33f)
    {
        if (target < Layer.Top || target > Layer.End) return;

        var stepMultiple = DOTween.Sequence();
        var direction = target > currentLayer ? 1 : -1;
        while (currentLayer != target)
        {
            var nextLayer = (Layer)((int)currentLayer + direction);
            stepMultiple.Append(JumpToLayer(nextLayer, duration));
        }
    }

    [ContextMenu("Step Next")]
    public void StepToNextLayer() => StepToLayer(currentLayer + 1);

    [ContextMenu("Step Previous")]
    public void StepToPreviousLayer() => StepToLayer(currentLayer - 1);

    [ContextMenu("Select First Topic")]
    public void SelectFirstTopic()
    {
        GetPanel(currentLayer).SelectTopicPageTop();
    }

    private void StepIntoEntry(TIPSEntryData entry)
    {
        if (!entry.IsFolder) return;
        StepToNextLayer();
        var current = GetCurrentPanel();
        current.LoadEntriesInFolder(entry.ID);
        current.SelectTopicPageTop();
        latestFolderEntered = entry;
    }

    public void JumpToEntry(TIPSEntryData entry)
    {
        JumpToLayer((Layer)entry.Depth);
        var current = GetCurrentPanel();
        current.SelectEntry(entry);

        for (int i = 0; i < (int)currentLayer; i++)
        {
            panels[i].LoadEntriesInFolder(entry.Categorization[i]);
        }

        latestFolderEntered = TIPSManager.Instance.GetEntry(entry.Parent);
    }

    public void StepOutToParent()
    {
        if (currentLayer == Layer.Top) return;

        StepToPreviousLayer();
        var current = GetCurrentPanel();

        if (latestFolderEntered == null)
        {
            panelTop.LoadEntriesInFolder(rootFolderName);
            SelectFirstTopic();
            return;
        }

        current.SelectEntry(latestFolderEntered);
        latestFolderEntered = TIPSManager.Instance.GetEntry(latestFolderEntered.Parent);
    }

    /// <summary>
    /// Lists entries on the sub folder, used for partial search functionality.
    /// </summary>
    /// <param name="entries"> Entries to be listed. </param>
    public void ShowEntriesOnSub(TIPSEntryData[] entries)
    {
        JumpToLayer(Layer.Sub);
        GetCurrentPanel().LoadEntries(entries);
        latestFolderEntered = null; // enable step out to root folder
    }
}