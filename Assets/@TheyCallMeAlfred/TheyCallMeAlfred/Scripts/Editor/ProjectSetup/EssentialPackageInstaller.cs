using System;
using System.IO;
using UnityEditor;
using static System.Environment;
using static System.IO.Path;

namespace TheyCallMeAlfredUnity.Editor.ProjectSetup {
    public static class EssentialPackageInstaller {

        #region Git Packages
        [MenuItem("TheyCallMeAlfred/Setup/Packages/Git/Unity Utils")]
        public static void InstallUnityUtils() =>
            PackageManager.InstallPackages(new[] { "git+https://github.com/adammyhre/Unity-Utils.git" });
        #endregion

        #region Unity Asset Store Packages (Local Files)
        [MenuItem("TheyCallMeAlfred/Setup/Packages/Asset Store/PrimeTween")]
        public static void InstallPrimeTween() =>
            PackageManager.InstallPackages(new[] { "file:../Assets/Plugins/PrimeTween/internal/com.kyrylokuzyk.primetween.tgz" });
        #endregion

        #region Unity Asset Store Packages (Import from Cache)
        [MenuItem("TheyCallMeAlfred/Setup/Assets/Import vFolders (v2.1.9)")]
        public static void ImportVFolders() {
            try {
                AssetImporter.ImportAsset("vFolders 2.unitypackage", "kubacho lab/Editor ExtensionsUtilities");
                EditorUtility.DisplayDialog("vFolders Import", "vFolders has been imported successfully!", "OK");
            }
            catch (FileNotFoundException) {
                EditorUtility.DisplayDialog("vFolders Import",
                    "vFolders package not found in Asset Store cache.\n\n" +
                    "Please ensure you have downloaded vFolders from the Unity Asset Store first.",
                    "OK");
            }
        }

        [MenuItem("TheyCallMeAlfred/Setup/Assets/Import vHierarchy (v2.1.5)")]
        public static void ImportVHierarchy() {
            try {
                AssetImporter.ImportAsset("vHierarchy 2.unitypackage", "kubacho lab/Editor ExtensionsUtilities");
                EditorUtility.DisplayDialog("vHierarchy Import", "vHierarchy has been imported successfully!", "OK");
            }
            catch (FileNotFoundException) {
                EditorUtility.DisplayDialog("vHierarchy Import",
                    "vHierarchy package not found in Asset Store cache.\n\n" +
                    "Please ensure you have downloaded vHierarchy from the Unity Asset Store first.",
                    "OK");
            }
        }

        [MenuItem("TheyCallMeAlfred/Setup/Assets/Import vTabs (v2.1.3)")]
        public static void ImportVTabs() {
            try {
                AssetImporter.ImportAsset("vTabs 2.unitypackage", "kubacho lab/Editor ExtensionsUtilities");
                EditorUtility.DisplayDialog("vTabs Import", "vTabs has been imported successfully!", "OK");
            }
            catch (FileNotFoundException) {
                EditorUtility.DisplayDialog("vTabs Import",
                    "vTabs package not found in Asset Store cache.\n\n" +
                    "Please ensure you have downloaded vTabs from the Unity Asset Store first.",
                    "OK");
            }
        }

        [MenuItem("TheyCallMeAlfred/Setup/Assets/Import Odin Inspector")]
        public static void ImportOdinInspector() {
            try {
                AssetImporter.ImportAsset("Odin Inspector and Serializer.unitypackage", "Sirenix/Editor ExtensionsSystem");
                EditorUtility.DisplayDialog("Odin Inspector Import", "Odin Inspector has been imported successfully!", "OK");
            }
            catch (FileNotFoundException) {
                EditorUtility.DisplayDialog("Odin Inspector Import",
                    "Odin Inspector package not found in Asset Store cache.\n\n" +
                    "Please ensure you have downloaded Odin Inspector from the Unity Asset Store first.",
                    "OK");
            }
        }

        [MenuItem("TheyCallMeAlfred/Setup/Assets/Import Odin Validator")]
        public static void ImportOdinValidator() {
            try {
                AssetImporter.ImportAsset("Odin Validator.unitypackage", "Sirenix/Editor ExtensionsUtilities");
                EditorUtility.DisplayDialog("Odin Validator Import", "Odin Validator has been imported successfully!", "OK");
            }
            catch (FileNotFoundException) {
                EditorUtility.DisplayDialog("Odin Validator Import",
                    "Odin Validator package not found in Asset Store cache.\n\n" +
                    "Please ensure you have downloaded Odin Validator from the Unity Asset Store first.",
                    "OK");
            }
        }

        [MenuItem("TheyCallMeAlfred/Setup/Assets/Import Editor Console Pro")]
        public static void ImportEditorConsolePro() {
            try {
                AssetImporter.ImportAsset("Editor Console Pro.unitypackage", "FlyingWorm/Editor ExtensionsSystem");
                EditorUtility.DisplayDialog("Editor Console Pro Import", "Editor Console Pro has been imported successfully!", "OK");
            }
            catch (FileNotFoundException) {
                EditorUtility.DisplayDialog("Editor Console Pro Import",
                    "Editor Console Pro package not found in Asset Store cache.\n\n" +
                    "Please ensure you have downloaded Editor Console Pro from the Unity Asset Store first.",
                    "OK");
            }
        }
        #endregion

