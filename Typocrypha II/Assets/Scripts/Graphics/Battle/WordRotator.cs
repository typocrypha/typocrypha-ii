using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WordRotator : MonoBehaviour, IInputHandler
{
    private const float dimAmount = 0.3f;
    private static readonly Color dimColor = new Color(dimAmount, dimAmount, dimAmount, 0);
    [SerializeField] private TMPro.TextMeshPro text;
    [SerializeField] private FXText.TMProColor colorEffect;
    [SerializeField] private FXText.TMProShake shakeEffect;
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip failClip;

    private float goal = 3;
    private float time = 0.6f;
    private Color brightColor;
    private bool skipColor = false;
    private bool pendingFocus = false;
    private float focusTime = 2f;

    public PauseHandle PH { get; } = new PauseHandle();

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
        if (pendingFocus)
        {
            text.renderer.sortingOrder = 2;
            float centerTime = 0.5f;
            transform.DOMove(new Vector2(0, 1) + (new Vector2(2.5f, 0.5f) * Random.insideUnitCircle), centerTime).SetEase(Ease.InOutCubic);
            transform.DOScale(new Vector2(2, 2), centerTime).SetEase(Ease.InOutCubic);
            text.DOColor(new Color(1, 0.6666667f, 0, 1), centerTime).OnComplete(OnFocused);
        }
        else
        {
            text.renderer.sortingOrder = 1;
            transform.DOMoveX(goal, time).SetEase(Ease.InOutSine).OnComplete(MoveLeft);
        }
    }

    public void FocusPending(float timePerLetter)
    {
        pendingFocus = true;
        focusTime = text.text.Length * timePerLetter;
    }

    private void OnFocused()
    {
        StartCoroutine(FocusedCR(focusTime));
    }

    private IEnumerator FocusedCR(float timeAllowed)
    {
        colorEffect.enabled = true;
        shakeEffect.enabled = true;
        InputManager.Instance.StartInput(this);
        var pause = Typocrypha.Keyboard.instance.PH.UnpauseOverride();
        float time = 0;
        var endOfFrameYielder = new WaitForEndOfFrame();
        bool success = false;
        while (time < timeAllowed)
        {
            if(index >= text.text.Length)
            {
                success = true;
                break;
            }
            yield return endOfFrameYielder;
            time += Time.deltaTime / Settings.GameplaySpeed;
        }
        shakeEffect.done = true;
        InputManager.Instance.CompleteInput();
        Typocrypha.Keyboard.instance.PH.Pause(pause);
        if (success)
        {
            AudioManager.instance.PlaySFX(successClip);
        }
        else
        {
            AudioManager.instance.PlaySFX(failClip);
            Battlefield.instance.Player.Damage(10);
            SpellFxManager.instance.PlayDamageNumber(10, Battlefield.instance.Player);
        }
        colorEffect.done = true;
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

    public void Focus() { }

    public void Unfocus() { }

    private int index;
    public bool? CheckInput(char inputChar)
    {
        if(char.ToLower(inputChar) == char.ToLower(text.text[index]))
        {
            index++;
            colorEffect.ind[1] = index;
            shakeEffect.ind[0]++;
            shakeEffect.ind[1]++;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Submit()
    {
        return;
    }

    public void Clear()
    {
        return;
    }


}
