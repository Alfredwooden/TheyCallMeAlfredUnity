## 1.1.0 - 08-10-2025

### Major Refactor - ProjectSetup System

### Added

- **Install Essentials Menu**: One-click installation of essential packages
  - vFolders2, vTabs2, vHierarchy2, Editor Console Pro, Odin Inspector, Odin Validator, Cinemachine
- **Smart Package Management**: Automatic download & import workflow for Asset Store packages
- **Dependency Resolution**: Build Uploader automatically installs SharpZipLib dependency
- **Enhanced Validation**: Comprehensive package validation with detailed reporting
- **Streamlined Menu Structure**: Consolidated package management under single menu

### Changed

- **Menu Reorganization**:
  - Moved validation inside Packages menu
  - Removed duplicate "Download & Import" menu
  - Essential packages removed from individual menus
- **Platform Detection**: Fixed macOS detection using Unity's RuntimePlatform instead of OSVersion
- **Logging Cleanup**: Removed excessive debug logging for cleaner console output
- **Package Configuration**: Centralized package definitions in dictionaries for easy maintenance

### Technical Improvements

- **AssetImporter.cs**:
  - Fixed platform detection for macOS Asset Store cache
  - Removed excessive logging
  - Improved error handling
- **EssentialPackageInstaller.cs**:
  - Added smart download & import workflow
  - Implemented dependency management
  - Created Install Essentials functionality
  - Streamlined menu structure
  - Added Build Uploader package support

### Package Support

- **Asset Store Packages**: vFolders2, vHierarchy2, vTabs2, Odin Inspector, Odin Validator, Editor Console Pro, PrimeTween, Build Uploader
- **Git Packages**: Unity Utils
- **Unity Packages**: 2D Animation, Addressables, Behavior, Cinemachine, Input System, ProBuilder, Project Auditor, URP, SharpZipLib, Splines, Visual Effect Graph

---

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
