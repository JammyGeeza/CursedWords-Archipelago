using HarmonyLib;
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
                    CursedWordsArchipelago.Instance.TryCheckEncounterLocations(
                        player.MyCharacter,
                        player.CurrentRunProgress.CurrentStage,
                        player.CurrentRunProgress.CurrentNodeType,
                        controller.GetBossModifiers());
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
        [HarmonyPatch(nameof(MusicController.OnWinRun))]
        [HarmonyPostfix]
        private static void OnWinRun_Postfix()
        {
            Logger.LogInfo($"{nameof(MusicController)}.{nameof(MusicController.OnWinRun)} postfix!");

            // Attempt to send run win check (Stage 5-3)
            Player player = GameStatics.GetPlayer();

            if (UnityEngine.Object.FindFirstObjectByType<EncounterController>() is EncounterController controller && controller != null)
            {
                CursedWordsArchipelago.Instance.TryCheckEncounterLocations(
                    player.MyCharacter,
                    player.CurrentRunProgress.CurrentStage,
                    player.CurrentRunProgress.CurrentNodeType,
                    controller.GetBossModifiers());

                List<string> runsCompleted = SaveManager.GetCharactersWithAscensionsUnlocked()
                    .Select(c => (Activator.CreateInstance(c) as Character).GetName())
                    .ToList();

                // If current win isn't there, add it to the list
                if (!runsCompleted.Contains(player.GetCharacter().GetName()))
                {
                    runsCompleted.Add(player.GetCharacter().GetName());
                }

                Logger.LogInfo("Characters currently won with:");
                foreach (string characterRunCompleted in runsCompleted)
                {
                    Logger.LogInfo($"\t{characterRunCompleted}");
                }

                // Check goal condition
                if (ArchipelagoHelper.SlotData.GoalRequirements.All(runsCompleted.Contains))
                {
                    Logger.LogInfo("Goal condition has been reached!");
                    ArchipelagoHelper.TryGoal();
                }
            }
            else
            {
                Logger.LogWarning("Run has been won, but no encounter controller was found.");
            }
        }
    }
}
