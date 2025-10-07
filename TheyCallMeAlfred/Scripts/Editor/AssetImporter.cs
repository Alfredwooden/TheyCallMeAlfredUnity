using System;
using System.IO;
using UnityEditor;
using UnityEngine;

using static System.Environment;
using static System.IO.Path;

namespace TheyCallMeAlfredUnity.Editor
{
    public static class AssetImporter
    {
        public static void ImportAsset(string asset, string folder)
        {
            string basePath;
            if (OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            {
                string homeDirectory = GetFolderPath(SpecialFolder.Personal);
                basePath = Combine(homeDirectory, "Library/Unity/Asset Store-5.x");
            }
            else
            {
                string defaultPath = Combine(GetFolderPath(SpecialFolder.ApplicationData), "Unity");
                basePath = Combine(EditorPrefs.GetString("AssetStoreCacheRootPath", defaultPath), "Asset Store-5.x");
            }

            asset = asset.EndsWith(".unitypackage") ? asset : asset + ".unitypackage";

            string fullPath = Combine(basePath, folder, asset);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"The asset package was not found at the path: {fullPath}");
            }

            AssetDatabase.ImportPackage(fullPath, false);
        }
    }
}
