using HarmonyLib;
using Mod.Extensions;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(EncounterController))]
    internal class EncounterController_Patches : PatchBase
    {
        /// <summary>
        /// On game setup, adjust re-roll amount per encounter.
        /// </summary>
        [HarmonyPatch("GameSetup")]
        [HarmonyPostfix]
        private static IEnumerator OnGameSetup_Postfix(IEnumerator __result, EncounterController __instance)
        {
            Logger.LogInfo("EncounterController.GameSetup postfix!");

            // Perform existing actions in coroutine
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }

            // Set re-roll amount per encounter
            int rerollsReceived = ArchipelagoHelper.AmountOfItemReceived("Progressive Grid Re-roll");
            __instance.SetEncounterRerollAmount(rerollsReceived);
        }

        /// <summary>
        /// Check location when a stamp or sticker is sold.
        /// </summary>
        [HarmonyPatch("SellItem")]
        [HarmonyPostfix]
        private static void SellItem_Postfix(Item item)
        {
            Logger.LogInfo("EncounterController.SellItem Postfix!");
            Logger.LogInfo($"Sold item: {item.Name}");

            // Attempt to check shop locations
            CursedWordsArchipelago.Instance.TryCheckGenericLocations($"sell_{(item.IsStamp() ? "stamp" : "sticker")}");
        }

        /// <summary>
        /// Check location(s) when a word is scored.
        /// </summary>
        [HarmonyPatch("ShowScoreCalculation")]
        [HarmonyPrefix]
        private static void ShowScoreCalculation_Prefix(ScorePacket finalScore)
        {
            Logger.LogInfo($"{nameof(EncounterController)}.ShowScoreCalculation Prefix!");
            Logger.LogInfo($"Word score: {finalScore.Score}");

            // Attempt to check word length locations
            CursedWordsArchipelago.Instance.TryCheckNumericLocations("word_score", finalScore.Score);
        }

        /// <summary>
        /// On word submission skip, check the skip location.
        /// </summary>
        [HarmonyPatch(nameof(EncounterController.SkipWordSubmission))]
        [HarmonyPostfix]
        private static void OnSkipWordSubmission_Postfix(EncounterController __instance)
        {
            Logger.LogInfo($"{nameof(EncounterController)}.{nameof(EncounterController.SkipWordSubmission)} postfix!");

            // Check the 'Skip a Grid' location
            CursedWordsArchipelago.Instance.TryCheckGenericLocations("skip_grid");
        }

        /// <summary>
        /// Check location when a word length is submitted.
        /// </summary>
        [HarmonyPatch(nameof(EncounterController.SubmitWord))]
        [HarmonyPostfix]
        private static void SubmitWord_Postfix(EncounterController __instance, List<TileSelection> tiles)
        {
            Logger.LogInfo($"{nameof(EncounterController)}.{nameof(EncounterController.SubmitWord)} Postfix!");
            Logger.LogInfo($"Word length: {tiles.Count}");

            // Attempt to check word length locations
            CursedWordsArchipelago.Instance.TryCheckNumericLocations("word_length", tiles.Count);
        }
    }
}
