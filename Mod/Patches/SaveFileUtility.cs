using BepInEx.Logging;
using FullSerializer;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(SaveFileUtility))]
    internal class SaveFileUtility_Patches
    {
        /// <summary>
        /// Override default save data from newly created saves.
        /// </summary>
        [HarmonyPatch(nameof(SaveFileUtility.GetSaveFileFromFsData))]
        [HarmonyPostfix]
        public static void GetSaveFileFromFsData_Postfix(Dictionary<string, fsData> saveFileDict, ref SaveFile __result)
        {
            // If save has not been entered before, set save data to a 'blank' state
            if (!__result.HasEnteredSaveFile)
            {
                __result.IsTutorialComplete = true;
                __result.CharacterHighestCompletedAscensions = new Dictionary<Type, int>();
                __result.BulkUnlocksUnlocked = new List<Type>();
            };
        }
    }
}
