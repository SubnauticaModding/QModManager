# How to install QModManager on Mac using the Installer:

## Installation
1. Run QModsInstaller.pkg
1. Follow the installation setup
1. Place all mods you wish to use in ~/"Library/Application Support/Steam/steamapps/common/Subnautica/QMods"

# How to install QModManually on Mac Manually:

## Prerequisites

Download and install [Mono Project](https://www.mono-project.com/download/stable/)

## Installation

From a terminal run:
1. `cp Build/* ~/"Library/Application Support/Steam/steamapps/common/Subnautica/Subnautica.app/Contents/Resources/Data/Managed"`
1. `cd ~/"Library/Application Support/Steam/steamapps/common/Subnautica/Subnautica.app/Contents/Resources/Data/Managed"`
1. `mono QModManager.exe` - Note: If an error is encountered, make sure you copied all the necessary dependancies to the folder as well. To see which dependancies are necessary, [Check Here](https://github.com/QModManager/QModManager/tree/dev/master/Dependencies).
1. `mkdir ~/"Library/Application Support/Steam/steamapps/common/Subnautica/QMods"`

## Usage

Place all mods you wish to use in `~/"Library/Application Support/Steam/steamapps/common/Subnautica/QMods"`
For epic games, the folder is in `~/"Library/Application Support/Epic Games/Subnautica/QMods"`

## Testing

To test your installation, try the [Custom Load Screen](https://www.nexusmods.com/subnautica/mods/124?tab=description) mod for Subnautica and [Custom Load Screen](https://www.nexusmods.com/subnauticabelowzero/mods/8?tab=description) for Below Zero. Please note these are separate mods.

# Known issues:

1. Subnautica Map Mod DOES NOT WORK.  It requires DX11 or DX12 and mac only supports OpenGL.
1. If you have QMods previously installed, you must uninstall and install via the above instructions.
1. Alternatively to uninstall QMods, you can use steam to verify game content to refresh your install of Subnautica.
