using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WordFx : MonoBehaviour
{
    private const float moveTime = 0.4f;
    private const float startingScale = 1.1f;
    private const float endingScale = 0.33f;
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private AnimationCurve scaleEase;
    public void Play(SpellWord word, Caster caster, Battlefield.Position targetPosition, WordFxDefinition.AnimType animType, TweenCallback onComplete = null)
    {
        if(animType == WordFxDefinition.AnimType.None)
        {
            onComplete?.Invoke();
            return;
        }
        text.text = word.DisplayName;
        var sequence = DOTween.Sequence();
        if (caster is Player)
        {
            transform.position = Battlefield.instance.GetSpaceScreenSpace(targetPosition);
            transform.localScale = new Vector3(startingScale, startingScale);
            sequence.Append(transform.DOScale(new Vector3(endingScale, endingScale), moveTime).SetEase(scaleEase));
        }
        else
        {
            transform.position = Battlefield.instance.GetSpaceScreenSpace(caster.FieldPos);
            transform.localScale = new Vector3(0.33f, 0.33f);
            var endPos = Battlefield.instance.GetSpaceScreenSpace(targetPosition);
            sequence.Append(transform.DOMove(endPos, moveTime).SetEase(Ease.OutQuart));
            sequence.Join(transform.DOPunchScale(new Vector3(1.1f, 1.1f), moveTime, 0, 0).SetEase(Ease.OutQuart));
        }
        if (onComplete != null)
        {
            sequence.onComplete = onComplete;
        }
        sequence.Play();
    }
}
