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
    public void Play(SpellWord word, CastResults results, TweenCallback onComplete = null)
    {
        text.text = word.DisplayName;
        var sequence = DOTween.Sequence();
        if (results.caster is Player)
        {
            transform.position = Battlefield.instance.GetSpaceScreenSpace(results.target.FieldPos);
        }
        else
        {
            transform.position = Battlefield.instance.GetSpaceScreenSpace(results.caster.FieldPos);
            var endPos = Battlefield.instance.GetSpaceScreenSpace(results.target.FieldPos);
            sequence.Join(transform.DOMove(endPos, moveTime));
        }
        transform.localScale = new Vector3(startingScale, startingScale);
        sequence.Join(transform.DOScale(new Vector3(endingScale, endingScale), moveTime).SetEase(scaleEase));
        if (onComplete != null)
        {
            sequence.onComplete = onComplete;
        }
        sequence.Play();
    }
}
