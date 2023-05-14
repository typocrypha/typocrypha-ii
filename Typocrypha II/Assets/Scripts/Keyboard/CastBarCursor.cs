using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CastBarCursor : MonoBehaviour, IPausable
{
    public PauseHandle PH { get; private set; }
    [SerializeField] private TextMeshProUGUI cursorText;

    private int index = 0;
    private bool showing = true;
    public bool Showing
    {
        get => showing;
        set
        {
            showing = value;
            cursorText.color = showing ? Color.white : Color.clear;
        }
    }

    public void OnPause(bool b)
    {
        enabled = Showing = !b;
    }

    private void Awake()
    {
        PH = new PauseHandle(OnPause);
    }

    private void FixedUpdate()
    {
        if(++index >= 30)
        {
            Showing = !Showing;
            index = 0;
        }
    }

    public void SetDelay(float time, bool show)
    {
        index = -Mathf.RoundToInt(time / Time.fixedDeltaTime);
        Showing = show;
    }
}
