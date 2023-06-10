using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TargetReticle : MonoBehaviour, IPausable
{
    #region IPausable
    public PauseHandle PH { get; private set; }
    // Stop target reticule movement and animation.
    public void OnPause(bool pause)
    {
        enabled = !pause;
        if (Battlefield.instance.Player is Player player)
        {
            player.PH.Pause = pause;
        }
        //gameObject.SetActive(!pause);
        if (pause)
        {
            enableDisableTween.Start(rectTr.DOSizeDelta(new Vector2(0, 0), enableDisableTween.Time));
            //enableDisableTween.Start(reticleImage.DOFade(0, enableDisableTween.Time), false);
        }
        else
        {
            enableDisableTween.Start(rectTr.DOSizeDelta(new Vector2(500, 500), enableDisableTween.Time));
            //enableDisableTween.Start(reticleImage.DOFade(1, enableDisableTween.Time), false);
        }
    }
    #endregion

    [SerializeField] private RectTransform rectTr;
    [SerializeField] private Image reticleImage;
    [SerializeField] private TweenInfo enableDisableTween;

    void Awake()
    {
        PH = new PauseHandle(OnPause);
        PH.SetParent(BattleManager.instance.PH);
    }

    void Start()
    {
        // Initialize position
        lastTargetPos = new Battlefield.Position(0, 1);
        targetPosScreenspace = Battlefield.instance.GetSpaceScreenSpace(lastTargetPos);
        transform.position = targetPosScreenspace;
    }

    public float MoveSpeed = 0.5f; // Speed of target reticule movement (when selecting diff targets).
    private Battlefield.Position lastTargetPos;
    private Vector2 targetPosScreenspace;

    // Update is called once per frame
    void Update()
    {
        // Move scouter reticule
        SetTargetPos();
        if(transform.position.x != targetPosScreenspace.x || transform.position.y != targetPosScreenspace.y)
        {
            transform.position = Vector2.Lerp(transform.position, targetPosScreenspace, MoveSpeed);
        }
    }

    protected void SetTargetPos()
    {
        if (Battlefield.instance.Player == null)
            return;
        var targetFieldPos = Battlefield.instance.Player.TargetPos;
        if (lastTargetPos == targetFieldPos)
            return;
        lastTargetPos = new Battlefield.Position(targetFieldPos);
        targetPosScreenspace = Battlefield.instance.GetSpaceScreenSpace(targetFieldPos);
    }
}
