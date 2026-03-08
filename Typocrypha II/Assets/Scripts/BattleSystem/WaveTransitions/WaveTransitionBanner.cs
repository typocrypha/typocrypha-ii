using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveTransitionBanner : MonoBehaviour, IPausable
{
    public PauseHandle PH { get; private set; }

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] private Animator animationController;

    public string TitleText
    {
        get => titleText.text;
        set => titleText.text = value;
    }

    public string NumberText
    {
        get => numberText.text;
        set => numberText.text = value;
    }
    private void Awake()
    {
        PH = new PauseHandle();
    }

    public void ContinueAnimation()
    {
        animationController.SetTrigger("KeyDownSpace");
    }
}
