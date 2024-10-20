using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WordRotator : MonoBehaviour
{
    private const float dimAmount = 0.3f;
    private static readonly Color dimColor = new Color(dimAmount, dimAmount, dimAmount, 0);
    [SerializeField] private TMPro.TextMeshPro text;

    private float goal = 3;
    private float time = 0.6f;
    private Color brightColor;
    private bool skipColor = false;

    public bool Pending = false;

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
        if (Pending)
        {
            text.renderer.sortingOrder = 2;
            float centerTime = 0.5f;
            transform.DOMove(new Vector2(0, 1), centerTime).SetEase(Ease.InOutCubic);
            transform.DOScale(new Vector2(2, 2), centerTime).SetEase(Ease.InOutCubic);
            text.DOColor(new Color(1, 0.6666667f, 0, 1), centerTime).OnComplete(HideAfterTime);
        }
        else
        {
            text.renderer.sortingOrder = 1;
            transform.DOMoveX(goal, time).SetEase(Ease.InOutSine).OnComplete(MoveLeft);
        }
    }

    private void HideAfterTime()
    {
        StartCoroutine(HideAfterTimeCR());
    }

    private IEnumerator HideAfterTimeCR()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    public void MoveLeft()
    {
        text.renderer.sortingOrder = 0;
        transform.DOMoveX(-goal, time).SetEase(Ease.InOutSine).OnComplete(MoveRight);
        if (!skipColor)
        {
            text.DOColor(brightColor - dimColor, time * 0.75f).OnComplete(() => text.DOColor(brightColor, time * 0.25f));
        }
    }

    private void SetHeight(float height)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Lerp(-1f, 2.5f, height));
        float scaleMod = Mathf.Lerp(0.2f, 0.95f, height);
        transform.localScale = new Vector3(scaleMod, scaleMod, 1);
        goal = Mathf.Lerp(1.5f, 4, height);
        brightColor = text.color = Color.Lerp(new Color(0.75f, 0.75f, 0.75f, 0), new Color(1, 1, 1, 0), height);
        brightColor.a = 1;
        time *= Random.Range(0.95f, 1.05f);
        StartCoroutine(RunAnimation(Random.Range(0.1f, (time * 4) + 0.1f)));
    }
}
