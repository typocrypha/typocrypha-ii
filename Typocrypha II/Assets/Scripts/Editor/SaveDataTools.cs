using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public static class SaveDataTools
{
    [MenuItem("Tools/Save Data/Reset Campaign Save Data")]
    public static void ResetCampaignSaveData()
    {
        FileUtil.DeleteFileOrDirectory(SaveManager.SaveFilePath(0));
        FileUtil.DeleteFileOrDirectory(SaveManager.SaveFilePath(SaveManager.debugSaveFile));
    }

    [MenuItem("Tools/Save Data/Reset Global Save Data")]
    public static void ResetGlobalSaveData()
    {
        FileUtil.DeleteFileOrDirectory(SaveManager.GlobalSaveFilePath());
    }

    [MenuItem("Tools/Save Data/Reset All Save Data")]
    public static void ResetAllSaveData()
    {
        ResetCampaignSaveData();
        ResetGlobalSaveData();
    }
}
