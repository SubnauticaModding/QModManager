# SMLHelper
SMLHelper is a complete library that helps you out on your Subnautica modding adventures by making adding new items, changing items, adding models, sprites a LOT easier. Check out [the wiki page](https://github.com/SMLHelper/SMLHelper/wiki) for details on how to use it.

## Contributing
We would love to have people contribute to SMLHelper.  
To get started, first fork the repo and then clone it to your local environment.  
As of version 2.1, SMLHelper has been updated to use Harmony version 1.2.0.1.

As of version 2.2, SMLHelper is now using the _publicized_ versions of `Assembly-CSharp.dll` and `Assembly-CSharp-firstpass.dll` (the originals being located in your `Subnautica_Data/Managed` folder).
To create your own publiciezed DLLs, you will need to download or compile the [Assembly Publicizer](https://github.com/CabbageCrow/AssemblyPublicizer/releases) and follow the [instructions](https://github.com/CabbageCrow/AssemblyPublicizer/blob/master/README.md) to convert the original DLLs into `Assembly-CSharp_publicized.dll` and `Assembly-CSharp-firstpass_publicized.dll`.  
You will need to copy these DLLs into the `Dependency` folder in order to build the solution.

Then, load up the solution, make your edits, then create your Pull Request!

## Alphabetical list of mods using SMLHelper
_Moved to https://smlhelper.github.io_
