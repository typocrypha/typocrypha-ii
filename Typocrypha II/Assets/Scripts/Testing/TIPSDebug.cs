#if UNITY_EDITOR
using UnityEngine;

public class TIPSDebug : MonoBehaviour
{
    private void Start()
    {
        var manager = TIPSManager.Instance;
        
        foreach (var entryName in manager.allTIPS.entries.Keys)
        {
            manager.UnlockEntryIfApplicable(entryName);
        }
    }
}

#endif