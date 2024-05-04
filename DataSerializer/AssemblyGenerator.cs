#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using ToolBox.Serialization.OdinSerializer.Editor;
using ToolBox.Serialization.OdinSerializer.Utilities.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace ToolBox.Serialization
{
    internal sealed class AssemblyGenerator : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private string _path = string.Empty;

        public int callbackOrder => -1;

        public void OnPreprocessBuild(BuildReport report)
        {
            var platform = EditorUserBuildSettings.activeBuildTarget;
            _path = Directory.CreateDirectory("Assets/OdinAOT").FullName;

            try
            {
                if (!AssemblyImportSettingsUtilities.IsJITSupported(
                        platform,
                        AssemblyImportSettingsUtilities.GetCurrentScriptingBackend(),
                        AssemblyImportSettingsUtilities.GetCurrentApiCompatibilityLevel())
                    && AOTSupportUtilities.ScanProjectForSerializedTypes(out var types))
                {
                    types.Add(typeof(byte[]));

                    var providers = AppDomain
                        .CurrentDomain
                        .GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .Where(x => x.GetInterfaces().Contains(typeof(ITypeProvider)))
                        .ToList();

                    var instances = providers.Select(x => (ITypeProvider)Activator.CreateInstance(x));

                    foreach (var provider in instances)
                    {
                        var userTypes = provider
                            .GetTypes()
                            .Where(x => !types.Contains(x));

                        types.AddRange(userTypes);
                    }

                    AOTSupportUtilities.GenerateDLL(_path, "OdinAOTSupport", types);
                }
            }
            finally
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (Directory.Exists(_path))
            {
                Directory.Delete(_path, true);
                File.Delete(_path + ".meta");
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif