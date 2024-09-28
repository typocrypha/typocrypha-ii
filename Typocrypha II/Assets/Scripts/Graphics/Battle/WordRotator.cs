using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WordRotator : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshPro text;

    private float goal = 3;
    private float time = 0.6f;

    public void Play(string word)
    {
        text.text = word;
        SetHeight(Random.Range(0f, 1f));
    }

    private IEnumerator RunAnimation(float delay)
    {
        yield return new WaitForSeconds(delay / 2);
        text.DOFade(1, delay / 2);
        MoveRight();
    }

    public void MoveRight()
    {
        text.renderer.sortingOrder = 1;
        transform.DOMoveX(goal, time).SetEase(Ease.InOutSine).OnComplete(MoveLeft);
    }

    public void MoveLeft()
    {
        text.renderer.sortingOrder = 0;
        transform.DOMoveX(-goal, time).SetEase(Ease.InOutSine).OnComplete(MoveRight);
    }

    private void SetHeight(float height)
    {

        transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Lerp(-1f, 2.5f, height));
        float scaleMod = Mathf.Lerp(0.2f, 0.9f, height);
        transform.localScale = new Vector3(scaleMod, scaleMod, 1);
        goal = Mathf.Lerp(1.5f, 4, height);
        text.color = Color.Lerp(new Color(0.75f, 0.75f, 0.75f, 0), new Color(1, 1, 1, 0), height);
        StartCoroutine(RunAnimation(Random.Range(0.1f, 2)));
    }
}
