# Dear Hiring Manager - Icon Composer & Liquid Glass Asset Pipeline

This directory contains the master source assets for Apple Icon Composer, macOS/iOS Asset Catalogs (`AppIcon.xcassets`), and next-gen Liquid Glass multi-layer styling.

## Asset Inventory

- `AppIcon_1024x1024.png` - Full resolution 1024×1024 master icon without alpha, conforming strictly to App Store Submission & iOS Springboard requirements.
- `AppIcon_512x512.png` - Standard 512×512 icon for mobile previews and web store listings.
- `DearHiringManager_Icon_Final.svg` - Master vector artwork of the stylized CV sheet, stamp, and corporate identity.
- `DearHiringManager_MasterIcon.svg` - Original vector design layers.

## Working with Apple Icon Composer & Liquid Glass

### What is Icon Composer / Liquid Glass?
Apple's modern design language incorporates multi-layer depth, dynamic lighting, and specular highlights (Liquid Glass):
1. **Background Layer**: The deep obsidian backdrop (`#07090E` to `#0E121B` gradient with subtle radial lighting).
2. **Midground Plate**: The embossed CV document with drop shadow and edge rim highlight.
3. **Foreground Insignia**: The red REJECTED / APPROVED stamp and typography with translucent gloss pass.

### Recommended Environment: PC vs Mac
- **Asset Authoring & Vector Design**: Can be done on both PC and Mac (Illustrator, Photoshop, Figma).
- **Apple Icon Composer & `.icns` / `.xcassets` compile**: **Significantly easier and native on Mac** using Xcode's Asset Catalog editor (`AppIcon.xcassets`), Apple's `actool`, or macOS Icon Composer.
- **Unity Automated Pipeline**: The project is configured with `IOSBuilder.cs`, which automatically copies and assigns `AppIcon_1024x1024.png` to all iOS icon slots in the generated Xcode project.
