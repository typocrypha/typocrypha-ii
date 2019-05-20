using UnityEditor;
using System.IO;
[InitializeOnLoad]
public class CreateAssetBundles
{
    static CreateAssetBundles()
    {
        BuildAllAssetBundles();
    }
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
