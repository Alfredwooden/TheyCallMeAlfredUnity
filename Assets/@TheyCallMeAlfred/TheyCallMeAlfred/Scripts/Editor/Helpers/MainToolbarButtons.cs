using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

public class MainToolbarButtons
{
    [MainToolbarElement("Project/Open Project Settings", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement ProjectSettingsButton()
    {
        var icon = EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D;
        var content = new MainToolbarContent(icon);
        return new MainToolbarButton(content, () => SettingsService.OpenProjectSettings());
    }

    [MainToolbarElement("Test/Mesh Info Text", defaultDockPosition = MainToolbarDockPosition.Right)]
    public static MainToolbarElement MeshInfoText()
    {
        // Create content with just text (no icon)
        var content = new MainToolbarContent("Mesh Info: Click to inspect");

        // Return as a button with empty action (non-clickable text)
        return new MainToolbarButton(content, () => { });
    }
}