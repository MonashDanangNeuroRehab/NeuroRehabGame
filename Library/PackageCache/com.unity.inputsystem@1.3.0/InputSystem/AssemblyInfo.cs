using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;

[assembly: AssemblyVersion(InputSystem.kAssemblyVersion)]
[assembly: InternalsVisibleTo("Unity.InputSystem.TestFramework")]
[assembly: InternalsVisibleTo("Unity.InputSystem.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.InputSystem.Tests")]
[assembly: InternalsVisibleTo("Unity.InputSystem.IntegrationTests")]

namespace UnityEngine.InputSystem
{
    public static partial class InputSystem
    {
        // Keep this in sync with "Packages/com.unity.inputsystem/package.json".
        // NOTE: Unfortunately, System.Version doesn't use semantic versioning so we can't include
        //       "-preview" suffixes here.
<<<<<<<< Updated upstream:Library/PackageCache/com.unity.inputsystem@1.3.0/InputSystem/AssemblyInfo.cs
        internal const string kAssemblyVersion = "1.3.0";
        internal const string kDocUrl = "https://docs.unity3d.com/Packages/com.unity.inputsystem@1.3";
========
        internal const string kAssemblyVersion = "1.4.4";
        internal const string kDocUrl = "https://docs.unity3d.com/Packages/com.unity.inputsystem@1.4";
>>>>>>>> Stashed changes:Library/PackageCache/com.unity.inputsystem@1.4.4/InputSystem/AssemblyInfo.cs
    }
}