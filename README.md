### Installation:

File and details available at 

https://www.nexusmods.com/subnautica/mods/16/
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

`EntryMethod` is the entry method for your patch

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
