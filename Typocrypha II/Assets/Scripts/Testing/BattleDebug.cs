using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDebug : MonoBehaviour
{
#if !DEBUG
    private void Awake()
    {
        Destroy(gameObject);
    }
#else
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && !DialogManager.instance.PH.Pause)
        {
            DialogManager.instance.StopAllCoroutines();
            DialogManager.instance.Hide(true, DialogManager.instance.CleanUp);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            BattleManager.instance.StopAllCoroutines();
            BattleManager.instance.NextWave();
        }
    }
#endif
}
