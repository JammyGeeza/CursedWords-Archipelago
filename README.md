# Cursed Words Archipelago Randomizer
This is an [Archipelago Randomizer](https://archipelago.gg) implementation for the game [Cursed Words: The Word Game That Isn't](https://store.steampowered.com/app/3856460/Cursed_Words_The_Word_Game_That_Isnt).

## Preface
This project is currently in **<ins>early development</ins>** and is deemed **'<ins>unstable</ins>'**. This means that it is either not fully playable yet or contains bugs that disrupt the experience, so please check the [Issues Page](https://github.com/JammyGeeza/CursedWords-Archipelago/issues) and the [Discord Forum](https://discord.com/channels/731205301247803413/1499176806962692196) before choosing to include this in your multiworld.

If you are unsure, please speak to whomever is hosting your multiworld first.

## Goal
The current selectable goal(s) are as follows:
- Select a set of characters and complete a run with each character.

_More goals will be added in future development_

## Locations
The currently available locations are as follows:
- Completing each encounter with each selected character. _(E.g. Rodman Stage 1-1, 1-2, 1-3, 2-1 ... etc.)_
- Shop Actions _(E.g. Buy a Sticker, Re-stock the Shop etc.)_
- Word Lengths _(E.g. submitted word has exactly 5 tiles)_
- Word Scores _(E.g. word score is greater than 100)_

_More locations will be added in future development_

## Items
The currently available items are as follows:
### Progression
- Playable Characters
- Sticker Bundles
- Stamp Bundles
- Tile Colours _(E.g. Blue, Shiny, Void etc.)_
- Tile Glyphs _(E.g. Chess, Currency, Fraction etc.)_
### Useful
- Progressive Grid Re-rolls
### Filler
- Money

_More items will be added in future development_

# Setup Guide

## Required Software
- [Cursed Words: The Word Game That Isn't](https://store.steampowered.com/app/3856460/Cursed_Words_The_Word_Game_That_Isnt)
- [BepInEx v5.4.23.5](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5)
- [Cursed Words Archipelago Mod Client](https://github.com/JammyGeeza/CursedWords-Archipelago/releases/latest)

## Optional Software
- [Archipelago Text Client](https://github.com/ArchipelagoMW/Archipelago/releases)

## Installing BepInEx (Windows)
1. Navigate to [BepInEx v5.4.23.5](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5) and download the `BepInEx_Win_x64_5.4.23.5.zip` file.
2. In File Explorer, find your Cursed Words install folder _(in Steam, right-click the game and go to Properties -> Installed Files -> Browse)_, unzip the file and place its entire contents into the folder alongside the `Cursed Words.exe` file.
   Your file directory should look like this:
      ```
      Cursed Words/
      ├── BepInEx/
      │   └── core/
      ├── Cursed Words_Data/
      ├── ...
      ├── Cursed Words.exe
      ├── ...
      ├── doorstop_config.ini
      └── winhttp.dll
      ```
3. Launch the game - a console window should appear and the game will take slightly longer to start than normal.
4. Once the game reaches the save selection menu, exit the game. It should have created some additional files in the `/BepInEx` folder.
   It should now look like this:
      ```
      Cursed Words/
      ├── BepInEx/
      │   ├── cache/
      │   ├── config/
      │   ├── core/
      │   ├── patches/
      │   └── plugins/
      ├── Cursed Words_Data/
      ├── ...
      ├── Cursed Words.exe
      ├── ...
      ├── doorstop_config.ini
      └── winhttp.dll
      ```
5. You are now ready to install the mod client.

## Installing the Mod Client (Windows)
1. Navigate to [Cursed Words Archipelago Mod Client](https://github.com/JammyGeeza/CursedWords-Archipelago/releases/latest) and download the `CursedWords_Archipelago.zip` file from the latest release.
2. In File Explorer, find your Cursed Words install folder _(in Steam, right-click the game and go to Properties -> Installed Files -> Browse)_, unzip the file and place the contents into the newly created `~/BepInEx/plugins` folder.
   Your file directory should look like this:
   ```
   Cursed Words/
      ├── BepInEx/
      │   ├── ...
      │   └── plugins/
      │       └── Archipelago/
      │           ├── Archipelago.MultiClient.Net.dll
      │           ├── Mod.dll
      │           └── Newtonsoft.Json.dll
      ├── Cursed Words_Data/
      ├── ...
      ├── Cursed Words.exe
      ├── ...
      ├── doorstop_config.ini
      └── winhttp.dll
   ```
3. Start the game - you should notice that any previous game saves are not shown. Don't worry, this is intentional and your saves are safe! The mod client creates separate save files for Archipelago. 

## Disabling the Mod Client (Windows)
At the moment there is no in-game way to disable the mod client so you will need to remove the `/plugins/Archipelago` folder from the install directory. This should return your game to normal and your existing saves should become playable again.

## Connecting to Archipelago
1. To create a new Archipelago save, click **SELECT** on an empty save slot.
2. A dialog window should prompt for Archipelago connection details. Enter the host _(including port)_, slot name and password _(if required)_ and click **Connect**.
3. On successful connection, the game should load to the Main Menu. Otherwise, it should display an error and prompt to re-try.

## Archipelago Text Client
It is recommended to use the [Archipelago Text Client](https://github.com/ArchipelagoMW/Archipelago/releases) to keep track of items you have sent and received. There is currently no in-game way to review the history of sent/received items. Additionally, there is currently no in-game console to use hints, so you will need to do these via the Archipelago Text Client.
