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
            Logger.LogDebug($"{nameof(MusicController)}.{nameof(MusicController.OnWinOrLoseEncounter)} Postfix!");
            Logger.LogDebug($"Encounter win: {isWin}");

            Player player = GameStatics.GetPlayer();

            if (isWin)
            {
                if (UnityEngine.Object.FindFirstObjectByType<EncounterController>() is EncounterController controller && controller != null)
                {
                    // Try and check encounter location(s)
                    CursedWordsArchipelago.Instance.TryCheckEncounterLocations(
                        "win_encounter",
                        player,
                        controller.GetBossModifiers(),
                        controller.GetRemainingGrids());
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
            Logger.LogDebug($"{nameof(MusicController)}.{nameof(MusicController.OnWinMichaelEncounter)} postfix!");

            // Attempt to send run win check (Stage 6-3)
            Player player = GameStatics.GetPlayer();

            if (UnityEngine.Object.FindFirstObjectByType<EncounterController>() is EncounterController controller && controller != null)
            {
                // Try and check encounter location check (needs encounter controller for boss checks)
                CursedWordsArchipelago.Instance.TryCheckEncounterLocations("win_encounter", player);
            }
            else
            {
                Logger.LogError("Michael encounter has been won, but no active encounter controller was found.");
                return;
            }
        }

        /// <summary>
        /// Check location when an encounter is won.
        /// </summary>
        [HarmonyPatch(nameof(MusicController.OnWinRun))]
        [HarmonyPostfix]
        private static void OnWinRun_Postfix()
        {
            Logger.LogDebug($"{nameof(MusicController)}.{nameof(MusicController.OnWinRun)} postfix!");

            Player player = GameStatics.GetPlayer();

            if (UnityEngine.Object.FindFirstObjectByType<EncounterController>() is EncounterController controller && controller != null)
            {
                // Try and check encounter location check (needs encounter controller for boss checks)
                CursedWordsArchipelago.Instance.TryCheckEncounterLocations("win_encounter", player, controller.GetBossModifiers());
            }
            else
            {
                Logger.LogError("Run has been won, but no active encounter controller was found.");
                return;
            }
        }
    }
}
