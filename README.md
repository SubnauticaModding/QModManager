# QModManager

The [Subnautica modding discord](https://discord.gg/UpWuWwq) has the most up-to-date information and is the best place to ask for help.

### What is this?

Config based patch management for Subnautica and Subnautica: Below Zero

## How do I install it?

### Mod Manager

QMods is available for several mod managers. Typically they will install QMods for you as a dependency of whichever mod you download.

* [Thunderstore](https://subnautica.thunderstore.io/package/Subnautica_Modding/QModManager/)
* [Nexus]( https://www.nexusmods.com/subnautica/mods/201)

### Manual

Extract the relevant [zip file](https://github.com/SubnauticaModding/QModManager/releases) into the `Subnautica` directory. There are several zip files, one for each version of the game.

* Subnautica - SN.STABLE
* Below Zero - BZ.STABLE

You will end up with a folder like this:
```
Subnautica
  - QMods/
  - BepinEx/
  - doorstop_config.ini
  - winhttp.dll
  - Subnautica.exe 
```

Each mod should have one folder in the `QMods` directory.

## Does it work with Nitrox?

Each mod needs to specify that it's compatible with Nitrox in their "mod.json" file.
```
{
  "name": "example_mod",
  "nitroxcompat": true
}
```

## How do I create a mod?

mroshaw created an [excellent guide](https://mroshaw.github.io/Subnautica) to get started.

For more links check the [#modding-resources](https://discord.com/channels/324207629784186882/664594296778915850) channel on discord.

## Where should I publish my mod?

There are a few places to publish mods.

* [Thunderstore](https://subnautica.thunderstore.io/)
* [Nexus](https://www.nexusmods.com/)
* Github

Note: Many mods (such as Nitrox) have decided not to publish on Nexus Mods due to their [Terms Of Service](https://help.nexusmods.com/article/18-terms-of-service#RightsYouAre), which state they "may retain your content indefinitely and are not obliged to delete your content if you so request".

For more questions, please [consult the wiki](https://github.com/SubnauticaModding/QModManager/wiki) or ask a question on [the discord](https://discord.gg/UpWuWwq).
