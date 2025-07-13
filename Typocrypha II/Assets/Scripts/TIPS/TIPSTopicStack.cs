using UnityEngine;
using DG.Tweening;
using System;

public class TIPSTopicStack : MonoBehaviour
{
    [SerializeField] private RectTransform panelContainer;
    [SerializeField] private TIPSTopicPanel panelTop, panelSub, panelAux;
    [SerializeField] Vector2 auxOffsetPosition = new Vector2(-16, 16);

    public enum Layer { Top = 0, Sub = 1, Aux = 2 };
    public Layer currentLayer = Layer.Top;

    public Action<TIPSEntryData> OnButtonSelected;

    private TIPSTopicPanel[] _panels;
    private TIPSTopicPanel[] panels => _panels != null ? _panels : _panels = new TIPSTopicPanel[] { panelTop, panelSub, panelAux };

    private TIPSEntryData latestFolderEntered;

    public TIPSTopicPanel GetPanel(Layer layer) => panels[(int)layer];
    public TIPSTopicPanel GetCurrentPanel() => GetPanel(currentLayer);

    private void Start()
    {
        foreach (var p in panels)
        {
            p.OnButtonPressed += StepIntoEntry;
            p.OnButtonSelected += OnButtonSelected;
        }
        panelTop.LoadEntriesInFolder("Root");
    }

    private void OnDestroy()
    {
        foreach (var p in panels)
        {
            p.OnButtonPressed -= StepIntoEntry;
            p.OnButtonSelected -= OnButtonSelected;
        }
    }

    public Sequence JumpToLayer(Layer target, float duration = 0.33f)
    {
        const float full = 1, near = 0.4f, far = 0.2f, none = 0f;

        var topFade = target == Layer.Top ? full : target == Layer.Sub ? near : far;
        var subFade = target == Layer.Top ? none : target == Layer.Sub ? full : near;
        var auxFade = target == Layer.Aux ? full : none;

        currentLayer = target;

        return DOTween.Sequence()
            .Join(panelTop.DOFade(topFade, duration))
            .Join(panelSub.DOFade(subFade, duration))
            .Join(panelAux.DOFade(auxFade, duration))
            .Join(panelContainer.DOAnchorPos(auxOffsetPosition * (int)target, duration));
    }

    public void StepToLayer(Layer target, float duration = 0.33f)
    {
        if (target < Layer.Top || target > Layer.Aux) return;

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
        current.LoadEntriesInFolder(entry.Title);
        current.SelectTopicPageTop();
        latestFolderEntered = entry;
    }

    public void JumpToEntry(TIPSEntryData entry)
    {
        JumpToLayer((Layer)entry.Depth);
        var current = GetCurrentPanel();
        current.LoadEntriesInFolder(entry.Parent);
        current.SelectEntry(entry.Title);

        if (currentLayer == Layer.Aux)
        {
            panelSub.LoadEntriesInFolder(entry.Categorization[(int)Layer.Sub]);
        }

        latestFolderEntered = TIPSManager.Instance.GetEntry(entry.Parent);
    }

    public void StepOutToParent()
    {
        if (currentLayer == Layer.Top) return;

        StepToPreviousLayer();
        var current = GetCurrentPanel();
        current.LoadEntriesInFolder(latestFolderEntered.Parent);
        current.SelectEntry(latestFolderEntered.Title);
        latestFolderEntered = TIPSManager.Instance.GetEntry(latestFolderEntered.Parent);
    }

//#if UNITY_EDITOR

//    [ContextMenu("Navigate Top")]
//    public void NavigateTop()
//    {
//        JumpToLayer(Layer.Top);
//    }

//    [ContextMenu("Navigate Sub")]
//    public void NavigateSub()
//    {
//        JumpToLayer(Layer.Sub);
//    }

//    [ContextMenu("Navigate Aux")]
//    public void NavigateAux()
//    {
//        JumpToLayer(Layer.Aux);
//    }

//#endif

}