using HarmonyLib;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(SaveFile))]
    internal class SaveFile_Patches
    {
        /// <summary>
        /// Prevent new save file data automatically including Rodman.
        /// </summary>
        /// <param name="__result"></param>
        [HarmonyPatch(nameof(SaveFile.SetNewSaveFileData))]
        [HarmonyPrefix]
        public static bool SetNewSaveFileData_Prefix()
        {
            Debug.Log("SaveFile.SetNewSaveFileData Prefix!");
            return false;
        }
    }
}
