using HarmonyLib;
using Mod.Extensions;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(EncounterController))]
    internal class EncounterController_Patches : PatchBase
    {
        /// <summary>
        /// Override Grid Dimensions
        /// </summary>
        [HarmonyPatch("GetGridDimensions")]
        [HarmonyPostfix]
        private static void OnGetGridDimensions_Postfix(ref Vector2Int __result)
        {
            Logger.LogInfo("EncounterController.GetGridDimensions postfix!");

            // Ignore if progressive grid size disabled
            if (!ArchipelagoHelper.SlotData.ShuffleGridSize)
            {
                return;
            }

            // Get progressive grid size item received count and calculate grid size
            int received = ArchipelagoHelper.AmountOfItemReceived("Progressive Grid Size");
            int newSize = 3 + received;

            // If grid is smaller than received, ignore - this is probably because a boss modifier is applied
            if (__result.x < newSize || __result.y < newSize)
            {
                return;
            }

            // Set new grid size
            __result = new Vector2Int() { x = newSize, y = newSize };
        }

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
            int rerollsReceived = ArchipelagoHelper.AmountOfItemReceived("Progressive Encounter Re-roll");
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
        /// Attempt to handle items that have not yet been handled.
        /// </summary>
        [HarmonyPatch(nameof(EncounterController.SetEncounterThreadStage))]
        [HarmonyPostfix]
        private static void SetEncounterThreadStage_Postfix(EncounterThreadStage newThreadStage)
        {
            Logger.LogInfo($"{nameof(EncounterController)}.{nameof(EncounterController.SetEncounterThreadStage)} Postfix!");
            Logger.LogInfo($"Stage changed to: {newThreadStage}");

            // If now waiting for word submission...
            if (newThreadStage is EncounterThreadStage.WaitingForWordSubmission)
            {
                // Get item mappings where the cue is the start of an encounter
                foreach (KeyValuePair<string, CuedAction> encounterItem in ItemMappings.Map.Where(kvp => kvp.Value.Cue == ActionCue.Encounter))
                {
                    // Perform action if not handled the amount of times received.
                    for (int i = 0; i < ArchipelagoHelper.GetItemCountDifference(encounterItem.Key); i++)
                    {
                        CursedWordsArchipelago.Instance.QueueAction(encounterItem.Value.Action);
                    }
                }
            }
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

            // Attempt to check tile type locations
            foreach (TileSelection tile in tiles)
            {
                CursedWordsArchipelago.Instance.TryCheckTileLocations("use_tile", tile.SelectedTile);
            }
        }
    }
}
