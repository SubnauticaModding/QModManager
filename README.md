### Installation:

1. Unzip the contents of [QModManager.zip](https://github.com/Qwiso/QModManager/releases/download/1.0.0/QModManager.zip) to  
`steamapps\common\Subnautica\Subnautica_Data\ManagedSubnautica\Subnautica_Data\Managed`  
2. Run `QModManager.exe`  
3. Follow the command prompt  

That's it!  
If you want to uninstall QModManager, run the exe again  
___

### Using QMods

There will be a new folder called `QMods`located at `steamapps\common\Subnautica\QMods`. This is where mods marked for support by QModManager should be placed
___

### Note to developers

To support your mod for the QMods system, you need to learn how `mod.json` is implemented (or will be, once complete). The critical keys are:  

```
{
  "Id":"snhardcoreplus.mod",
  "DisplayName":"Subnautica Hardcore Plus",
  "Author":"Qwiso",
  "Version":"1.0.0",
  "Requires":[],
  "Enable":true,
  "AssemblyName":"SNHardcorePlus.dll",
  "EntryMethod":"SNHardcorePlus.QPatch.Patch",
  "Config":{}
}
```

`AssemblyName` must be the case sensitive name of the dll file containing your patching method

`EntryMethod` is the entry method for your patch. QMods supports a default of `YOURNAMESPACE.QPatch.Patch` but any correctly targeted method should work

```cs
using Harmony;

namespace YOURNAMESPACE
{
    class QPatch()
    {
        public static void Patch()
        {
            // Harmony.PatchAll() or equivalent
        }
    }
}
```
