using UnityEngine;
using TMPro;
using DG.Tweening;

public class NametagScouterAnimator : MonoBehaviour
{
    [SerializeField] RectTransform VerticalOffset;
    [SerializeField] TextMeshProUGUI text;
    const float TWEEN_DURATION = 0.25f;
    const float TWEEN_DELAY = 0.25f;

    public void ShowScouterData()
    {
        VerticalOffset.DOAnchorPosY(text.preferredHeight, TWEEN_DURATION).SetDelay(TWEEN_DELAY);
    }

    public void HideScouterData()
    {
        VerticalOffset.DOAnchorPosY(0, TWEEN_DURATION);
    }
}
