using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Target reticule and scouter during battle.
/// </summary>
public class Scouter : MonoBehaviour, IPausable
{
    #region IPausable
    public PauseHandle PH { get; private set; }
    // Stop target reticule movement and animation.
    public void OnPause(bool pause)
    {
        enabled = !pause;
    }
    #endregion

    public GameObject scouterDisplay; // Scouter display object.
    public DialogBox scouterDialog; // Scouter text dialog.
    public Image scouterImage; // Scouter image.
    [SerializeField] private TargetReticle targetReticle;

    public bool ScouterActive // Is the scouter active (i.e. display is active)?
    {
        get => scouterDisplay.activeSelf;
        set => scouterDisplay.SetActive(value);
    }

    void Awake()
    {
        PH = new PauseHandle(OnPause);
    }

    void Start()
    {
        PH.SetParent(BattleManager.instance.PH);
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle scouter
        if (CheckInput() && (!Battlefield.instance.PH.Pause || ScouterActive))
        {
            ToggleScouter();
        }
    }

    private bool CheckInput()
    {
        return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
    }
    private void ToggleScouter()
    {
        if (ScouterActive)
        {
            ScouterActive = false;
            targetReticle.PH.Pause = false;
            Battlefield.instance.PH.Pause = false;
            Typocrypha.Keyboard.instance.PH.Pause = false;
            return;
        }
        var target = Battlefield.instance.GetCaster(Battlefield.instance.Player.TargetPos);
        if(target == null || target.ScouterData == null)
        {
            return;
        }

        targetReticle.PH.Pause = true;
        Battlefield.instance.PH.Pause = true;
        Typocrypha.Keyboard.instance.PH.Pause = true;
        ScouterActive = true;
        scouterDialog.StartDialogBox(target.ScouterData.Description);
        scouterImage.sprite = target.ScouterData.Image;
    }
}
