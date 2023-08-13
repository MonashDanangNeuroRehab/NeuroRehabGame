using System;

using UnityEngine;
using UnityEditor;

using Codice.CM.Common;
using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Editor.AssetUtils.Processor;
using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.CollabMigration;
using Unity.PlasticSCM.Editor.Inspector;
using Unity.PlasticSCM.Editor.ProjectDownloader;
<<<<<<<< Updated upstream:Library/PackageCache/com.unity.collab-proxy@1.15.16/Editor/PlasticSCM/PlasticPlugin.cs
========
using Unity.PlasticSCM.Editor.SceneView;
>>>>>>>> Stashed changes:Library/PackageCache/com.unity.collab-proxy@2.0.0/Editor/PlasticSCM/PlasticPlugin.cs
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.SceneView;

namespace Unity.PlasticSCM.Editor
{
    /// <summary>
    /// The Plastic SCM plugin for Unity editor.
    /// </summary>
    [InitializeOnLoad]
    public static class PlasticPlugin
    {
        /// <summary>
        /// Invoked when notification status changed.
        /// </summary>
        public static event Action OnNotificationUpdated = delegate { };

<<<<<<<< Updated upstream:Library/PackageCache/com.unity.collab-proxy@1.15.16/Editor/PlasticSCM/PlasticPlugin.cs
        internal static IAssetStatusCache AssetStatusCache { get; private set; }
========
        internal static IAssetStatusCache AssetStatusCache 
        { 
            get { return mAssetStatusCache; } 
        }

        internal static WorkspaceOperationsMonitor WorkspaceOperationsMonitor 
        { 
            get { return mWorkspaceOperationsMonitor; } 
        }
>>>>>>>> Stashed changes:Library/PackageCache/com.unity.collab-proxy@2.0.0/Editor/PlasticSCM/PlasticPlugin.cs

        static PlasticPlugin()
        {
            CloudProjectDownloader.Initialize();
            MigrateCollabProject.Initialize();
            EditorDispatcher.Initialize();

            if (!FindWorkspace.HasWorkspace(ApplicationDataPath.Get()))
                return;

            if (PlasticProjectOfflineMode.IsEnabled())
                return;

            CooldownWindowDelayer cooldownInitializeAction = new CooldownWindowDelayer(
                Enable, UnityConstants.PLUGIN_DELAYED_INITIALIZE_INTERVAL);
            cooldownInitializeAction.Ping();
        }

        /// <summary>
        /// Open the Plastic SCM window.
        /// Also, it disables the offline mode if it is enabled.
        /// </summary>
        public static void OpenPlasticWindowDisablingOfflineModeIfNeeded()
        {
            if (PlasticProjectOfflineMode.IsEnabled())
            {
                PlasticProjectOfflineMode.Disable();
                Enable();
            }

            ShowWindow.Plastic();
        }

        /// <summary>
        /// Get the plugin status icon.
        /// </summary>
        public static Texture GetPluginStatusIcon()
        {
            return PlasticNotification.GetIcon(mNotificationStatus);
        }

        internal static void Enable()
        {
            if (mIsEnabled)
                return;

            mIsEnabled = true;

            PlasticApp.InitializeIfNeeded();

            if (!FindWorkspace.HasWorkspace(Application.dataPath))
                return;

            EnableForWorkspace();
        }

        internal static void EnableForWorkspace()
        {
            if (mIsEnabledForWorkspace)
                return;

            WorkspaceInfo wkInfo = FindWorkspace.InfoForApplicationPath(
                Application.dataPath,
                PlasticApp.PlasticAPI);

            if (wkInfo == null)
                return;

            mIsEnabledForWorkspace = true;

            PlasticApp.SetWorkspace(wkInfo);

            AssetStatusCache = new AssetStatusCache(
                wkInfo,
                PlasticApp.PlasticAPI.IsGluonWorkspace(wkInfo));

<<<<<<<< Updated upstream:Library/PackageCache/com.unity.collab-proxy@1.15.16/Editor/PlasticSCM/PlasticPlugin.cs
            AssetMenuItems.Enable();
            AssetsProcessors.Enable();
            DrawAssetOverlay.Enable();
            DrawInspectorOperations.Enable();
            DrawSceneOperations.Enable();
========
            mAssetStatusCache = new AssetStatusCache(wkInfo, isGluonMode);

            PlasticAssetsProcessor plasticAssetsProcessor = new PlasticAssetsProcessor();

            mWorkspaceOperationsMonitor = BuildWorkspaceOperationsMonitor(
                plasticAssetsProcessor, isGluonMode);
            mWorkspaceOperationsMonitor.Start();

            AssetsProcessors.Enable(
                wkInfo.ClientPath, plasticAssetsProcessor, mAssetStatusCache);
            AssetMenuItems.Enable(
                wkInfo, mAssetStatusCache);
            DrawAssetOverlay.Enable(
                wkInfo.ClientPath, mAssetStatusCache);
            DrawInspectorOperations.Enable(
                wkInfo.ClientPath, mAssetStatusCache);
            DrawSceneOperations.Enable(
                wkInfo.ClientPath, mWorkspaceOperationsMonitor, mAssetStatusCache);
>>>>>>>> Stashed changes:Library/PackageCache/com.unity.collab-proxy@2.0.0/Editor/PlasticSCM/PlasticPlugin.cs
        }

        internal static void Disable()
        {
            try
            {
                PlasticApp.Dispose();

                if (!mIsEnabledForWorkspace)
                    return;

<<<<<<<< Updated upstream:Library/PackageCache/com.unity.collab-proxy@1.15.16/Editor/PlasticSCM/PlasticPlugin.cs
========
                mWorkspaceOperationsMonitor.Stop();

>>>>>>>> Stashed changes:Library/PackageCache/com.unity.collab-proxy@2.0.0/Editor/PlasticSCM/PlasticPlugin.cs
                AssetsProcessors.Disable();
                AssetMenuItems.Disable();
                DrawAssetOverlay.Disable();
                DrawInspectorOperations.Disable();
                DrawSceneOperations.Disable();
            }
            finally
            {
                mIsEnabled = false;
                mIsEnabledForWorkspace = false;
            }
        }

        internal static void SetNotificationStatus(
            PlasticWindow plasticWindow,
            PlasticNotification.Status status)
        {
            mNotificationStatus = status;

            plasticWindow.SetupWindowTitle(status);

            if (OnNotificationUpdated!=null) OnNotificationUpdated.Invoke();
        }

<<<<<<<< Updated upstream:Library/PackageCache/com.unity.collab-proxy@1.15.16/Editor/PlasticSCM/PlasticPlugin.cs
        static PlasticNotification.Status sNotificationStatus;

        static bool sIsEnabled;
        static bool sIsEnabledForWorkspace;
========
        static PlasticNotification.Status mNotificationStatus;
        static AssetStatusCache mAssetStatusCache;
        static WorkspaceOperationsMonitor mWorkspaceOperationsMonitor;
        static bool mIsEnabled;
        static bool mIsEnabledForWorkspace;
>>>>>>>> Stashed changes:Library/PackageCache/com.unity.collab-proxy@2.0.0/Editor/PlasticSCM/PlasticPlugin.cs
    }
}
