using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWord : MonoBehaviour, IInputHandler
{
    public enum FocusPosition
    {
        LeftFar,
        LeftClose,
        Center,
        RightClose,
        RightFar,
    }
    private const float centerTime = 0.5f;
    private static readonly Vector2[] focusPositions = new Vector2[]
    {
        new Vector2(-5, 2.25f),
        new Vector2(-3f, 1.25f),
        new Vector2(0, 0.25f),
        new Vector2(3f, 1.25f),
        new Vector2(5, 2.25f),
    };
    public static Vector2 GetFocusPosition(FocusPosition position)
    {
        int index = (int)position;
        if (index < 0 || index >= focusPositions.Length)
            return Vector2.zero;
        return focusPositions[index];
    }

    [SerializeField] protected TMPro.TextMeshPro text;
    [SerializeField] protected FXText.TMProColor colorEffect;
    [SerializeField] protected FXText.TMProShake shakeEffect;

    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip failClip;
    [SerializeField] private AudioClip focusClip;
    [SerializeField] private AnimationCurve scaleEasing;

    protected readonly List<Tween> activeTweens = new List<Tween>();

    public event System.Action OnComplete;
    public PauseHandle PH { get; } = new PauseHandle();

    public bool PendingFocus { get; private set; } = false;
    private float focusTime = 2f;

    public void Focus() { }

    public void Unfocus() { }

    public void SetText(string word)
    {
        text.text = word;
    }

    public void SetTarget()
    {
        colorEffect.enabled = true;
        shakeEffect.enabled = true;
        InputManager.Instance.StartInput(this);
        var pause = Typocrypha.Keyboard.instance.PH.UnpauseOverride();
        void TimerComplete()
        {
            InputManager.Instance.CompleteInput();
            Typocrypha.Keyboard.instance.PH.Pause(pause);
        }
        OnComplete += TimerComplete;
    }

    public void Focus(float focusTime, Vector2 focusPosition, bool activeWord, int wordIndex)
    {
        foreach (var tween in activeTweens)
        {
            tween.Kill();
        }
        PendingFocus = true;
        this.focusTime = focusTime;
        SetSortingOrder(2 + wordIndex);
        transform.DOMove(focusPosition, centerTime).SetEase(Ease.InOutCubic);
        if (activeWord)
        {
            text.DOColor(new Color(1, 0.6666667f, 0, 1), 0.1f).OnComplete(SetTarget);
        }
        else
        {
            text.DOColor(Color.white, 0.1f);
        }
        transform.DOScale(new Vector2(2, 2), centerTime).SetEase(Ease.InOutCubic).OnComplete(OnFocused);
        AudioManager.instance.PlaySFX(focusClip);
    }

    private void OnFocused()
    {
        PendingFocus = false;
        StartCoroutine(FocusedCR(focusTime));
    }

    private bool failed = false;

    private IEnumerator FocusedCR(float timeAllowed)
    {
        float time = 0;
        var endOfFrameYielder = new WaitForEndOfFrame();
        bool success = false;
        while (time < timeAllowed)
        {
            if (failed)
            {
                failed = false;
                var originalColor = colorEffect.defaultColor;
                colorEffect.defaultColor = Color.red;
                DOTween.To(() => colorEffect.defaultColor, (c) => colorEffect.defaultColor = c, originalColor, 0.2f).SetEase(Ease.InQuad).Play();
                time += (0.33f * Settings.GameplaySpeed);
                AudioManager.instance.PlaySFX(failClip);
            }
            if (index >= text.text.Length)
            {
                success = true;
                break;
            }
            yield return endOfFrameYielder;
            time += Time.deltaTime / Settings.GameplaySpeed;
            float scaleFactor = Mathf.Lerp(2, 3.5f, scaleEasing.Evaluate(time / timeAllowed));
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }
        shakeEffect.done = true;
        if (success)
        {
            AudioManager.instance.PlaySFX(successClip);
        }
        else
        {
            AudioManager.instance.PlaySFX(failClip);
            colorEffect.defaultColor = Color.red;
            float attackTime = 0.25f;
            transform.DOMove(Battlefield.instance.Player.transform.position + new Vector3(0, 0.5f), attackTime).SetEase(Ease.InOutCubic);
            transform.DOScale(Vector2.zero, attackTime).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(attackTime);
            Battlefield.instance.Player.Damage(12);
            SpellFxManager.instance.PlayDamageNumber(12, Battlefield.instance.Player);
        }
        colorEffect.done = true;
        gameObject.SetActive(false);
        OnComplete?.Invoke();
    }

    private int index;
    public bool? CheckInput(char inputChar)
    {
        if (index >= text.text.Length)
            return null;
        if (char.ToLower(inputChar) == char.ToLower(text.text[index]))
        {
            index++;
            colorEffect.ind[1] = index;
            shakeEffect.ind[0]++;
            shakeEffect.ind[1]++;
            return true;
        }
        else
        {
            failed = true;
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

    protected void SetSortingOrder(int order)
    {
        text.renderer.sortingOrder = order;
    }

    [System.Serializable]
    public class WordData
    {
        public FocusPosition position;
        public GameObject prefabOverride;
        public string text;
    }

    [System.Serializable]
    public class SequenceData
    {
        public List<WordData> words;
    }
}
