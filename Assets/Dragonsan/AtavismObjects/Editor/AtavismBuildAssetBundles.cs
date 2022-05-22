using System.IO;
using UnityEditor;
using UnityEngine;

namespace Atavism
{
    public class AtavismBuildAssetBundles
    {

        [MenuItem("Assets/Atavism Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);

            if (!Directory.Exists("AssetBundles"))
                Directory.CreateDirectory("AssetBundles");
            BuildPipeline.BuildAssetBundles("AssetBundles", BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}