using System;

using UnityEditor;

namespace Unity.PlasticSCM.Editor.UI
{
    public class CooldownWindowDelayer
    {
<<<<<<<< Updated upstream:Library/PackageCache/com.unity.collab-proxy@1.15.16/Editor/PlasticSCM/UI/CooldownWindowDelayer.cs
        internal CooldownWindowDelayer(Action action, double cooldownSeconds)
========
        internal static bool IsUnitTesting { get; set; }

        public CooldownWindowDelayer(Action action, double cooldownSeconds)
>>>>>>>> Stashed changes:Library/PackageCache/com.unity.collab-proxy@2.0.0/Editor/PlasticSCM/UI/CooldownWindowDelayer.cs
        {
            mAction = action;
            mCooldownSeconds = cooldownSeconds;
        }

        public void Ping()
        {
<<<<<<<< Updated upstream:Library/PackageCache/com.unity.collab-proxy@1.15.16/Editor/PlasticSCM/UI/CooldownWindowDelayer.cs
========
            if (IsUnitTesting)
            {
                mAction();
                return;
            }

>>>>>>>> Stashed changes:Library/PackageCache/com.unity.collab-proxy@2.0.0/Editor/PlasticSCM/UI/CooldownWindowDelayer.cs
            if (mIsOnCooldown)
            {
                RefreshCooldown();
                return;
            }

            StartCooldown();
        }

        void RefreshCooldown()
        {
            mIsOnCooldown = true;

            mSecondsOnCooldown = mCooldownSeconds;
        }

        void StartCooldown()
        {
            mLastUpdateTime = EditorApplication.timeSinceStartup;

            EditorApplication.update += OnUpdate;

            RefreshCooldown();
        }

        void EndCooldown()
        {
            EditorApplication.update -= OnUpdate;

            mIsOnCooldown = false;

            mAction();
        }

        void OnUpdate()
        {
            double updateTime = EditorApplication.timeSinceStartup;
            double deltaSeconds = updateTime - mLastUpdateTime;

            mSecondsOnCooldown -= deltaSeconds;

            if (mSecondsOnCooldown < 0)
                EndCooldown();

            mLastUpdateTime = updateTime;
        }

        readonly Action mAction;
        readonly double mCooldownSeconds;

        double mLastUpdateTime;
        bool mIsOnCooldown;
        double mSecondsOnCooldown;
    }
}
