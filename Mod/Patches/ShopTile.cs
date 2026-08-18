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

            // Attempt to check tile locations
            CursedWordsArchipelago.Instance.TryCheckTileLocations("buy_tile", __instance.MyTile);
        }
    }
}