        #region Unity Built-in Packages
        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/2D Animation")]
        public static void Install2DAnimation() =>
            PackageManager.InstallPackages(new[] { "com.unity.2d.animation@12.0.2" });

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Addressables")]
        public static void InstallAddressables() =>
            PackageManager.InstallPackages(new[] { "com.unity.addressables@2.7.4" });

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Behavior")]
        public static void InstallBehavior() =>
            PackageManager.InstallPackages(new[] { "com.unity.behavior@1.0.12" });

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Cinemachine")]
        public static void InstallCinemachine() =>
            PackageManager.InstallPackages(new[] { "com.unity.cinemachine@3.1.4" });

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Input System")]
        public static void InstallInputSystem() =>
            PackageManager.InstallPackages(new[] { "com.unity.inputsystem@1.15.0" });

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/ProBuilder")]
        public static void InstallProBuilder() =>
            PackageManager.InstallPackages(new[] { "com.unity.probuilder@6.0.7" });

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Project Auditor")]
        public static void InstallProjectAuditor() =>
            PackageManager.InstallPackages(new[] { "com.unity.project-auditor@1.0.2" });

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Render Pipeline URP")]
        public static void InstallURP() =>
            PackageManager.InstallPackages(new[] { "com.unity.render-pipelines.universal@17.2.0" });

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/SharpZipLib")]
        public static void InstallSharpZipLib() =>
            PackageManager.InstallPackages(new[] { "com.unity.sharp-zip-lib@1.4.0" });

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Splines")]
        public static void InstallSplines() =>
            PackageManager.InstallPackages(new[] { "com.unity.splines@2.8.2" });

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Visual Effect Graph")]
        public static void InstallVisualEffectGraph() =>
            PackageManager.InstallPackages(new[] { "com.unity.visualeffectgraph@17.2.0" });
        #endregion

        #region Install All Methods
        [MenuItem("TheyCallMeAlfred/Setup/Install All Git Packages")]
        public static void InstallAllGitPackages() =>
            PackageManager.InstallPackages(new[] {
                "git+https://github.com/adammyhre/Unity-Utils.git"
            });

        [MenuItem("TheyCallMeAlfred/Setup/Install All Unity Packages")]
        public static void InstallAllUnityPackages() =>
            PackageManager.InstallPackages(new[] {
                "com.unity.2d.animation@12.0.2",
                "com.unity.addressables@2.7.4",
                "com.unity.behavior@1.0.12",
                "com.unity.cinemachine@3.1.4",
                "com.unity.inputsystem@1.15.0",
                "com.unity.probuilder@6.0.7",
                "com.unity.project-auditor@1.0.2",
                "com.unity.render-pipelines.universal@17.2.0",
                "com.unity.sharp-zip-lib@1.4.0",
                "com.unity.splines@2.8.2",
                "com.unity.visualeffectgraph@17.2.0"
            });

        [MenuItem("TheyCallMeAlfred/Setup/Import All Asset Store Assets")]
        public static void ImportAllAssetStoreAssets() {
            try {
                AssetImporter.ImportAsset("Odin Inspector and Serializer.unitypackage", "Sirenix/Editor ExtensionsSystem");
                AssetImporter.ImportAsset("Odin Validator.unitypackage", "Sirenix/Editor ExtensionsUtilities");
                AssetImporter.ImportAsset("Editor Console Pro.unitypackage", "FlyingWorm/Editor ExtensionsSystem");
                EditorUtility.DisplayDialog("Asset Store Import", "All Asset Store assets have been imported successfully!", "OK");
            }
            catch (FileNotFoundException ex) {
                EditorUtility.DisplayDialog("Asset Store Import",
                    $"Some packages were not found in Asset Store cache:\n{ex.Message}\n\n" +
                    "Please ensure you have downloaded all required packages from the Unity Asset Store first.",
                    "OK");
            }
        }

        [MenuItem("TheyCallMeAlfred/Setup/Install All Packages")]
        public static void InstallAllPackages() =>
            PackageManager.InstallPackages(new[] {
                // Git packages
                "git+https://github.com/adammyhre/Unity-Utils.git",
                // Asset Store packages (local files)
                "file:../Assets/Plugins/PrimeTween/internal/com.kyrylokuzyk.primetween.tgz",
                // Unity packages
                "com.unity.2d.animation@12.0.2",
                "com.unity.addressables@2.7.4",
                "com.unity.behavior@1.0.12",
                "com.unity.cinemachine@3.1.4",
                "com.unity.inputsystem@1.15.0",
                "com.unity.probuilder@6.0.7",
                "com.unity.project-auditor@1.0.2",
                "com.unity.render-pipelines.universal@17.2.0",
                "com.unity.sharp-zip-lib@1.4.0",
                "com.unity.splines@2.8.2",
                "com.unity.visualeffectgraph@17.2.0"
            });
        #endregion
    }
}
