using BepInEx.Logging;
using HarmonyLib;
using Mod.Classes;
using Mod.Helpers;
using Mod.Mappings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(SaveManager))]
    internal class SaveManager_Patches : PatchBase
    {
        /// <summary>
        /// Override bulk unlocks with custom ones.
        /// </summary>
        [HarmonyPatch(nameof(SaveManager.UnlockBulkUnlock))]
        [HarmonyPrefix]
        public static bool OnUnlockBulkUnlock(BulkUnlock bulkUnlock)
        {
            Logger.LogInfo($"{nameof(SaveManager)}.{nameof(SaveManager.UnlockBulkUnlock)} prefix!");

            bool result = Lookups.ValidBulkUnlockTypes.Contains(bulkUnlock.GetType());

            if (!result)
            {
                Logger.LogInfo($"Ignoring bulk unlock of type {bulkUnlock.Name}");
            }

            return result;
        }

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
