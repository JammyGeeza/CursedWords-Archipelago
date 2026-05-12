using BepInEx.Logging;
using Modd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private ArchipelagoSlotData(Dictionary<string, object> slotData) 
        {
            Logger.LogInfo("Slot data:");

            if (slotData.TryGetValue("deathlink", out object deathlink))
            {
                try
                {
                    Deathlink = Convert.ToBoolean(deathlink);
                }
                catch (Exception ex)
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
