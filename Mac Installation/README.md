# How to install QMods on Mac using the Installer:

## Installation
1. Run the file in PkgInstaller/build
1. Follow the installation setup
1. Place all mods you wish to use in ~/"Library/Application Support/Steam/steamapps/common/Subnautica/QMods"

# How to install QMods on Mac Manually:

## Prerequisites

Download and install [Mono Project](https://www.mono-project.com/download/stable/)

## Installation

From a terminal run:
1. `cp Mac\ Installation/* ~/"Library/Application Support/Steam/steamapps/common/Subnautica/Subnautica.app/Contents/Resources/Data/Managed"`
1. `cd ~/"Library/Application Support/Steam/steamapps/common/Subnautica/Subnautica.app/Contents/Resources/Data/Managed"`
1. `mono QModManager.exe` - Note: You may receive an error stating that the Presentation Framework is missing, don’t worry about this error because this is not a Windows OS that requires the Presentation Framework.
1. `mkdir ~/"Library/Application Support/Steam/steamapps/common/Subnautica/QMods"`

## Usage

Place all mods you wish to use in `~/"Library/Application Support/Steam/steamapps/common/Subnautica/QMods"`

## Testing

To test your installation, try the [Custom Load Screen](https://www.nexusmods.com/subnautica/) mod.

# Known issues:

1. Subnautica Map Mod DOES NOT WORK.  It requires DX11 or DX12 and mac only supports OpenGL.
1. If you have QMods previously installed, you must uninstall and install via the above instructions.

# Developers

These files were created by cloning the existing DLL files, editing them in VisualStudio, and changing the built in path directories to be Mac specific. This has been merged into the main code and no longer requires separate mac installation files.
