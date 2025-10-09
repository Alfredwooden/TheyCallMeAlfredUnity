using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static System.Environment;
using static System.IO.Path;

namespace TheyCallMeAlfredUnity.Editor.ProjectSetup {
    public static class AssetImporter {
        public static void ImportAsset(string asset, string folder) {
            string assetPath = FindAsset(asset, folder);

            if (assetPath == null) {
                throw new FileNotFoundException($"The asset package '{asset}' was not found in the Asset Store cache.");
            }

            AssetDatabase.ImportPackage(assetPath, false);
        }

        public static bool CheckAssetExists(string asset, string folder) {
            try {
                string assetPath = FindAsset(asset, folder);

                if (assetPath != null) {
                    return true;
                } else {
                    return false;
                }
            }
            catch (Exception e) {
                Debug.LogError($"Error checking asset existence: {e.Message}");
                return false;
            }
        }

        public static string FindAsset(string asset, string folderPattern) {
            string basePath = GetAssetStorePath();
            asset = asset.EndsWith(".unitypackage") ? asset : asset + ".unitypackage";

            // First try the exact path
            string[] folderParts = folderPattern.Split('/');
            string exactPath = Combine(new[] { basePath }.Concat(folderParts).Concat(new[] { asset }).ToArray());

            if (File.Exists(exactPath)) {
                return exactPath;
            }

            // If exact path fails, search recursively
            try {
                string[] foundFiles = Directory.GetFiles(basePath, asset, SearchOption.AllDirectories);

                if (foundFiles.Length > 0) {
                    return foundFiles[0];
                }
            }
            catch (Exception e) {
                Debug.LogError($"Error searching for asset: {e.Message}");
            }

            return null;
        }

        public static string GetAssetStorePath() {
            string basePath;

            // Use Unity's runtime platform detection for more reliable results
            if (Application.platform == RuntimePlatform.OSXEditor) {
                // macOS
                string homeDirectory = GetFolderPath(SpecialFolder.Personal);
                basePath = Combine(homeDirectory, "Library", "Unity", "Asset Store-5.x");
            }
            else if (Application.platform == RuntimePlatform.LinuxEditor) {
                // Linux
                string homeDirectory = GetFolderPath(SpecialFolder.Personal);
                basePath = Combine(homeDirectory, ".local", "share", "unity3d", "Asset Store");
            }
            else {
                // Windows
                string defaultPath = Combine(GetFolderPath(SpecialFolder.ApplicationData), "Unity");
                basePath = Combine(EditorPrefs.GetString("AssetStoreCacheRootPath", defaultPath), "Asset Store-5.x");
            }

            if (!Directory.Exists(basePath)) {
                Debug.LogWarning($"Asset Store directory does not exist: {basePath}");
            }

            return basePath;
        }
    }
}
