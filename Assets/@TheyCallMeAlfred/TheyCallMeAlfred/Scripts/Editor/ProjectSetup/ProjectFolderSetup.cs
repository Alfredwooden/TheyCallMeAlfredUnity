using UnityEditor;

namespace TheyCallMeAlfredUnity.Editor.ProjectSetup {
    public static class ProjectFolderSetup {
        [MenuItem("TheyCallMeAlfred/Setup/Create Folders")]
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

            // Disable Domain Reload
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;
        }
    }
}
