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
    PauseHandle ph;
    public PauseHandle PH => ph;
    // Stop target reticule movement and animation.
    public void OnPause(bool pause)
    {
        enabled = !pause;
        GetComponent<Animator>().speed = pause ? 0f : 1f;
    }
    #endregion

    public GameObject scouterDisplay; // Scouter display object.
    public DialogBox scouterDialog; // Scouter text dialog.
    public Image scouterImage; // Scouter image.

    public bool ScouterActive // Is the scouter active (i.e. display is active)?
    {
        get => scouterDisplay.activeSelf;
        set => scouterDisplay.SetActive(value);
    }

    void Awake()
    {
        ph = new PauseHandle(OnPause);
    }

    public float MoveSpeed = 0.5f; // Speed of target reticule movement (when selecting diff targets).
    Vector2 TargetPos // Position of player target.
    {
        get => Battlefield.instance.Player != null
            ? Battlefield.instance.GetSpace(Battlefield.instance.Player.TargetPos)
            : Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Move scouter reticule
        if (!ScouterActive)
            transform.position = Vector2.Lerp(transform.position, TargetPos, MoveSpeed);
        // Toggle scouter
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ScouterActive = !ScouterActive;
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
