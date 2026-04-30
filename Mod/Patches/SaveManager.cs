using BepInEx.Logging;
using HarmonyLib;
using Mod.Helpers;
using System.Diagnostics;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(SaveManager))]
    internal class SaveManager_Patches
    {
        /// <summary>
        /// Re-direct save path to archipelago-specific save file.
        /// </summary>
        [HarmonyPatch(nameof(SaveManager.GetSaveFileName))]
        [HarmonyPrefix]
        public static bool GetSaveFileName_Prefix(int slotIndex, ref string __result)
        {
            __result = $"{GameStatics.SaveDirectory}/slot_{((slotIndex == 0) ? GameStatics.SaveSlot : slotIndex)}_archipelago.sav";
            return false;
        }

        /// <summary>
        /// Reset archipelago data when a save is reset.
        /// </summary>
        [HarmonyPatch(nameof(SaveManager.ResetSave))]
        [HarmonyPostfix]
        public static void ResetSave_Postfix(int slotIndex)
        {
            ArchipelagoData.ResetDataForSaveSlot(slotIndex);
        }
    }
}
