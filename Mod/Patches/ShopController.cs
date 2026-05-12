using HarmonyLib;
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
    [HarmonyPatch(typeof(ShopController))]
    internal class ShopController_Patches : PatchBase
    {
        /// <summary>
        /// When generating goods in stock, re-populate the item pools so any received sticker/stamp bundles are available.
        /// </summary>
        [HarmonyPatch("GenerateGoodsInStock")]
        [HarmonyPostfix]
        private static IEnumerator OnGenerateGoodsInStock(IEnumerator __result, bool isFirstShop, bool isCascadingAnimations, bool isReroll, bool freeItem)
        {
            Logger.LogInfo($"{nameof(ShopController)}.GenerateGoodsInStock postfix!");

            // If this is a re-roll, attempt to send the check
            if (isReroll)
            {
                CursedWordsArchipelago.Instance.TryCheckGenericLocations("restock_shop");
            }

            // Re-populate item pools
            ItemPools.PopulatePools();

            // Perform existing actions in co-routine
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }
        }
    }
}
