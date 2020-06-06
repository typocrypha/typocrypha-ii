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

    void Start()
    {
        SwirlAmount = 0f;
    }

    void Update()
    {
        PostProcess.postProcessMap["swirl_screen_mat"].shaderParams["_Width"] = Screen.width;
        PostProcess.postProcessMap["swirl_screen_mat"].shaderParams["_Height"] = Screen.height;
        SwirlAmount = _SwirlAmount;
    }
}
