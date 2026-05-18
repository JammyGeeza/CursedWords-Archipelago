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
        /// Gets or sets whether progressive grid size is enabled.
        /// </summary>
        public bool ProgressiveGridSize { get; private set; } = false;

        /// <summary>
        /// Gets or sets the locked tile positions.
        /// </summary>
        public List<(int x, int y)> ProgressiveTilePositions { get; private set; } = new List<(int x, int y)>();


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
                    Logger.LogInfo($"Goal type: {goalRequirements?.GetType().FullName}");

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

            if (slotData.TryGetValue("progressive_grid_size", out object progressiveGridSize))
            {
                try
                {
                    ProgressiveGridSize = Convert.ToBoolean(progressiveGridSize);
                }
                catch
                {
                    Logger.LogWarning("Progressive Grid Size slot data in unexpected format, defaulting to 'false'");
                }
            }

            Logger.LogInfo($"\tProgressive Grid Size: {ProgressiveGridSize}");

            if (slotData.TryGetValue("progressive_tile_positions", out object progressiveTilePositions))
            {
                try
                {
                    Logger.LogInfo($"Progressive Tile Positions type: {progressiveTilePositions?.GetType().FullName}");

                    if (progressiveTilePositions is IEnumerable<object> enumerable)
                    {
                        ProgressiveTilePositions = enumerable
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
                    Logger.LogWarning("Progressive Tile Positions slot data in unexpected format, defaulting to none");
                }
            }

            Logger.LogInfo("\tProgressive Tile Positions:");
            foreach ((int x, int y) position in ProgressiveTilePositions)
            {
                Logger.LogInfo($"\t\t{position.x},{position.y}");
            }
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
