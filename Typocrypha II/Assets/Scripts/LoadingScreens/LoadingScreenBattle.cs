using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dialog to Battle (or back) transition.
/// </summary>
public class LoadingScreenBattle : LoadingScreenDefault
{
    public float _SwirlAmount;
    public float SwirlAmount // Amount of swirl.
    {
        set
        {
            PostProcess.postProcessMap["swirl_screen_mat"].shaderParams["_SwirlAmount"] = value;
        }
    }
    PostProcess swirl; // Swirling post process effect.

    void Start()
    {
        SwirlAmount = 0f;
    }

    void Update()
    {
        SwirlAmount = _SwirlAmount;
    }
}
