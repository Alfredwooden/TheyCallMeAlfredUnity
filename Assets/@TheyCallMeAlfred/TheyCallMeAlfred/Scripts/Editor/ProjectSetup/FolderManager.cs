using System.IO;
using UnityEditor;
using UnityEngine;
using static System.IO.Path;

namespace TheyCallMeAlfredUnity.Editor.ProjectSetup {
    public static class FolderManager {
        public static void Create(string root, params string[] folders) {
            string fullpath = Combine(Application.dataPath, root);
            if (!Directory.Exists(fullpath)) {
                Directory.CreateDirectory(fullpath);
            }

            foreach (string folder in folders) {
                CreateSubFolders(fullpath, folder);
            }
        }

        private static void CreateSubFolders(string rootPath, string folderHierarchy) {
            string[] folders = folderHierarchy.Split('/');
            string currentPath = rootPath;

            foreach (string folder in folders) {
                currentPath = Combine(currentPath, folder);
                if (!Directory.Exists(currentPath)) {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }

        public static void Move(string newParent, string folderName) {
            string sourcePath = $"Assets/{folderName}";
            if (AssetDatabase.IsValidFolder(sourcePath)) {
                string destinationPath = $"Assets/{newParent}/{folderName}";
                string error = AssetDatabase.MoveAsset(sourcePath, destinationPath);

                if (!string.IsNullOrEmpty(error)) {
                    Debug.LogError($"Failed to move {folderName}: {error}");
                }
            }
        }

        public static void Delete(string folderName) {
            string pathToDelete = $"Assets/{folderName}";

            if (AssetDatabase.IsValidFolder(pathToDelete)) {
                AssetDatabase.DeleteAsset(pathToDelete);
            }
        }
    }
}
