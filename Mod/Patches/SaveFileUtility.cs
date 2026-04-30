using BepInEx.Logging;
using FullSerializer;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(SaveFileUtility))]
    internal class SaveFileUtility_Patches
    {
        /// <summary>
        /// Check if archipelago connection data has been stored in the save file
        /// </summary>
        /// <param name="__result"></param>
        //[HarmonyPatch(nameof(SaveFileUtility.GetSaveFileFromFsData))]
        //[HarmonyPrefix]
        public static void GetSaveFileFromFsData_Prefix(ref Dictionary<string, fsData> saveFileDict)
        {
            Debug.Log("SaveFileUtility.GetSaveFileFromFsData Prefix!");
        }

        //[HarmonyPatch(nameof(SaveFileUtility.GetSaveFileFromFsData))]
        //[HarmonyPostfix]
        public static void GetSaveFileFromFsData_Postfix(ref Dictionary<string, fsData> saveFileDict, ref SaveFile __result)
        {

        }
    }
}
