using BepInEx.Logging;
using HarmonyLib;
using Mod.Classes;
using Mod.Extensions;
using Mod.Helpers;
using Modd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mod.Mappings
{
    public enum ActionCue
    {
        None,
        Encounter,
        // Other cues here, if any...
    }

    public class CuedAction
    {
        /// <summary>
        /// The action to be performed.
        /// </summary>
        public Func<IEnumerator<bool>> Action { get; set; }

        /// <summary>
        /// The cue required for the action to be performed.
        /// </summary>
        public ActionCue Cue { get; set; } = ActionCue.None;

        public CuedAction(Func<IEnumerator<bool>> action, ActionCue cue = ActionCue.None)
        {
            Action = action;
            Cue = cue;
        }
    }

    public static class Items
    {
        private static ManualLogSource Logger
        {
            get => CursedWordsArchipelago.Instance.LogSource;
        }

        public static Dictionary<string, CuedAction> AllItems = new Dictionary<string, CuedAction>()
        {
            // Characters
            // NOTE: These are only being unlocked so they appear in the save selection menu.
            { "Rodman", new CuedAction(() => UnlockCharacter(typeof(WetDennis))) },
            { "Nina Nix", new CuedAction(() => UnlockCharacter(typeof(NinaNix))) },
            { "Hayley Bayles", new CuedAction(() => UnlockCharacter(typeof(HayleyBayles))) },
            { "Bones the Dog", new CuedAction(() => UnlockCharacter(typeof(BonesTheDog))) },
            { "Sam Gambit", new CuedAction(() => UnlockCharacter(typeof(SamGambit))) },
            { "Octacles", new CuedAction(() => UnlockCharacter(typeof(Octacles))) },

            // Re-rolls
            { "Progressive Encounter Re-roll", new CuedAction(() => IncrementReroll()) },

            // Slots
            { "Progressive Stamp Slot", new CuedAction(() => FreeStampSlot()) },
            { "Progressive Sticker Slot", new CuedAction(() => FreeStickerSlot()) },

            // Traps
            { "Force Grid Re-roll", new CuedAction(() => ForceReroll(), ActionCue.Encounter) },
            { "Lose Money", new CuedAction(() => DecrementMoney(3), ActionCue.Encounter) },

            // Filler
            { "$1", new CuedAction(() => IncrementMoney(1), ActionCue.Encounter) },
            { "$2", new CuedAction(() => IncrementMoney(2), ActionCue.Encounter) },
            { "$3", new CuedAction(() => IncrementMoney(3), ActionCue.Encounter) },
            { "Consumable Tile", new CuedAction(() => AddRandomConsumableTile(), ActionCue.Encounter) },
            { "Extra Re-roll", new CuedAction(() => IncrementReroll(true), ActionCue.Encounter) },
            { "Random Tile Boost", new CuedAction(() => RandomTileBoost(), ActionCue.Encounter) },
        };

        
        /// <summary>
        /// Add a random consumable tile to the player inventory.
        /// </summary>
        private static IEnumerator<bool> AddRandomConsumableTile()
        {
            bool success = false;

            Logger.LogDebug("Attempting to add a random consumable tile...");

            if (CharacterInfoPanel.SingletonInventoryVisualController != null)
            {
                try
                {
                    // Generate random letter tile
                    Tile tile = RandomItemHelper.GenerateRandomLetterTile();

                    // Add tile to inventory
                    Player player = GameStatics.GetPlayer();
                    player.AddTileToInventory(tile);

                    success = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to add consumable tile. Reason: {ex.Message}");
                }

                // Re-populate tiles, just to be safe
                CharacterInfoPanel.SingletonInventoryVisualController.PopulateTiles();
            }
            else
            {
                Logger.LogDebug($"No active character info panel - deferring...");
            }

            yield return success;
        }

        /// <summary>
        /// Decrement the player's money by a specified amount.
        /// </summary>
        /// <param name="amount">The amount to decrement by.</param>
        static IEnumerator<bool> DecrementMoney(int amount)
        {
            bool success = false;

            Logger.LogDebug($"Attempting to decrement money by -${amount}...");

            if (CharacterInfoPanel.SingletonInventoryVisualController != null)
            {
                try
                {
                    Player player = GameStatics.GetPlayer();
                    player.ChangeMoney(-amount);

                    success = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to decrement money. Reason: {ex.Message}");
                }

                // Re-populate money, just to be safe
                CharacterInfoPanel.SingletonInventoryVisualController.PopulateCash();
            }
            else
            {
                Logger.LogDebug("No active inventory found - deferring...");
            }

            yield return success;
        }

        /// <summary>
        /// Force the current grid to be re-rolled.
        /// </summary>
        static IEnumerator<bool> ForceReroll()
        {
            bool success = false;

            Logger.LogDebug("Attempting force grid re-roll...");

            if (UnityEngine.Object.FindFirstObjectByType<EncounterController>() is EncounterController encounterController && encounterController != null)
            {
                // If waiting for user submission, activate re-roll
                if (encounterController.GetCurrentEncounterThreadStage() == EncounterThreadStage.WaitingForWordSubmission)
                {
                    if (encounterController.GetTileSelectionManager() is  TileSelectionManager tsManager)
                    {
                        // Clear player-selected tiles and block player input before the re-roll 
                        tsManager.SelectionCancelledCallback();
                        tsManager.SetIsInputBlocked(true);


                        // Transition the grid
                        bool gridTransitioned = false;
                        CoroutineHelper.Instance.StartCoroutine(
                            RunToCompletion(encounterController.GetTransitionGridOutAndIn(true), () => gridTransitioned = true));
                        while (!gridTransitioned)
                        {
                            yield return false;
                        }

                        // Un-block input
                        tsManager.SetIsInputBlocked(false);
                        success = true;
                    }
                    else
                    {
                        Logger.LogDebug($"No tile selection manager found - deferring...");
                    }
                }
                else
                {
                    Logger.LogDebug($"Current encounter is not in correct state - deferring...");
                }
            }
            else
            {
                Logger.LogDebug("No currently active encounter found - deferring...");
            }

            yield return success;
        }

        /// <summary>
        /// Remove an APStampPadlock from the player's inventory.
        /// </summary>
        static IEnumerator<bool> FreeStampSlot()
        {
            bool success = false;

            Logger.LogDebug("Attempting to free a stamp slot in inventory...");

            if (CharacterInfoPanel.SingletonInventoryVisualController != null)
            {
                try
                {
                    // Remove a stamp padlock, if one exists
                    Player player = GameStatics.GetPlayer();
                    if (player.GetStamps().FirstOrDefault(itm => itm is APStampPadlock) is APStampPadlock stampPadlock && stampPadlock != null)
                    {
                        player.RemoveItemFromInventory(stampPadlock);
                    }

                    // If in the shop, re-populate shop items in case a player could now purchase an item
                    if (UnityEngine.Object.FindFirstObjectByType<ShopController>() is ShopController shopController && shopController != null)
                    {
                        // Refresh each stamp in stock
                        List<ItemInStock> stampsInStock = shopController.GetStampsInStock();
                        for (int i = 0; i < stampsInStock.Count; i++)
                        {
                            ItemInStock stampInStock = stampsInStock[i];
                            if (stampInStock != null && stampInStock.MyItem != null)
                            {
                                shopController.CallPopulateStampInStock(stampInStock, i, stampInStock.IsFirstDiscount, stampInStock.IsFree);
                            }
                        }
                    }

                    success = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to free a stamp slot. Reason: {ex.Message}");
                }

                // Repopulate stamps, just to be safe
                CharacterInfoPanel.SingletonInventoryVisualController.PopulateStamps();
            }
            else
            {
                Logger.LogDebug("No currently active inventory found - deferring...");

                // Doesn't rely on this working on-the-spot, as a run will infer unlocked stamp count on load anyway
                success = true;
            }

            yield return success;
        }

        /// <summary>
        /// Remove an APStickerPadlock from the player's inventory.
        /// </summary>
        static IEnumerator<bool> FreeStickerSlot()
        {
            bool success = false;

            Logger.LogDebug("Attempting to free a sticker slot...");

            if (CharacterInfoPanel.SingletonInventoryVisualController != null)
            {
                try
                {
                    // Remove a sticker padlock, if one exists
                    Player player = GameStatics.GetPlayer();
                    if (player.GetStickers().FirstOrDefault(itm => itm is APStickerPadlock) is APStickerPadlock stickerPadlock && stickerPadlock != null)
                    {
                        Logger.LogInfo("Removing sticker padlock...");
                        player.RemoveItemFromInventory(stickerPadlock);
                    }

                    // If in the shop, re-populate shop items in case a player could now purchase an item
                    if (UnityEngine.Object.FindFirstObjectByType<ShopController>() is ShopController shopController && shopController != null)
                    {
                        // Refresh each sticker in stock
                        List<ItemInStock> stickersInStock = shopController.GetStickersInStock().ToList();
                        for (int i = 0; i < stickersInStock.Count; i++)
                        {
                            ItemInStock stickerInStock = stickersInStock[i];
                            if (stickerInStock != null && stickerInStock.MyItem != null)
                            {
                                shopController.PopulateStickerInStock(stickerInStock, i, stickerInStock.IsFirstDiscount, stickerInStock.IsFree);
                            }
                        }
                    }

                    success = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to free a sticker slot. Reason: {ex.Message}");
                }

                // Repopulate stickers, just to be safe
                CharacterInfoPanel.SingletonInventoryVisualController.PopulateStickers();
            }
            else
            {
                Logger.LogDebug("No currently active inventory found - deferring...");

                // Doesn't rely on this working on-the-spot, as a run will infer unlocked sticker count on load anyway
                success = true;
            }

            yield return success;
        }

        /// <summary>
        /// Increment the player's money by a specified amount.
        /// </summary>
        /// <param name="amount">The amount to increment by.</param>
        static IEnumerator<bool> IncrementMoney(int amount)
        {
            bool success = false;

            Logger.LogDebug($"Attempting to increment money by ${amount}...");

            if (CharacterInfoPanel.SingletonInventoryVisualController != null)
            {
                try
                {
                    Player player = GameStatics.GetPlayer();
                    player.ChangeMoney(amount);

                    success = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to increment money. Reason: {ex.Message}");
                }

                // Re-populate money, just to be safe
                CharacterInfoPanel.SingletonInventoryVisualController.PopulateCash();
            }
            else
            {
                Logger.LogDebug("No currently active inventory found - deferring...");
            }

            yield return success;
        }

        /// <summary>
        /// Increment the user's current remaining re-roll attempts.
        /// </summary>
        /// <param name="isTemporary">Whether this is a temporary or permanent increase.</param>
        static IEnumerator<bool> IncrementReroll(bool isTemporary = false)
        {
            bool success = false;

            Logger.LogDebug($"Attempting to increment re-roll count (Temporary: {isTemporary})...");
            
            if (UnityEngine.Object.FindFirstObjectByType<EncounterController>() is EncounterController encounterController && encounterController != null)
            {
                try
                {
                    encounterController.IncrementEncounterRerollAmount(1);
                    success = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to increment re-roll. Reason: {ex.Message}");
                }
            }
            else
            {
                Logger.LogDebug("No currently active encounter found - deferring...");

                // If not temporary, treat as success as re-roll count is inferred on encounter load anyway
                if (!isTemporary)
                {
                    success = true;
                }
            }

            yield return success;
        }

        static IEnumerator<bool> RandomTileBoost()
        {
            bool success = false;

            Logger.LogDebug("Attempting to boost a random tile...");

            // Get current encounter
            if (UnityEngine.Object.FindFirstObjectByType<EncounterController>() is EncounterController encounterController && encounterController != null)
            {
                if (encounterController.IsWaitingForWordSubmission())
                {
                    try
                    {
                        // Get grid layout controller
                        GridLayoutController gridLayoutController = Traverse.Create(encounterController)
                            .Field("_gridLayoutController")
                            .GetValue<GridLayoutController>();

                        // Get active tiles
                        List<TileObject> activeTiles = gridLayoutController.GetTileObjects()
                            .Where(t => t != null && t.isActiveAndEnabled)
                            .ToList();

                        // Randomly select a tile and modify its value by 5
                        TileObject selectedTile = activeTiles[UnityEngine.Random.Range(0, activeTiles.Count)];
                        int modifyBy = selectedTile.MyTile.GetTileType() != TileType.Void ? 5 : -5;
                        selectedTile.MyTile.ChangeValueModifier(new ScorePacket(modifyBy));

                        Logger.LogDebug($"Boosted tile '{selectedTile.MyTile.Letter}' at position '{selectedTile.MyTile.Coordinates}'");

                        // Re-populate the tile
                        selectedTile.Populate();

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Failed to boost random tile. Reason: {ex.Message}");
                    }
                }
                else
                {
                    Logger.LogDebug("Active counter is not in the correct state - deferring...");
                }
            }
            else
            {
                Logger.LogDebug("No currently active encounter found - deferring...");
            }

            yield return success;
        }

        /// <summary>
        /// Run a coroutine to completion and perform an action on complete.
        /// </summary>
        /// <param name="routine">The coroutine to run.</param>
        /// <param name="onCompletion">The action to perform on completion.</param>
        private static IEnumerator RunToCompletion(IEnumerator coroutine, Action onCompletion)
        {
            yield return CoroutineHelper.Instance.StartCoroutine(coroutine);
            onCompletion();
        }

        static IEnumerator<bool> UnlockCharacter(Type characterType)
        {
            bool success = false;

            Logger.LogDebug($"Attempting to unlock character '{characterType.Name}'...");

            try
            {
                SaveManager.AddCharacterToUnlockedCharacters(characterType);
                success = true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to unlock character. Reason: {ex.Message}");
            }

            yield return success;
        }
    }
}
