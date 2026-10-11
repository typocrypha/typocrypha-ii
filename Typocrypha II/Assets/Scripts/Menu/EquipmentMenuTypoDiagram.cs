using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class EquipmentMenuTypoDiagram : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private RectTransform typoRT;
    [SerializeField] private List<BadgeUI> badges;
    [SerializeField] private GameObject selectionParent;
    [SerializeField] private UICircle selectionCircle;
    [SerializeField] private UILineRenderer selectionLine;
    [SerializeField] private UILineConnector connector;
    [SerializeField] private RectTransform slotSelectWindow;
    [SerializeField] private RectTransform circleRT;
    [SerializeField] private RectTransform lineStart;
    [SerializeField] private RectTransform lineMid;
    [SerializeField] private RectTransform lineEnd;

    [Header("Animation References")]
    [SerializeField] private Vector2 unselectedPos;
    [SerializeField] private Vector2 selectedPos;
    

    [System.Serializable]
    private class BadgeUI
    {
        public BadgeWord.EquipmentSlot SlotType;
        public Image Image;
        public RectTransform RT;
    }

    public void Deselect()
    {
        selectionParent.SetActive(false);
        AnimateToPosition(unselectedPos);
    }

    public void Select(BadgeWord.EquipmentSlot slotType)
    {
        var selectedBadge = badges.Find(badge => badge.SlotType == slotType);
        if (selectedBadge == null)
        {
            Debug.Log($"Couldn't find slot for type {slotType}");
            Deselect();
            return;
        }

        // Copy over world position of the unrelated slot select window
        lineStart.transform.position = slotSelectWindow.position;

        AnimateToPosition(selectedPos);
        AnimateSelectedBadgePos(selectedBadge.RT);
    }

    private void AnimateToPosition(Vector2 pos)
    {
        typoRT.DOAnchorPos(pos, 0.5f).SetEase(Ease.InOutBack);
    }

    public void AnimateSelectedBadgePos(RectTransform selectedBadgeRT)
    {
        DOTween.Kill("BadgeSelect");

        var seq = DOTween.Sequence();
        // Reset everything
        lineStart.position = slotSelectWindow.position;
        lineMid.position = slotSelectWindow.position;
        lineEnd.position = slotSelectWindow.position;
        circleRT.position = selectedBadgeRT.position;
        selectionCircle.Arc = 0;
        seq.AppendCallback(() => selectionLine.SetAllDirty());
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            selectionParent.SetActive(true);
            selectionLine.SetAllDirty();
        });

        // Animate the middle and final targets to the middle
        Vector2 endPoint = new Vector2(circleRT.anchoredPosition.x - (circleRT.sizeDelta.x / 2), circleRT.anchoredPosition.y);
        Vector2 midPoint = new Vector2(endPoint.x / 3, endPoint.y / 2);
        seq.Append(lineMid.DOAnchorPos(midPoint, 0.2f).SetEase(Ease.Linear));
        seq.Join(lineEnd.DOAnchorPos(midPoint, 0.2f).SetEase(Ease.Linear));

        // Animate the final target to position
        seq.Append(lineEnd.DOAnchorPos(endPoint, 0.15f).SetEase(Ease.Linear));

        // Animate the circle
        float circleArc = 0;
        seq.Join(DOTween.To(() => circleArc,
            x => {
                circleArc = x;
                selectionCircle.Arc = x;
                selectionCircle.SetAllDirty();
            }, 1f, 0.2f).SetDelay(0.15f));
        seq.SetEase(Ease.InOutSine);

        seq.SetId("BadgeSelect");
    }
}
