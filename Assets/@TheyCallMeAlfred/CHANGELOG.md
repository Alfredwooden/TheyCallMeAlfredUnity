## 1.0.0 - 07-10-2025

### First release

### Added

- Added `TheyCallMeAlfredUnity` namespace to all utility scripts:
  - `AnimationTrigger.cs`
  - `CombineMesh.cs`
  - `SimpleRotation.cs`
  - `TextureSwapper.cs`
- Added `AddComponentMenu` attributes to all utility scripts for easy access in Unity's Component menu
- Reorganized ProjectSetup functionality into modular Editor scripts:
  - `ProjectSetup.cs` - Main entry point with Unity menu items
  - `AssetImporter.cs` - Handles asset package imports
  - `PackageManager.cs` - Manages package installation
  - `FolderManager.cs` - Manages folder operations
