using UnityEditor;

namespace TheyCallMeAlfredUnity.Editor.ProjectSetup {
    public static class ProjectSetup {
        [MenuItem("Tools/Setup/Import Essential Assets")]
        public static void ImportEssentials() {
            AssetImporter.ImportAsset("Odin Inspector and Serializer.unitypackage", "Sirenix/Editor ExtensionsSystem");
            AssetImporter.ImportAsset("Odin Validator.unitypackage", "Sirenix/Editor ExtensionsUtilities");
            AssetImporter.ImportAsset("Editor Console Pro.unitypackage", "FlyingWorm/Editor ExtensionsSystem");
            // and so on...
        }

        [MenuItem("Tools/Setup/Install Essential Packages")]
        public static void InstallPackages() =>
            PackageManager.InstallPackages(new[] {
                "com.unity.2d.animation", "git+https://github.com/adammyhre/Unity-Utils.git",
                "git+https://github.com/adammyhre/Unity-Improved-Timers.git",
                "git+https://github.com/KyleBanks/scene-ref-attribute.git"
                // If necessary, import new Input System last as it requires a Unity Editor restart
                // "com.unity.inputsystem"
            });

        [MenuItem("Tools/Setup/Create Folders")]
        public static void CreateFolders() {
            FolderManager.Create("_Project", "Animation", "Art", "Materials", "Prefabs", "Scripts/Tests",
                "Scripts/Tests/Editor", "Scripts/Tests/Runtime");
            AssetDatabase.Refresh();
            FolderManager.Move("_Project", "Scenes");
            FolderManager.Move("_Project", "Settings");
            FolderManager.Delete("TutorialInfo");
            AssetDatabase.Refresh();

            AssetDatabase.MoveAsset("Assets/InputSystem_Actions.inputactions",
                "Assets/_Project/Settings/InputSystem_Actions.inputactions");
            AssetDatabase.DeleteAsset("Assets/Readme.asset");
            AssetDatabase.Refresh();

            // Optional: Disable Domain Reload
            // EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;
        }
    }
}
