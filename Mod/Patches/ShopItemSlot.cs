using HarmonyLib;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(ShopItemSlot))]
    internal class ShopItemSlot_Patches
    {
        /// <summary>
        /// Check location when a stamp or sticker is purchased.
        /// </summary>
        [HarmonyPatch(nameof(ShopItemSlot.OnBuyButtonClickedCallback))]
        [HarmonyPostfix]
        private static void OnBuyButtonClickedCallback_Postfix(ShopItemSlot __instance)
        {
            Debug.Log($"{nameof(ShopItemSlot)}.{nameof(ShopItemSlot.OnBuyButtonClickedCallback)} Postfix!");

            // Attempt to check shop locations
            CursedWordsArchipelago.Instance.TryCheckGenericLocations($"buy_{(__instance.IsStamp ? "stamp" : "sticker")}");
        }

        //InventoryVisualController

        /// <summary>
        /// Check location when a stamp or sticker is frozen.
        /// </summary>
        [HarmonyPatch(nameof(ShopItemSlot.OnFreezeButtonClickedCallback))]
        [HarmonyPostfix]
        private static void OnFreezeButtonClickedCallback_Postfix(ShopItemSlot __instance)
        {
            Debug.Log($"{nameof(ShopItemSlot)}.{nameof(ShopItemSlot.OnFreezeButtonClickedCallback)} Postfix!");

            // Attempt to check shop locations
            CursedWordsArchipelago.Instance.TryCheckGenericLocations($"freeze_{(__instance.IsStamp ? "stamp" : "sticker")}");
        }
    }
}
