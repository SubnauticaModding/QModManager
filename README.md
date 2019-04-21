### QModManager

#### Config based patch management for Subnautica and Subnautica: Below Zero

https://www.nexusmods.com/subnautica/mods/201/  
https://www.nexusmods.com/subnauticabelowzero/mods/1/

___

#### This file will not provide a step-by-step tutorial for creating mods!

While creating a mod, you can use the [`0Harmony-1.2.0.1.dll`](https://github.com/pardeike/Harmony) found in the `Managed` folder to patch methods at runtime and change their code.

Your mod must have a `static` method with **no parameters** that must be in a class which **needs to be in a namespace**. That method will be called when the game loads to load the mod. This is usually where you want to make your calls to [SMLHelper](https://nexusmods.com/subnautica/mods/113) or patch methods using [Harmony](https://github.com/pardeike/Harmony).

The patch method and the build DLL file name will be specified in the `mod.json` file.

Example:

`Mod.cs`
```cs
using Harmony;

namespace MyNamespace
{
    class MyClass
    {
        static void PatchMethod()
        {
            HarmonyInstance.Create("AwesomeMod").PatchAll();
        }
    }
}
```

`mod.json`
```
{
  ...
  "AssemblyName": "MyMod.dll",
  "EntryMethod": "MyNamespace.MyClass.PatchMethod",
  ...
}
```

#### To support your mod for the QMods system, you need to learn how the `mod.json` file works. It contains informations about a mod, and it can have the folling keys:

- `Id`: Your unique mod id. Can only contain alphanumeric characters and underscores.  
_(required)_  
Type: `string`  
Example: `"BestMod"`

- `DisplayName`: The display name of your mod. Just like the mod id, but it can contain any type of characters.  
_(required)_  
Type: `string`  
Example: `"Best Mod"` 

- `Author`: Your username. Should be the same across all your mods. Can contain any type of characters.  
_(required)_  
Type: `string`  
Example: `"Awesome Guy"`

- `Version`: The mod version. This needs to be updated every time your mod gets updated (please update it).  
_(required)_  
Type: `string`  
Example: `"1.0.0"`

- `Dependencies`: Other mods that your mod needs. If a dependency is not found, the mod isn't loaded.  
_(optional, defaults to `[]`)_  
Type: `string[]`  
Example: `[ "DependencyModID" ]`

- `VersionDependencies`: Just like `Dependencies`, but you can specify a version range for the needed mods.  
_(optional, default to `{}`)_  
Type: `Dictionary<string, string>`  
Example: `{ "SMLHelper": "2.x", "AnotherVeryImportantMod": ">1.2.3" }`  
**Note: The versioning system which is used is SemVer. [Here](https://github.com/adamreeve/semver.net/blob/master/README.md) is a readme file with all of the possible version range declarations. Some weird things could occurr, so it's recommended that you test out your version ranges [here](https://semver.npmjs.com/).**

- `LoadBefore`: Specify mods that will be loaded after your mod. If a mod in this list isn't found, it is simply ignored.  
_(optional, defaults to `[]`)_  
Type: `string[]`  
Example: `[ "AModID", "SomeOtherModID" ]`

- `LoadAfter`: Specify mods that will be loaded before your mod. If a mod in this list isn't found, it is simply ignored.  
_(optional, defaults to `[]`)_  
Type: `string[]`  
Example: `[ "AnotherModID" ]`

- `Enable`: Whether or not to enable the mod.  
_(optional, defaults to `true`)_  
Type: `bool`  
Example: `true`

- `Game`: The game that this mod is for. Can be `"Subnautica"`, `"BelowZero"`, or `"Both"`  
_(optional, defaults to `"Subnautica"`)_  
Type: `string`  
Example: `"Subnautica"`

- `AssemblyName`: The name of the DLL file which contains the mod.  
_(required)_  
Type: `string`  
Example: `"BestMod.dll"`

- `EntryMethod`: The method which is called to load the mod. The method must be public, static, and have no parameters.  
_(required)_  
Type: `string`  
Example: `"BestMod.QMod.Patch"`

#### The final result would look something like this:

```json
{
  "Id": "BestMod",
  "DisplayName": "Best Mod",
  "Author": "Awesome Guy",
  "Version": "1.0.0",
  "Dependencies": [ "DependencyModID" ],
  "VersionDependencies": { 
    "SMLHelper": "2.x", 
    "AnotherVeryImportantMod": ">1.2.3" 
  },
  "LoadBefore": [ "AModID", "SomeOtherModID" ],
  "LoadAfter": [ "AnotherModID" ],
  "Enable": true,
  "Game": "Subnautica",
  "AssemblyName": "BestMod.dll",
  "EntryMethod": "BestMod.QMod.Patch"
}
```

_Please note that you need to add commas after each value except the last one. If you get a "mod.json deserialization failed" error, check the `mod.json` file on an online json validator._

___

### Mac Users

Refer to the `README.md` file in the [`Mac Installation`](Mac%20Installation) folder for installation instructions.
___

### Linux Users

Using [Steam Proton 4.2](https://www.protondb.com/app/264710), QModManager can be used with Subnautica, allowing you to use certain mods. To Get this working, the following actions should be taken.

- (Optionally) Use an application like Q4Wine to add the Steam Proton directory to your prefix list for easy access.
- Run the Windows installer in the Steam Proton directory `./steamapps/compatdata/264710/`
- Install QModManager into the actual Subnautica directory `./steamapps/common/Subnautica/`
- Manually install mods by adding them to the QMods folder in your Subnautica directory  `./steamapps/common/Subnautica/QMods/`
