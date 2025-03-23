using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WordRotator : BattleWord
{
    private const float dimAmount = 0.3f;
    private static readonly Color dimColor = new Color(dimAmount, dimAmount, dimAmount, 0);

    private float goal = 3;
    private float time = 0.6f;
    private Color brightColor;
    private bool skipColor = false;
    private bool pendingFade = false;

    public void Play(string word)
    {
        text.text = word;
        SetHeight(Random.Range(0f, 1f));
    }

    private IEnumerator RunAnimation(float delay)
    {
        yield return new WaitForSeconds(delay / 2);
        skipColor = true;
        text.DOFade(1, delay / 2).OnComplete(() => skipColor = false);
        MoveRight();
    }

    public void MoveRight()
    {
        activeTweens.Clear();
        if (pendingFade)
        {
            DoFade();
        }
        else
        {
            SetSortingOrder(1);
            activeTweens.Add(transform.DOMoveX(goal, time).SetEase(Ease.InOutSine).OnComplete(MoveLeft));
        }
    }

    private void DoFade()
    {
        SetSortingOrder(-6);
        pendingFade = false;
        // Maybe replace with shatter effect?
        text.DOFade(0, 0.75f);
        transform.DOScale(0, 0.75f);
    }

    public void FadePending()
    {
        pendingFade = true;
    }

    public void MoveLeft()
    {
        activeTweens.Clear();
        if (pendingFade)
        {
            DoFade();
            return;
        }
        SetSortingOrder(-3);
        activeTweens.Add(transform.DOMoveX(-goal, time).SetEase(Ease.InOutSine).OnComplete(MoveRight));
        if (!skipColor)
        {
            activeTweens.Add(text.DOColor(brightColor - dimColor, time * 0.75f).OnComplete(() => text.DOColor(brightColor, time * 0.25f)));
        }
    }

    private void SetHeight(float height)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Lerp(-2f, 2.5f, height));
        float scaleMod = Mathf.Lerp(0.25f, 0.95f, height);
        transform.localScale = new Vector3(scaleMod, scaleMod, 1);
        goal = Mathf.Lerp(2f, 5, height);
        brightColor = text.color = Color.Lerp(new Color(0.75f, 0.75f, 0.75f, 0), new Color(1, 1, 1, 0), height);
        brightColor.a = 1;
        time *= Random.Range(0.95f, 1.05f);
        StartCoroutine(RunAnimation(Random.Range(0.1f, (time * 4) + 0.1f)));
    }
}
