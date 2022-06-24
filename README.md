# QModManager

The [Subnautica Modding Discord](https://discord.gg/UpWuWwq) has the most up-to-date information and is the best place to ask for help.

### What is this?

Config based patch management for Subnautica and Subnautica: Below Zero

## How do I install it?

### Manual - Mod-Hosting Websites

QMods is available for several mod-hosting websites. Typically they will install QMods for you as a dependency of whichever mod you download.

Subnautica

* [Thunderstore](https://subnautica.thunderstore.io/package/Subnautica_Modding/QModManager/)
* [Nexus](https://www.nexusmods.com/subnautica/mods/201)

Below Zero

* [Thunderstore](https://belowzero.thunderstore.io/package/Subnautica_Modding/QModManager_BZ/)
* [Nexus](https://www.nexusmods.com/subnauticabelowzero/mods/1)

### Manual - GitHub

Extract the relevant [zip file](https://github.com/SubnauticaModding/QModManager/releases) into the `Subnautica` directory. There are several zip files, one for each version of the game.

* Subnautica - SN.STABLE
* Below Zero - BZ.STABLE

You will end up with a folder like this:
```markdown
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
```json
{
  "Id": "MyMod",
  "NitroxCompat": true
}
```

## How do I create a mod?

Mroshaw created an [excellent guide](https://mroshaw.github.io/Subnautica) to get started.

For more links check the [#modding-resources](https://discord.com/channels/324207629784186882/664594296778915850) channel on discord.

## Where should I publish my mod?

There are a few places to publish mods.

* [Thunderstore](https://subnautica.thunderstore.io/)
* [Nexus](https://www.nexusmods.com/)
* [GitHub](https://www.github.com/)

For more questions, please [consult the wiki](https://github.com/SubnauticaModding/QModManager/wiki) or ask a question on [the Discord](https://discord.gg/UpWuWwq).
