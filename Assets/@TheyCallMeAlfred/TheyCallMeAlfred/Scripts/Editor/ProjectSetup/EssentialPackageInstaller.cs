using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TheyCallMeAlfredUnity.Editor.ProjectSetup {
    public static class EssentialPackageInstaller {

        private static readonly Dictionary<string, (string asset, string folder)> AssetStorePackages = new() {
            { "vFolders2", ("vFolders 2.unitypackage", "kubacho lab/Editor ExtensionsUtilities") },
            { "vHierarchy2", ("vHierarchy 2.unitypackage", "kubacho lab/Editor ExtensionsUtilities") },
            { "vTabs2", ("vTabs 2.unitypackage", "kubacho lab/Editor ExtensionsUtilities") },
            { "Odin Inspector", ("Odin Inspector and Serializer.unitypackage", "Sirenix/Editor ExtensionsSystem") },
            { "Odin Validator", ("Odin Validator.unitypackage", "Sirenix/Editor ExtensionsUtilities") },
            { "Editor Console Pro", ("Editor Console Pro.unitypackage", "FlyingWorm/Editor ExtensionsSystem") },
            { "PrimeTween", ("PrimeTween High-Performance Animations and Sequences.unitypackage", "Kyrylo Kuzyk/Editor ExtensionsAnimation") },
            { "Build Uploader", ("Build Uploader.unitypackage", "JamesGamesNZ/Editor ExtensionsUtilities") },
            { "DOTween Pro", ("DOTween Pro.unitypackage", "Demigiant/Editor ExtensionsVisual Scripting") }
        };

        private static readonly Dictionary<string, string> GitPackages = new() {
            { "Unity Utils", "git+https://github.com/adammyhre/Unity-Utils.git" }
        };

        private static readonly Dictionary<string, string> UnityPackages = new() {
            { "2D Animation", "com.unity.2d.animation@12.0.2" },
            { "Addressables", "com.unity.addressables@2.7.4" },
            { "Behavior", "com.unity.behavior@1.0.12" },
            { "Cinemachine", "com.unity.cinemachine@3.1.4" },
            { "Input System", "com.unity.inputsystem@1.15.0" },
            { "ProBuilder", "com.unity.probuilder@6.0.7" },
            { "Project Auditor", "com.unity.project-auditor@1.0.2" },
            { "Render Pipeline URP", "com.unity.render-pipelines.universal@17.2.0" },
            { "SharpZipLib", "com.unity.sharp-zip-lib@1.4.0" },
            { "Splines", "com.unity.splines@2.8.2" },
            { "Visual Effect Graph", "com.unity.visualeffectgraph@17.2.0" }
        };

        #region Essentials
        [MenuItem("TheyCallMeAlfred/Setup/Install Essentials")]
        public static void InstallEssentials() {
            var essentials = new[] {
                "vFolders2", "vTabs2", "vHierarchy2", "Editor Console Pro",
                "Odin Inspector", "Odin Validator", "Cinemachine"
            };

            string message = "This will install the essential packages:\n\n";
            foreach (var essential in essentials) {
                message += $"• {essential}\n";
            }
            message += "\nContinue?";

            if (EditorUtility.DisplayDialog("Install Essentials", message, "Install", "Cancel")) {
                InstallMultiplePackages(essentials);
            }
        }

        private static void InstallMultiplePackages(string[] packageNames) {
            foreach (var packageName in packageNames) {
                if (AssetStorePackages.ContainsKey(packageName)) {
                    DownloadAndImportAssetStorePackage(packageName);
                } else if (UnityPackages.ContainsKey(packageName)) {
                    InstallUnityPackage(packageName);
                }
            }
        }
        #endregion

        #region Validation
        [MenuItem("TheyCallMeAlfred/Setup/Packages/Validate All Packages")]
        public static void ValidateAllPackages() {
            string report = "Package Validation Report:\n\n";
            bool allValid = true;

            report += "Asset Store Packages:\n";
            foreach (var kvp in AssetStorePackages) {
                bool exists = AssetImporter.CheckAssetExists(kvp.Value.asset, kvp.Value.folder);
                string status = exists ? "✓ FOUND" : "✗ MISSING";
                report += $"  {status} - {kvp.Key}\n";
                if (!exists) allValid = false;
            }

            report += "\nGit Packages:\n";
            foreach (var kvp in GitPackages) {
                report += $"  ✓ AVAILABLE - {kvp.Key}\n";
            }

            report += "\nUnity Packages:\n";
            foreach (var kvp in UnityPackages) {
                report += $"  ✓ AVAILABLE - {kvp.Key}\n";
            }

            if (allValid) {
                report += "\nAll packages are available!";
            } else {
                report += "\nSome Asset Store packages are missing from cache.";
            }

            EditorUtility.DisplayDialog("Package Validation", report, "OK");
        }
        #endregion

        #region Asset Store Packages
        [MenuItem("TheyCallMeAlfred/Setup/Packages/Asset Store/PrimeTween")]
        public static void ImportPrimeTween() => DownloadAndImportAssetStorePackage("PrimeTween");

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Asset Store/Build Uploader")]
        public static void ImportBuildUploader() {
            // Install dependency first
            InstallUnityPackage("SharpZipLib");
            // Then install Build Uploader
            DownloadAndImportAssetStorePackage("Build Uploader");
        }

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Asset Store/DOTween Pro")]
        public static void ImportDOTweenPro() => DownloadAndImportAssetStorePackage("DOTween Pro");


        private static void OpenAssetStoreWindow() {
            // Open Unity's Asset Store window
            EditorApplication.ExecuteMenuItem("Window/Asset Store");
        }

        private static void DownloadAndImportAssetStorePackage(string packageName) {
            if (!AssetStorePackages.TryGetValue(packageName, out var package)) {
                Debug.LogError($"Unknown Asset Store package: {packageName}");
                return;
            }

            bool exists = AssetImporter.CheckAssetExists(package.asset, package.folder);

            if (exists) {
                try {
                    AssetImporter.ImportAsset(package.asset, package.folder);
                }
                catch (Exception e) {
                    EditorUtility.DisplayDialog($"{packageName} Import Error",
                        $"Failed to import {packageName}:\n{e.Message}",
                        "OK");
                }
            } else {
                // Package doesn't exist, guide user through download process
                EditorUtility.DisplayDialog($"{packageName} Download Required",
                    $"{packageName} is not in your Asset Store cache.\n\n" +
                    "Please follow these steps:\n" +
                    "1. The Asset Store window will open\n" +
                    "2. Search for and download the package\n" +
                    "3. Once downloaded, use the 'Import' menu option\n\n" +
                    "The package will then be available for import.",
                    "Open Asset Store");

                OpenAssetStoreWindow();
            }
        }
        #endregion

        #region Git Packages
        [MenuItem("TheyCallMeAlfred/Setup/Packages/Git/Unity Utils")]
        public static void InstallUnityUtils() => InstallGitPackage("Unity Utils");

        private static void InstallGitPackage(string packageName) {
            if (!GitPackages.TryGetValue(packageName, out var packageUrl)) {
                Debug.LogError($"Unknown Git package: {packageName}");
                return;
            }

            PackageManager.InstallPackages(new[] { packageUrl });
        }
        #endregion

        #region Unity Built-in Packages
        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/2D Animation")]
        public static void Install2DAnimation() => InstallUnityPackage("2D Animation");

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Addressables")]
        public static void InstallAddressables() => InstallUnityPackage("Addressables");

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Behavior")]
        public static void InstallBehavior() => InstallUnityPackage("Behavior");


        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Input System")]
        public static void InstallInputSystem() => InstallUnityPackage("Input System");

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/ProBuilder")]
        public static void InstallProBuilder() => InstallUnityPackage("ProBuilder");

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Project Auditor")]
        public static void InstallProjectAuditor() => InstallUnityPackage("Project Auditor");

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Render Pipeline URP")]
        public static void InstallURP() => InstallUnityPackage("Render Pipeline URP");

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/SharpZipLib")]
        public static void InstallSharpZipLib() => InstallUnityPackage("SharpZipLib");

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Splines")]
        public static void InstallSplines() => InstallUnityPackage("Splines");

        [MenuItem("TheyCallMeAlfred/Setup/Packages/Unity/Visual Effect Graph")]
        public static void InstallVisualEffectGraph() => InstallUnityPackage("Visual Effect Graph");

        private static void InstallUnityPackage(string packageName) {
            if (!UnityPackages.TryGetValue(packageName, out var packageId)) {
                Debug.LogError($"Unknown Unity package: {packageName}");
                return;
            }

            PackageManager.InstallPackages(new[] { packageId });
        }
        #endregion
    }
}
