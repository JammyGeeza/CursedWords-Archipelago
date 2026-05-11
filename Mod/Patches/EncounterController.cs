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
    [HarmonyPatch(typeof(EncounterController))]
    internal class EncounterController_Patches
    {
        [HarmonyPatch("GameSetup")]
        [HarmonyPostfix]
        private static void OnGameSetup_Postfix(EncounterController __instance)
        {
            Debug.Log("EncounterController.GameSetup postfix!");

            int rerollsReceived = ArchipelagoHelper.AmountOfItemReceived("Progressive Re-roll");

            Debug.Log($"Setting encounter re-rolls to: {rerollsReceived}");

            // Set re-roll count based on received items
            Traverse.Create(__instance)
                .Field("_rerollsForEncounter")
                .SetValue(rerollsReceived);
        }

        /// <summary>
        /// Check location when a stamp or sticker is sold.
        /// </summary>
        [HarmonyPatch("SellItem")]
        [HarmonyPostfix]
        private static void SellItem_Postfix(Item item)
        {
            Debug.Log("EncounterController.SellItem Postfix!");
            Debug.Log($"Sold item: {item.Name}");

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
            Debug.Log($"{nameof(EncounterController)}.ShowScoreCalculation Prefix!");
            Debug.Log($"Word score: {finalScore.Score}");

            // Attempt to check word length locations
            CursedWordsArchipelago.Instance.TryCheckWordLocations("word_score", finalScore.Score);
        }

        /// <summary>
        /// Check location when a word length is submitted.
        /// </summary>
        [HarmonyPatch(nameof(EncounterController.SubmitWord))]
        [HarmonyPostfix]
        private static void SubmitWord_Postfix(EncounterController __instance, List<TileSelection> tiles)
        {
            Debug.Log($"{nameof(EncounterController)}.{nameof(EncounterController.SubmitWord)} Postfix!");
            Debug.Log($"Word length: {tiles.Count}");

            // Attempt to check word length locations
            CursedWordsArchipelago.Instance.TryCheckWordLocations("word_length", tiles.Count);
        }
    }
}
