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

    public static TargetReticle instance;
    [SerializeField] private RectTransform rectTr;
    [SerializeField] private Image reticleImage;
    [SerializeField] private TweenInfo enableDisableTween;
    private Battlefield.Position TargetPos { get; } = new Battlefield.Position(0, 1);

    void Awake()
    {
        PH = new PauseHandle(OnPause);
        if (instance == null) instance = this;
        else Destroy(gameObject);
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
        if (Input.GetKeyDown(KeyCode.LeftArrow) && TargetPos.Col > 0)
        {
            TargetPos.Col--;
            UpdateTargetPos();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && TargetPos.Col < (Battlefield.instance.Columns - 1))
        {
            TargetPos.Col++;
            UpdateTargetPos();
        }

        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //    TargetPos.Row = 0;
        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //    TargetPos.Row = 1;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            var field = Battlefield.instance;
            var newPos = new Battlefield.Position(TargetPos);
            do
            {
                ++newPos.Col;
                if (newPos.Col >= field.Columns)
                    newPos.Col = 0;
                var caster = field.GetCaster(newPos);
                if (caster != null && !caster.IsDeadOrFled)
                    break;
            }
            while (newPos.Col != TargetPos.Col);
            TargetPos.Col = newPos.Col;
            UpdateTargetPos();
        }
        // Move scouter reticule
        UpdateTargetPos();
        if (transform.position.x != targetPosScreenspace.x || transform.position.y != targetPosScreenspace.y)
        {
            transform.position = Vector2.Lerp(transform.position, targetPosScreenspace, MoveSpeed);
        }
    }

    protected void UpdateTargetPos()
    {
        if (Battlefield.instance.Player == null)
            return;
        if (lastTargetPos == TargetPos)
            return;
        Battlefield.instance.Player.TargetPos = TargetPos;
        lastTargetPos = new Battlefield.Position(TargetPos);
        targetPosScreenspace = Battlefield.instance.GetSpaceScreenSpace(TargetPos);
    }
}
