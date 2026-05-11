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

        //[HarmonyPatch(nameof(ShopItemSlot.ChangeShownStatus))]
        //[HarmonyPostfix]
        //private static void OnChangeShownStatus_Postfix(ref ShopItemSlot __instance, bool isShown)
        //{
        //    Debug.Log($"{nameof(ShopItemSlot)}.{nameof(ShopItemSlot.Populate)} postfix!");

        //    Player player = GameStatics.GetPlayer();

        //    if ((__instance.MyItemInStock.MyItem.IsStamp() && player.GetStamps().Count() >= ArchipelagoHelper.AmountOfItemReceived("Progressive Stamp Slot"))
        //        || (__instance.MyItemInStock.MyItem.IsSticker() && player.GetStickers().Count() >= ArchipelagoHelper.AmountOfItemReceived("Progresive Sticker Slot")))
        //    {
        //        // Get elements
        //        Button _buyButton = Traverse.Create(__instance)
        //        .Field("_buyButton")
        //        .GetValue<Button>();

        //        Image _buyButtonTopImage = Traverse.Create(__instance)
        //            .Field("_buyButtonTopImage")
        //            .GetValue<Image>();

        //        Image _buyButtonBackgroundImage = Traverse.Create(__instance)
        //            .Field("_buyButtonBackgroundImage")
        //            .GetValue<Image>();

        //        TextMeshProUGUI _buyButtonTMP = Traverse.Create(__instance)
        //            .Field("_buyButtonTMP")
        //            .GetValue<TextMeshProUGUI>();

        //        _buyButtonTMP.SetText("<#191919FF>BUY");
        //        _buyButtonTopImage.color = new Color32(158, 122, 45, byte.MaxValue);
        //        _buyButtonBackgroundImage.color = new Color32(77, 57, 15, byte.MaxValue);
        //        _buyButton.interactable = false;
        //    }
        //}

        //[HarmonyPatch(nameof(ShopItemSlot.Populate))]
        //[HarmonyPostfix]
        //private static void OnPopulate_Postfix(ref ShopItemSlot __instance)
        //{
        //    OnChangeShownStatus_Postfix(ref __instance, true);
        //}
    }
}
