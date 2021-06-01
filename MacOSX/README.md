# Building on OSX

# Below Zero Building

## Preparation

### Prepare Solution
* Open `QModManager.sln` with Visual Code to initialize packages

### Create Publicized Assemblies
* Follow instructions at the [SMLHelper GitHub page](https://github.com/SubnauticaModding/SMLHelper)
* Download [AssemblyPublicizer.zip](https://github.com/CabbageCrow/AssemblyPublicizer/releases/download/v1.1.0/AssemblyPublicizer.zip) and extract to `~/AssemblyPublicizer.exe`
* Create publizised assemblies:
    * `cd "~/Library/Application Support/Steam/steamapps/common/SubnauticaZero/SubnauticaZero.app/Contents/Resources/Data/Managed"`
    * `mono ~/AssemblyPublicizer.exe -i Assembly-CSharp.dll`
    * `mono ~/AssemblyPublicizer.exe -i Assembly-CSharp-firstpass.dll`
    * Copy the dlls from `publicized_assemblies/` to `Dependencies/BZ.STABLE/`

## Build
* Create distribution package by running `MacOSX/build.sh` from the top level of the repository.

## Installation
* Copy distribution package contents to `~/Library/Application Support/Steam/steamapps/common/SubnauticaZero`
* Update Steam Launch options to `"/Users/username/Library/Application Support/Steam/steamapps/common/SubnauticaZero/QModManager.sh" %command%`

# Subnautica Building

## Preparation

### Prepare Solution
* Open `QModManager.sln` with Visual Code to initialize packages

### Create Publicized Assemblies
* Follow instructions at the [SMLHelper GitHub page](https://github.com/SubnauticaModding/SMLHelper)
* Download [AssemblyPublicizer.zip](https://github.com/CabbageCrow/AssemblyPublicizer/releases/download/v1.1.0/AssemblyPublicizer.zip) and extract to `~/AssemblyPublicizer.exe`
* Create publizised assemblies:
    * `cd "~/Library/Application Support/Steam/steamapps/common/Subnautica/Subnautica.app/Contents/Resources/Data/Managed"`
    * `mono ~/AssemblyPublicizer.exe -i Assembly-CSharp.dll`
    * `mono ~/AssemblyPublicizer.exe -i Assembly-CSharp-firstpass.dll`
    * Copy the dlls from `publicized_assemblies/` to `Dependencies/SN.STABLE/`

## Modify scripts for SN.STABLE
* In MacOSX/dist/QModManager.sh modify the line `EXECUTABLE_NAME="SubnauticaZero.app"` to `EXECUTABLE_NAME="Subnautica.app"`
* In MacOSX/build.sh modify the line `CONFIGURATION="BZ.STABLE"` to `CONFIGURATION="SN.STABLE"`

## Build
* Create distribution package by running `MacOSX/build.sh` from the top level of the repository.

## Installation
* Copy distribution package contents to `~/Library/Application Support/Steam/steamapps/common/Subnautica`
* Update Steam Launch options to `"/Users/username/Library/Application Support/Steam/steamapps/common/Subnautica/QModManager.sh" %command%`
