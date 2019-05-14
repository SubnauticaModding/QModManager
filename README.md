### QModManager
[![All Contributors](https://img.shields.io/badge/all_contributors-3-orange.svg?style=flat-square)](#contributors)

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

## Contributors

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore -->
<table><tr><td align="center"><a href="https://github.com/AlexejheroYTB"><img src="https://avatars3.githubusercontent.com/u/32238504?v=4" width="100px;" alt="AlexejheroYTB"/><br /><sub><b>AlexejheroYTB</b></sub></a><br /><a href="https://github.com/QModManager/QModManager/commits?author=AlexejheroYTB" title="Code">💻</a> <a href="#maintenance-AlexejheroYTB" title="Maintenance">🚧</a> <a href="#question-AlexejheroYTB" title="Answering Questions">💬</a> <a href="https://github.com/QModManager/QModManager/commits?author=AlexejheroYTB" title="Documentation">📖</a> <a href="#ideas-AlexejheroYTB" title="Ideas, Planning, & Feedback">🤔</a> <a href="#projectManagement-AlexejheroYTB" title="Project Management">📆</a></td><td align="center"><a href="https://github.com/Qwiso"><img src="https://avatars1.githubusercontent.com/u/4432563?v=4" width="100px;" alt="Zachary Jones"/><br /><sub><b>Zachary Jones</b></sub></a><br /><a href="https://github.com/QModManager/QModManager/commits?author=qwiso" title="Code">💻</a> <a href="#content-qwiso" title="Content">🖋</a> <a href="#tool-qwiso" title="Tools">🔧</a> <a href="#infra-qwiso" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="#example-qwiso" title="Examples">💡</a></td><td align="center"><a href="https://github.com/RandyKnapp"><img src="https://avatars1.githubusercontent.com/u/3331569?v=4" width="100px;" alt="Randy Knapp"/><br /><sub><b>Randy Knapp</b></sub></a><br /><a href="https://github.com/QModManager/QModManager/commits?author=RandyKnapp" title="Code">💻</a> <a href="#design-RandyKnapp" title="Design">🎨</a></td></tr></table>

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!