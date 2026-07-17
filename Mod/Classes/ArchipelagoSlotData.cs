using BepInEx.Logging;
using Modd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Mod.Helpers
{
    public class ArchipelagoSlotData
    {
        private static ManualLogSource Logger
        {
            get => CursedWordsArchipelago.Instance.LogSource;
        }

        /// <summary>
        /// Gets a value indicating whether Deathlink is enabled.
        /// </summary>
        public bool Deathlink { get; private set; }

        /// <summary>
        /// Gets or sets the goal requirements.
        /// </summary>
        public string[] GoalRequirements { get; private set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets whether shuffle grid size is enabled.
        /// </summary>
        public bool ShuffleGridSize { get; private set; } = false;

        /// <summary>
        /// Gets or sets whether shuffle inventory slots is enabled.
        /// </summary>
        public bool ShuffleInventorySlots { get; private set; } = false;

        /// <summary>
        /// Gets or sets whether shuffle item rarities is enabled.
        /// </summary>
        public bool ShuffleItemRarities { get; private set; } = false;

        /// <summary>
        /// Gets or sets whether shuffle locked tile positions is enabled.
        /// </summary>
        public bool ShuffleLockedTilePositions { get; private set; } = false;

        /// <summary>
        /// Gets or sets the shuffled locked tile positions coordinates
        /// </summary>
        public List<(int x, int y)> ShuffleLockedTilePositionsCoords { get; private set; } = new List<(int x, int y)>();

        /// <summary>
        /// Gets or sets whether shopsanity is enabled.
        /// </summary>
        public bool Shopsanity { get; private set; } = false;

        /// <summary>
        /// Gets or sets the amount that shopsanity items should cost.
        /// </summary>
        public int ShopsanityCost { get; private set; } = 12;


        private ArchipelagoSlotData(Dictionary<string, object> slotData)
        {
            Logger.LogInfo("Slot data:");

            if (slotData.TryGetValue("deathlink", out object deathlink))
            {
                try
                {
                    Deathlink = Convert.ToBoolean(deathlink);
                }
                catch
                {
                    Logger.LogWarning("Deathlink slot data in unexpected format, defaulting to 'false'");
                }
            }

            Logger.LogInfo($"\tDeathlink: {Deathlink}");

            if (slotData.TryGetValue("goal", out object goalRequirements))
            {
                try
                {
                    if (goalRequirements is IEnumerable enumerable)
                    {
                        GoalRequirements = enumerable
                            .Cast<object>()
                            .Select(x => x.ToString())
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToArray();
                    }
                    else
                    {
                        throw new FormatException();
                    }

                }
                catch (Exception ex)
                {
                    Logger.LogError("Goal requirements in invalid format, can't default.");
                    throw ex;
                }
            }

            Logger.LogInfo("\tGoal Requirements:");
            foreach (string goalRequirement in GoalRequirements)
            {
                Logger.LogInfo($"\t\t{goalRequirement}");
            }

            if (slotData.TryGetValue("shuffle_grid_size", out object shuffleGridSize))
            {
                try
                {
                    ShuffleGridSize = Convert.ToBoolean(shuffleGridSize);
                }
                catch
                {
                    Logger.LogWarning("Shuffle Grid Size slot data in unexpected format, defaulting to 'false'");
                }
            }

            Logger.LogInfo($"\tShuffle Grid Size: {ShuffleGridSize}");

            if (slotData.TryGetValue("shuffle_inventory_slots", out object shuffleInventorySlots))
            {
                try
                {
                    ShuffleInventorySlots = Convert.ToBoolean(shuffleInventorySlots);
                }
                catch
                {
                    Logger.LogWarning("Shuffle Inventory Slots slot data in unexpected format, defaulting to 'false'");
                }
            }

            Logger.LogInfo($"\tShuffle Inventory Slots: {ShuffleInventorySlots}");

            if (slotData.TryGetValue("shuffle_item_rarities", out object shuffleItemRarities))
            {
                try
                {
                    ShuffleItemRarities = Convert.ToBoolean(shuffleItemRarities);
                }
                catch
                {
                    Logger.LogWarning("Shuffle Item Rarities slot data in unexpected format, defaulting to 'false'");
                }
            }

            Logger.LogInfo($"\tShuffle Item Rarities: {ShuffleItemRarities}");

            if (slotData.TryGetValue("shuffle_locked_tile_positions", out object shuffleLockedTilePositions))
            {
                try
                {
                    ShuffleLockedTilePositions = Convert.ToBoolean(shuffleLockedTilePositions);
                }
                catch
                {
                    Logger.LogWarning("Shuffle Locked Tile Positions slot data in unexpected format, defaulting to 'false'");
                }
            }

            Logger.LogInfo($"\tShuffle Locked Tile Positions: {ShuffleLockedTilePositions}");

            if (slotData.TryGetValue("shuffle_locked_tile_positions_coords", out object shuffleLockedTilePositionsCoords))
            {
                try
                {
                    if (shuffleLockedTilePositionsCoords is IEnumerable<object> enumerable)
                    {
                        ShuffleLockedTilePositionsCoords = enumerable
                            .Cast<IEnumerable>()
                            .Where(coord => coord != null)
                            .Select(coord => coord.Cast<object>().Take(2).ToList())
                            .Where(coords => coords.Count >= 2)
                            .Select(coords => (Convert.ToInt32(coords[0]), Convert.ToInt32(coords[1])))
                            .ToList();
                    }
                    else
                    {
                        throw new FormatException();
                    }
                }
                catch
                {
                    Logger.LogWarning("Shuffle Locked Tile Positions Coordinates slot data in unexpected format, defaulting to none");
                }
            }

            if (ShuffleLockedTilePositionsCoords.Any())
            {
                Logger.LogInfo("\t\tCoordinates:");
                foreach ((int x, int y) position in ShuffleLockedTilePositionsCoords)
                {
                    Logger.LogInfo($"\t\t\t{position.x},{position.y}");
                }
            }

            if (slotData.TryGetValue("shopsanity", out object shopsanity))
            {
                try
                {
                    Shopsanity = Convert.ToBoolean(shopsanity);
                }
                catch
                {
                    Logger.LogWarning("Shopsanity slot data in unexpected format, defaulting to 'false'");
                }
            }

            Logger.LogInfo($"\tShopsanity: {Shopsanity}");

            if (slotData.TryGetValue("shopsanity_cost", out object shopsanityCost))
            {
                try
                {
                    ShopsanityCost = Convert.ToInt32(shopsanityCost);
                }
                catch
                {
                    Logger.LogWarning("Shopsanity Cost slot data in unexpected format, defaulting to '12'");
                }
            }

            Logger.LogInfo($"\tShopsanity Cost: {ShopsanityCost}");
        }

        public static ArchipelagoSlotData Parse(Dictionary<string, object> slotData)
        {
            try
            {
                return new ArchipelagoSlotData(slotData);
            }
            catch (Exception ex)
            {
                Logger.LogError($"ERROR parsing Archipelago Slot Data: {ex.Message}");
                return null;
            }
        }
    }
}
