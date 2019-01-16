File and details available at 

https://www.nexusmods.com/subnautica/mods/16/

___

### Creating mods

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
  "AssemblyName": "BestMod.dll",
  "EntryMethod": "BestMod.QMod.Patch"
}
```

_Please note that you need to add commas after each value except the last one. If you get a "mod.json deserialization failed" error, check the `mod.json` file on an online json validator._

___

### Mac Users

Refer to the README.md in the [Mac Installation](Mac Installation) folder for installation instructions.
