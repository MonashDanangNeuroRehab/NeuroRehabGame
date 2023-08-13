using UnityEditor;

using AssetOverlays = Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;

namespace Unity.PlasticSCM.Editor.AssetUtils.Processor
{
    class AssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        internal static bool IsEnabled { get; set; }
        internal static bool ForceCheckout { get; private set; }

        /* We need to do a checkout, verifying that the content/date or size has changed. 
         * In order to do this checkout we need the changes to have reached the disk. 
         * That's why we save the changed files in this array, and when they are reloaded
         * in AssetPostprocessor.OnPostprocessAllAssets we process them. */
        internal static string[] ModifiedAssets { get; set; }

        static AssetModificationProcessor()
        {
            ForceCheckout = EditorPrefs.GetBool("forceCheckoutPlasticSCM");
        }

<<<<<<<< Updated upstream:Library/PackageCache/com.unity.collab-proxy@1.15.16/Editor/PlasticSCM/AssetsUtils/Processor/AssetModificationProcessor.cs
        internal static void RegisterAssetStatusCache(
========
        internal static void Enable(
            string wkPath,
>>>>>>>> Stashed changes:Library/PackageCache/com.unity.collab-proxy@2.0.0/Editor/PlasticSCM/AssetsUtils/Processor/AssetModificationProcessor.cs
            IAssetStatusCache assetStatusCache)
        {
            mWkPath = wkPath;
            mAssetStatusCache = assetStatusCache;
<<<<<<<< Updated upstream:Library/PackageCache/com.unity.collab-proxy@1.15.16/Editor/PlasticSCM/AssetsUtils/Processor/AssetModificationProcessor.cs
========

            mIsEnabled = true;
        }

        internal static void Disable()
        {
            mIsEnabled = false;

            mWkPath = null;
            mAssetStatusCache = null;
>>>>>>>> Stashed changes:Library/PackageCache/com.unity.collab-proxy@2.0.0/Editor/PlasticSCM/AssetsUtils/Processor/AssetModificationProcessor.cs
        }

        internal static void SetForceCheckoutOption(bool isEnabled)
        {
            ForceCheckout = isEnabled;

            EditorPrefs.SetBool(
                "forceCheckoutPlasticSCM",
                isEnabled);
        }

        static string[] OnWillSaveAssets(string[] paths)
        {
<<<<<<<< Updated upstream:Library/PackageCache/com.unity.collab-proxy@1.15.16/Editor/PlasticSCM/AssetsUtils/Processor/AssetModificationProcessor.cs
            if (!IsEnabled)
========
            if (!mIsEnabled)
>>>>>>>> Stashed changes:Library/PackageCache/com.unity.collab-proxy@2.0.0/Editor/PlasticSCM/AssetsUtils/Processor/AssetModificationProcessor.cs
                return paths;

            ModifiedAssets = (string[])paths.Clone();

            return paths;
        }

        static bool IsOpenForEdit(string assetPath, out string message)
        {
            message = string.Empty;

<<<<<<<< Updated upstream:Library/PackageCache/com.unity.collab-proxy@1.15.16/Editor/PlasticSCM/AssetsUtils/Processor/AssetModificationProcessor.cs
            if (!IsEnabled)
                return true;

            if (assetPath.StartsWith("ProjectSettings/"))
========
            if (!mIsEnabled)
>>>>>>>> Stashed changes:Library/PackageCache/com.unity.collab-proxy@2.0.0/Editor/PlasticSCM/AssetsUtils/Processor/AssetModificationProcessor.cs
                return true;

            if (!ForceCheckout)
                return true;

            if (assetPath.StartsWith("ProjectSettings/"))
                return true;

            string assetFullPath = AssetsPath.GetFullPathUnderWorkspace.
                ForAsset(mWkPath, assetPath);

            if (assetFullPath == null)
                return true;

            if (MetaPath.IsMetaPath(assetFullPath))
                assetFullPath = MetaPath.GetPathFromMetaPath(assetFullPath);

            AssetOverlays.AssetStatus status = mAssetStatusCache.
                GetStatus(assetFullPath);

            if (AssetOverlays.ClassifyAssetStatus.IsAdded(status) ||
                AssetOverlays.ClassifyAssetStatus.IsCheckedOut(status))
                return true;

            return !AssetOverlays.ClassifyAssetStatus.IsControlled(status);
        }

<<<<<<<< Updated upstream:Library/PackageCache/com.unity.collab-proxy@1.15.16/Editor/PlasticSCM/AssetsUtils/Processor/AssetModificationProcessor.cs
========
        static bool mIsEnabled;

>>>>>>>> Stashed changes:Library/PackageCache/com.unity.collab-proxy@2.0.0/Editor/PlasticSCM/AssetsUtils/Processor/AssetModificationProcessor.cs
        static IAssetStatusCache mAssetStatusCache;
        static string mWkPath;
    }
}
