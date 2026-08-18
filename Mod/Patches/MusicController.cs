using HarmonyLib;
using Mod.Enums;
using Mod.Extensions;
using Mod.Helpers;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(MusicController))]
    internal class MusicController_Patches : PatchBase
    {
        public static bool IgnoreDeath { get; set; } = false;

        /// <summary>
        /// Check location when an encounter is won.
        /// </summary>
        [HarmonyPatch(nameof(MusicController.OnWinOrLoseEncounter))]
        [HarmonyPostfix]
        private static void OnWinOrLoseEncounter_Postfix(bool isWin)
        {
            Logger.LogInfo($"{nameof(MusicController)}.{nameof(MusicController.OnWinOrLoseEncounter)} Postfix!");
            Logger.LogInfo($"Encounter win: {isWin}");

            Player player = GameStatics.GetPlayer();

            if (isWin)
            {
                if (UnityEngine.Object.FindFirstObjectByType<EncounterController>() is EncounterController controller && controller != null)
                {
                    // Try and check encounter location(s)
                    CursedWordsArchipelago.Instance.TryCheckEncounterLocations("win_encounter", player, controller.GetBossModifiers());
                }
                else
                {
                    Logger.LogWarning("Encounter has been won, but no encounter controller was found.");
                }
            }
            else if (!isWin && ArchipelagoHelper.SlotData.Deathlink)
            {
                // Ignore means it was likely caused by receiving a deathlink
                if (!IgnoreDeath)
                {
                    Logger.LogInfo("Attempting to send deathlink...");
                    ArchipelagoHelper.TrySendDeathlink($"Failed to beat Stage {player.CurrentRunProgress.CurrentStage}");
                }

                IgnoreDeath = false;
            }
        }

        /// <summary>
        /// Check location when an encounter is won.
        /// </summary>
        [HarmonyPatch(nameof(MusicController.OnWinMichaelEncounter))]
        [HarmonyPostfix]
        private static void OnWinMichaelEncounter_Postfix()
        {
            Logger.LogInfo($"{nameof(MusicController)}.{nameof(MusicController.OnWinMichaelEncounter)} postfix!");

            // Attempt to send run win check (Stage 5-3)
            Player player = GameStatics.GetPlayer();

            // Attempt to send run win check (Stage 6-3)
            CursedWordsArchipelago.Instance.TryCheckEncounterLocations("win_encounter", player);

            // Is goal type michael?
            if (ArchipelagoHelper.SlotData.GoalType is GoalType.Michael)
            {
                // Check if all characters have beaten michael
                int beatenMichaelCount = SaveManager.GetCharacterHasBeatenFinalBossAmount();
                if (beatenMichaelCount == ArchipelagoHelper.SlotData.Characters.Length)
                {
                    Logger.LogInfo("Goal condition has been reached!");
                    ArchipelagoHelper.TryGoal();
                }
            }
        }

        /// <summary>
        /// Check location when an encounter is won.
        /// </summary>
        [HarmonyPatch(nameof(MusicController.OnWinRun))]
        [HarmonyPostfix]
        private static void OnWinRun_Postfix()
        {
            Logger.LogInfo($"{nameof(MusicController)}.{nameof(MusicController.OnWinRun)} postfix!");

            Player player = GameStatics.GetPlayer();

            if (UnityEngine.Object.FindFirstObjectByType<EncounterController>() is EncounterController controller && controller != null)
            {
                // Try and check encounter location check (needs encounter controller for boss checks)
                CursedWordsArchipelago.Instance.TryCheckEncounterLocations("win_encounter", player, controller.GetBossModifiers());
            }
            else
            {
                Logger.LogError("Run has been won, but no encounter controller was found.");
                return;
            }

            // Is the goal 'Runs'?
            if (ArchipelagoHelper.SlotData.GoalType is GoalType.Runs)
            {
                // Check if all characters have beaten at least one run
                int beatenRunCount = SaveManager.GetCharactersWonWith().Count;
                if (beatenRunCount >= ArchipelagoHelper.SlotData.Characters.Length)
                {
                    Logger.LogInfo("Goal condition has been reached!");
                    ArchipelagoHelper.TryGoal();
                }
            }
            // Is the goal 'Crowns'?
            else if (ArchipelagoHelper.SlotData.GoalType is GoalType.Crowns)
            {
                // Check if all characters have beaten the goal crown
                int beatenCrownCount = SaveManager.GetHighestCompletedAscensions()
                    .Select((key, val) => val)
                    .Where(v => v >= ArchipelagoHelper.SlotData.HighestCrown)
                    .Count();
                if (beatenCrownCount >= ArchipelagoHelper.SlotData.Characters.Length)
                {
                    Logger.LogInfo("Crowns goal condition has been reached!");
                    ArchipelagoHelper.TryGoal();
                }

            }
        }
    }
}
