using HarmonyLib;
using Mod.Extensions;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(EncounterController))]
    internal class EncounterController_Patches
    {
        [HarmonyPatch("GameSetup")]
        [HarmonyPostfix]
        private static IEnumerator OnGameSetup_Postfix(IEnumerator __result, EncounterController __instance)
        {
            Debug.Log("EncounterController.GameSetup postfix!");

            // Perform existing actions in coroutine
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }

            // Set re-roll amount per encounter
            int rerollsReceived = ArchipelagoHelper.AmountOfItemReceived("Progressive Re-roll");
            __instance.SetEncounterRerollAmount(rerollsReceived);
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
