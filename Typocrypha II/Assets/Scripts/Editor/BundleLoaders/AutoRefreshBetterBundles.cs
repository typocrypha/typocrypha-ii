using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public static class AutoRefreshBetterBundles
{
    private const string MenuName = "Assets/Toggle Auto-Refresh BetterBundles";
    private const string EditorPrefsKey = "AutoRefreshBetterBundles";

    public static bool IsEnabled
    {
        get { return EditorPrefs.GetBool(EditorPrefsKey, false); }
        set { EditorPrefs.SetBool(EditorPrefsKey, value); }
    }

    [MenuItem(MenuName)]
    private static void ToggleAction()
    {
        IsEnabled = !IsEnabled;
    }

    [MenuItem(MenuName, true)]
    private static bool ToggleActionValidate()
    {
        Menu.SetChecked(MenuName, IsEnabled);
        return true;
    }
}
