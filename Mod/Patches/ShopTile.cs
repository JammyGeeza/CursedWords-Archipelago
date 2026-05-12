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
    [HarmonyPatch(typeof(ShopTile))]
    internal class ShopTile_Patches : PatchBase
    {
        /// <summary>
        /// Check location when a tile is purchased.
        /// </summary>
        [HarmonyPatch(nameof(ShopTile.OnBuyButtonClickedCallback))]
        [HarmonyPostfix]
        private static void OnBuyButtonClickedCallback_Postfix(ShopTile __instance)
        {
            Logger.LogInfo($"{nameof(ShopTile)}.{nameof(ShopTile.OnBuyButtonClickedCallback)} Postfix!");

            // Attempt to check shop locations
            CursedWordsArchipelago.Instance.TryCheckGenericLocations("buy_tile");
        }
    }
}
