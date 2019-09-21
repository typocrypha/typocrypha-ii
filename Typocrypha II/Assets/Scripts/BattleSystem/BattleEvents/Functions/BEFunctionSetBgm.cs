using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEFunctionSetBgm : BattleEventFunction
{
    public AudioClip bgm;
    public override void Run()
    {
        AudioManager.instance.PlayBGM(bgm);
    }
}
