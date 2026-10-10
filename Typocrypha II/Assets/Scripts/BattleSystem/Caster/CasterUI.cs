using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

/// <summary>
/// Manages UI for a caster.
/// </summary>
public class CasterUI : MonoBehaviour
{
    public UnityEvent_float onHealthChanged; // Pass percentage health.
    public UnityEvent_string onHealthChangedNumber; // Pass absolute health as string.
    public UnityEvent_float onSpChanged; // Pass percentage SP
    public UnityEvent_float onChargeChanged; // Pass percentage charge.
    public UnityEvent_string onChargeChangedNumber; // Pass absolute charge / MP as string.
    public UnityEvent_string onStaggerChanged; // Pass absolute stagger as string.
    public UnityEvent_string onNameChanged; // Pass name as string
    public UnityEvent onStun; // Call when stunned.
    public UnityEvent onUnstun;
    public UnityEvent_float onStunProgressChanged;
    public UnityEvent onSpiritForm; // Call when entering spirit form
    public UnityEvent_string onSpellChanged; // Pass name of spell currently being cast.
    public UnityEvent_sprite onSpellIconChanged; // Pass icon of spell current being cast.
    public UnityEvent_sprite onSpriteChanged; // Pass new sprite
    public UnityEvent_bool onCounterStateChanged;
    public UnityEvent_string onScouterDataChanged;
    public UnityEvent onScouterShow;
    public UnityEvent onScouterHide;
    public UnityEvent onDamageReceived;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite spiritSprite;
    [SerializeField] private CanvasGroup ui;
    [SerializeField] private HighlightCounterable highlightCounterable;
    [SerializeField] private int numParticles;
    [SerializeField] private GameObject particlePrefab;

    private readonly List<SpriteRenderer> dynamicSprites = new List<SpriteRenderer>();

    public void SetTextColor(Color color)
    {
        highlightCounterable.SetDefaultColor(color);
    }

    public void SetDimmable(bool dimmable)
    {
        int order = dimmable ? -1 : 1;
        if (sprite)
        {
            sprite.sortingOrder = order;
        }
        SyncSortingOrders(order);
    }

    private void SyncSortingOrders(int order)
    {
        foreach (var sr in dynamicSprites)
        {
            sr.sortingOrder = order;
        }
    }

    public void ShowUI(bool show)
    {
        if (ui) ui.alpha = show ? 1 : 0;
    }

    private const float t1 = 0.75f;
    private const float t2 = t1 * 0.75f;
    private const float staggerTime = 0.025f * 25;
    public void PlaySpiritModeAnimation()
    {
        float staggerScaled = staggerTime / numParticles;
        for (int i = 0; i < numParticles; i++)
        {
            var target = (Vector2)sprite.transform.position + (Random.onUnitSphere * new Vector2(2, 2));
            var particle = Instantiate(particlePrefab, sprite.transform);
            var particleSprite = particle.GetComponent<SpriteRenderer>();
            dynamicSprites.Add(particleSprite);
            var sequence = DOTween.Sequence();
            sequence.Append(particle.transform.DOMove(target, t1 * 0.5f).SetEase(Ease.OutQuad));
            sequence.Join(particleSprite.DOFade(0.75f, t1 * 0.25f));
            sequence.AppendInterval(staggerScaled * i);
            sequence.Append(particle.transform.DOMove(sprite.transform.position, t2).SetEase(Ease.InOutExpo));
            sequence.Join(particleSprite.DOFade(0, t2 * 0.25f).SetDelay(t2 / 2));
            dynamicSprites.Add(particleSprite);
        }
        SyncSortingOrders(sprite.sortingOrder);
    }
    public void SwapToSpiritSprite()
    {
        sprite.sprite = spiritSprite;
    }
}
