using HarmonyLib;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(ScatteredItemPools))]
    internal class ScatteredItemPools_Patches : PatchBase
    {
        /// <summary>
        /// Override fallback to Graduation Cap for Scattered Items, as it isn't in the emoji dictionary and crashes.
        /// (I spoke to Skyeward about this, but this scenario should never happen in Vanilla!)
        /// </summary>
        [HarmonyPatch("GetRarityWeightedItem")]
        [HarmonyPostfix]
        private static void GetRarityWeightedItem_Postfix(List<Item> items, ref Item __result)
        {
            Logger.LogDebug($"{nameof(ScatteredItemPools)}.GetRarityWeightedItem postfix!");

            // If graduation cap, override it
            if (__result is GraduationCap)
            {
                // Try to replace with generic mult item
                __result = ScatteredItemPools.GetRandomGenericMultItem();

                // If it's STILL a graduation cap, try any random item
                if (__result is GraduationCap)
                {
                    __result = ScatteredItemPools.GetRandomItem();
                }
            }
        }
    }
}
