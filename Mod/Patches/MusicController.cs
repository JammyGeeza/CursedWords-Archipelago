using HarmonyLib;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(MusicController))]
    internal class MusicController_Patches
    {
        /// <summary>
        /// Check location when an encounter is won.
        /// </summary>
        [HarmonyPatch(nameof(MusicController.OnWinOrLoseEncounter))]
        [HarmonyPostfix]
        private static void OnWinOrLoseEncounter_Postfix(bool isWin)
        {
            Debug.Log($"{nameof(MusicController)}.{nameof(MusicController.OnWinOrLoseEncounter)} Postfix!");
            Debug.Log($"Encounter win: {isWin}");

            // Ignore if not win
            if (!isWin)
            {
                return;
            }

            Player player = GameStatics.GetPlayer();
            Debug.Log($"Encounter won: {player.CurrentRunProgress.CurrentStage}/{player.CurrentRunProgress.CurrentNodeType}");

            // Try and check encounter location
            CursedWordsArchipelago.Instance.TryCheckEncounterLocations(player.MyCharacter, player.CurrentRunProgress.CurrentStage, player.CurrentRunProgress.CurrentNodeType);
        }
    }
}
