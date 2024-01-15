using UnityEditor;
using System.IO;


public class RefreshBetterBundles
{
    [MenuItem("Assets/Refresh BetterBundles")]
    static void DoRefreshBetterBundles()
    {
        SpellBundleLoader.LoadSpellBundles();
        SpellTagBundleLoader.LoadSpellTagBundles();
        CasterTagBundleLoader.LoadCasterTagBundles();
        PrefabBundleLoader.LoadPrefabBundles();
        BadgeBundleLoader.LoadSpellBundles();
    }
}
