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
    [HarmonyPatch(typeof(ScoreCalculation))]
    internal class ScoreCalculation_Patches
    {
        /// <summary>
        /// Check location(s) when word is scored.
        /// </summary>
        //[HarmonyPatch(nameof(ScoreCalculation.GetScoreFromScoreCalcInfo))]
        //[HarmonyPostfix]
        //private static void GetScoreFromScoreCalcInfo_Postfix(ScorePacket __result)
        //{
        //    Debug.Log($"{nameof(ScoreCalculation)}.{nameof(ScoreCalculation.GetScoreFromScoreCalcInfo)} Postfix!");
        //    Debug.Log($"Calculated Score: {__result.Score}");
        //}
    }
}
