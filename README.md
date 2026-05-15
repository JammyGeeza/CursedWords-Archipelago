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

## Setup Guide
### Requirements
- The [Cursed Words: The Word Game That Isn't](https://store.steampowered.com/app/3856460/Cursed_Words_The_Word_Game_That_Isnt) game via Steam
- Latest version of the [Archipelago Launcher](https://github.com/ArchipelagoMW/Archipelago/releases/latest)
- [BepInEx v5.4.23.5](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5)
- Latest version of the [Cursed Words Archipelago](https://github.com/JammyGeeza/CursedWords-Archipelago/releases/latest) Mod

### Installing Archipelago
Please visit the [Archipelago Setup Guide](https://archipelago.gg/tutorial/Archipelago/setup_en) for full instructions on how to install the Archipelago Launcher and generate games with YAML files.

### Installing BepInEx
1. Unzip the entire contents of the `BepInEx_Win_x64_5.4.23.5.zip` file into your Cursed Words install folder, your install location should look like this:
   
   <img width="728" height="486" alt="image" src="https://github.com/user-attachments/assets/52d922b2-2bd2-4764-9f64-a83eb4057bd9" />
2. Start the game - a console window should appear for a few seconds as the game boots. This should then create some additional folders (cache, config, core, patchers, plugins) in the `~/BepInEx/` folder and look like this:
   
   <img width="811" height="305" alt="image" src="https://github.com/user-attachments/assets/66024f06-5e30-4c0c-97da-a15c96313aae" />
3. Close the game after it successfully launches to the save selection menu.

### Installing the Mod
1. Once BepInEx has been installed as per the instructions above, unzip the `CursedWords_Archipelago.zip` file and place the contents into the newly created `~/BepInEx/plugins` folder. It should look like this:

   <img width="903" height="179" alt="image" src="https://github.com/user-attachments/assets/6eaef6f3-7324-4a95-b00a-0cf7da17d0fa" />
2. Start the game.

### Connecting to Archipelago
1. After starting the game, your original game saves should not be visible - don't worry, they are safe! the mod creates separate saves for Archipelago.
2. To create a new Archipelago save, click `SELECT` on an empty save slot. To load an existing Archipelago save, click `SELECT` on a filled save slot:
   
   <img width="1922" height="1112" alt="image" src="https://github.com/user-attachments/assets/7d76425f-f595-449d-8076-83c914036605" />
3. A pop-up should appear to enter the Archipelago details. Enter the host _(including port)_, the slot name and a password _(if required)_.
   If you are loading an existing save, the last known connection details for that save should be auto-populated.

   <img width="560" height="384" alt="image" src="https://github.com/user-attachments/assets/d6830c14-ad09-454c-8347-b112f29339b1" />
4. Click 'Connect' - if successful it will load to the Main Menu. Otherwise, it should display an error and allow a retry.
5. Start playing!
