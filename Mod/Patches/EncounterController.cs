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
        [HarmonyPatch(nameof(EncounterController.SubmitWord))]
        [HarmonyPostfix]
        private static void SubmitWord_Postfix(List<TileSelection> tiles, List<string> words)
        {
            Debug.Log($"{nameof(EncounterController)}.{nameof(EncounterController.SubmitWord)} Postfix!");
            Debug.Log($"Word submitted with length: {tiles.Count}");

            // Attempt to check word length locations
            CursedWordsArchipelago.Instance.TryCheckLocations("word_length", tiles.Count);
        }

        [HarmonyPatch("ShowScoreCalculation")]
        [HarmonyPostfix]
        private static void ShowScoreCalculation_Postfix(ScorePacket finalScore)
        {
            Debug.Log($"{nameof(EncounterController)}.ShowScoreCalculation Postfix!");
            Debug.Log($"Word submitted with final score: {finalScore.Score}");

            // Attempt to check word length locations
            CursedWordsArchipelago.Instance.TryCheckLocations("word_score", finalScore.Score);
        }
    }
}
