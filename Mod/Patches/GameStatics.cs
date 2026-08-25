using HarmonyLib;
using Mod.Classes;
using Mod.Helpers;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(GameStatics))]
    internal class GameStatics_Patches : PatchBase
    {
        /// <summary>
        /// Override items requiring unlock with items not yet received from the multiworld.
        /// </summary>
        [HarmonyPatch(nameof(GameStatics.GetItemsRequiringUnlock))]
        [HarmonyPrefix]
        private static bool OnGetItemsRequiringUnlock_Prefix(ref List<Type> __result)
        {
            Logger.LogInfo($"{nameof(GameStatics)}.{nameof(GameStatics.GetItemsRequiringUnlock)} prefix!");

            // Return items not present in the unlocked cache
            __result = CursedWordsArchipelago.Instance.ItemTypeCache
                .Where(kvp => !CursedWordsArchipelago.Instance.UnlockedItemTypeCache.Contains(kvp.Key))
                .Select(kvp => kvp.Key)
                .ToList();

            return false;
        }

        [HarmonyPatch(nameof(GameStatics.GetNumberOfStages))]
        [HarmonyPrefix]
        private static bool OnGetNumberOfStages_Prefix(ref int __result)
        {
            // Set all characters to 5 stages
            __result = 5;

            return false;
        }
    }
}
