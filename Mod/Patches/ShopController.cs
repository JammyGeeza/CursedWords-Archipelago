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
    [HarmonyPatch(typeof(ShopController))]
    internal class ShopController_Patches : PatchBase
    {
        /// <summary>
        /// Check location when the shop is restocked.
        /// </summary>
        [HarmonyPatch(nameof(ShopController.OnRerollButtonClickedCallback))]
        [HarmonyPostfix]
        private static void OnRerollButtonClickedCallback_Postfix(ShopController __instance)
        {
            Logger.LogInfo($"{nameof(ShopController)}.{nameof(ShopController.OnRerollButtonClickedCallback)} postfix!");

            // Attempt to check shop locations
            CursedWordsArchipelago.Instance.TryCheckGenericLocations("restock_shop");
        }
    }
}
