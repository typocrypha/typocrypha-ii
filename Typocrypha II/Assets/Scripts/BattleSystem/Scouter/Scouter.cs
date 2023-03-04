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
        if (Input.GetKeyDown(KeyCode.LeftShift) && (!Battlefield.instance.PH.Pause || ScouterActive))
        {
            ScouterActive = !ScouterActive;
            targetReticle.PH.Pause = ScouterActive;
            Battlefield.instance.PH.Pause = ScouterActive;
            if (ScouterActive)
            {
                var obj = Battlefield.instance.GetObject(Battlefield.instance.Player.TargetPos);
                scouterDialog.StartDialogBox(obj?.GetScouterInfo().DescriptionText);
                scouterImage.sprite = obj?.GetScouterInfo().DisplayImage;
            }
        }
    }

    /// <summary>
    /// Get scouter data from database.
    /// </summary>
    /// <param name="key">Name of data file.</param>
    /// <returns>Scouter data object.</returns>
    public static ScouterData GetScouterData(string key)
    {
        var bundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, "scouterdata"));
        var data = bundle.LoadAsset<ScouterData>(key);
        bundle.Unload(false);
        return data;
    }
}
