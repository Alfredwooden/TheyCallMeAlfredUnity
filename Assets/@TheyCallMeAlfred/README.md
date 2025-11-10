# TheyCallMeAlfred Unity

A Unity package containing helpful utilities, rendering tools, mesh combiners, animation helpers, and project setup automation.

## Requirements

- Unity 2022.3 LTS or newer
- Git installed on your system

## Installation

### Via Unity Package Manager (Git URL)

1. Open your Unity project
2. Open the Package Manager window (`Window > Package Manager`)
3. Click the `+` button in the top-left corner
4. Select `Add package from git URL...`
5. Enter the following URL:
   ```
   https://github.com/Alfredwooden/TheyCallMeAlfredUnity.git?path=/Assets/%40TheyCallMeAlfred
   ```
6. Click `Add`

### Via manifest.json (Recommended)

1. Navigate to your project's `Packages` folder
2. Open `manifest.json` in a text editor
3. Add the following line to the `dependencies` section:
   ```json
   {
     "dependencies": {
       "com.alfredwooden.theycallmealfredunity": "https://github.com/Alfredwooden/TheyCallMeAlfredUnity.git?path=/Assets/%40TheyCallMeAlfred",
       ...
     }
   }
   ```
4. Save the file and return to Unity
5. Unity will automatically download and install the package

## Features

### Runtime Components

#### Animation
- **Animation Trigger** - Auto-discover and cycle through animation states
- **Simple Rotation** - Rotate objects with configurable speed and axis

#### Rendering
- **Texture Swapper** - Cycle through texture variations using UV scrolling

#### Mesh
- **Combine Mesh** - Combine child meshes into a single mesh for optimization

### Editor Tools

#### Project Setup
- **Essential Package Installer** - Automated installation for common Unity packages
- **Package Manager** - Git and Unity package installation utilities
- **Asset Importer** - Import packages from Asset Store cache
- **Folder Manager** - Project folder structure management

## Usage Examples

### Texture Swapper
```csharp
// Add component to GameObject with a Renderer
var swapper = gameObject.AddComponent<TextureSwapper>();
swapper.faceCount = 3;
swapper.textureDuration = 2f;
swapper.StartTextureLoop();
```

### Animation Trigger
```csharp
// Add component to GameObject with an Animator
var trigger = gameObject.AddComponent<AnimationTrigger>();
trigger.animationDuration = 3f;
trigger.StartAnimationLoop();
```

### Combine Mesh
```csharp
// Add to parent GameObject with MeshFilter children
gameObject.AddComponent<CombineMesh>();
// Meshes will be combined on Start()
```

## Package Installation via Editor Menu

Access package installation tools via Unity menu:
```
TheyCallMeAlfred > Setup > Packages
```

Available package categories:
- Unity Built-in Packages
- Git Packages
- Asset Store Packages (requires Asset Store cache)

## Dependencies

- `com.unity.cinemachine`: 3.1.4

## License

MIT License - See repository for details

## Author

**Alfredo Barros**
- GitHub: [@Alfredwooden](https://github.com/Alfredwooden)
- Email: coomcs@gmail.com

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/Alfredwooden/TheyCallMeAlfredUnity).
