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

    [MenuItem("Tools/Save Data/Unlock/Unlock All Shop Codes")]
    public static void UnlockAllShopCodes()
    {
        UnlockAllShopCodesForFile(SaveManager.SaveFilePath(0));
        UnlockAllShopCodesForFile(SaveManager.SaveFilePath(SaveManager.debugSaveFile));
    }

    private static void UnlockAllShopCodesForFile(string path)
    {
        var file = SaveManager.LoadFile<CampaignSaveData>(path);
        if (file == null)
            return;
        file.shopUnlockCodes = (int)BadgeWord.ShopUnlockCodes.All;
        SaveManager.SaveFile(file, path);
    }

    [MenuItem("Tools/Save Data/Unlock/Set $ To 100000")]
    public static void SetMoneyTo100000()
    {
        SetMoneyTo100000ForFile(SaveManager.SaveFilePath(0));
        SetMoneyTo100000ForFile(SaveManager.SaveFilePath(SaveManager.debugSaveFile));
    }

    private static void SetMoneyTo100000ForFile(string path)
    {
        var file = SaveManager.LoadFile<CampaignSaveData>(path);
        if (file == null)
            return;
        file.currency = 100000;
        SaveManager.SaveFile(file, path);
    }
}
